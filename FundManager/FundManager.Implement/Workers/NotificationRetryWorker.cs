using FundManager.Common.Constants;
using FundManager.Implement.Repositories.Interface;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.SignalRHubs;
using FundManager.Implement.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FundManager.Implement.Workers
{
    /// <summary>
    /// Background worker that periodically scans pending Notification rows
    /// and retries delivery via SignalR. Uses IServiceScopeFactory to resolve scoped dependencies.
    /// </summary>
    public class NotificationRetryWorker : BackgroundService
    {
        private readonly ILogger<NotificationRetryWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<PatronSignatureHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _interval;
        private readonly int _maxAttempts;

        public NotificationRetryWorker(
            ILogger<NotificationRetryWorker> logger,
            IServiceScopeFactory scopeFactory,
            IHubContext<PatronSignatureHub> hubContext,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _configuration = configuration;

            var intervalSeconds = _configuration.GetValue<int?>("NotificationRetry:IntervalSeconds") ?? 15;
            _interval = TimeSpan.FromSeconds(Math.Max(5, intervalSeconds));
            _maxAttempts = _configuration.GetValue<int?>("NotificationRetry:MaxAttempts") ?? 5;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[NotificationRetryWorker] Starting. Interval={Interval}s, MaxAttempts={MaxAttempts}",
                _interval.TotalSeconds, _maxAttempts);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingNotificationsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[NotificationRetryWorker] Unexpected error while processing pending notifications");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException) { /* shutdown */ }
            }

            _logger.LogInformation("[NotificationRetryWorker] Stopped.");
        }

        private async Task ProcessPendingNotificationsAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var patronDeviceService = scope.ServiceProvider.GetRequiredService<IPatronDeviceService>();

            // FIX: Get both "Pending" AND "Sent" (Sent but no ACK = needs retry)
            var pending = await notificationRepository.GetAllPendingOrSentAsync(_maxAttempts);

            if (pending == null || pending.Count == 0) return;

            _logger.LogInformation(
                "[NotificationRetryWorker] Found {Count} notifications to process (Pending or Sent without ACK)",
                pending.Count);

            var byStaff = pending.GroupBy(n => n.StaffDeviceId);

            foreach (var group in byStaff)
            {
                if (ct.IsCancellationRequested) break;

                var staffDeviceId = group.Key;
                try
                {
                    var staffDevice = await patronDeviceService.GetStaffDeviceByIdAsync(staffDeviceId);
                    var connectionId = staffDevice?.ConnectionId;

                    foreach (var note in group)
                    {
                        if (ct.IsCancellationRequested) break;

                        if (note.AttemptCount >= _maxAttempts)
                        {
                            if (note.Status != NotificationStatus.Failed)
                            {
                                note.Status = NotificationStatus.Failed;
                                note.LastError = "Max attempts reached without ACK";
                                note.UpdatedAt = DateTime.Now;
                                notificationRepository.Update(note);
                                await unitOfWork.SaveChangesAsync();

                                _logger.LogWarning(
                                    "[NotificationRetryWorker] Notification {Id} marked Failed (max attempts, no ACK)",
                                    note.Id);
                            }
                            continue;
                        }

                        // Prepare payload
                        object? payload = null;
                        try
                        {
                            payload = JsonSerializer.Deserialize<object>(note.PayloadJson) ?? note.PayloadJson;
                        }
                        catch
                        {
                            payload = note.PayloadJson;
                        }

                        // Try send (keep status as "Sent" until ACK)
                        if (!string.IsNullOrEmpty(connectionId))
                        {
                            try
                            {
                                await _hubContext.Clients.Client(connectionId)
                                    .SendAsync("signatureCompleted", payload, ct);

                                // Mark as "Sent" (waiting for ACK)
                                note.Status = NotificationStatus.Sent;
                                note.SentAt = DateTime.Now;
                                note.AttemptCount++;
                                note.UpdatedAt = DateTime.Now;
                                notificationRepository.Update(note);
                                await unitOfWork.SaveChangesAsync();

                                _logger.LogInformation(
                                    "[NotificationRetryWorker] Sent notification {Id} to Staff_{StaffId}. Waiting for ACK...",
                                    note.Id, staffDeviceId);

                                continue;
                            }
                            catch (Exception sendEx)
                            {
                                note.AttemptCount++;
                                note.LastError = sendEx.Message;
                                note.UpdatedAt = DateTime.Now;
                                notificationRepository.Update(note);
                                await unitOfWork.SaveChangesAsync();

                                _logger.LogWarning(sendEx,
                                    "[NotificationRetryWorker] Failed to send notification {Id}",
                                    note.Id);
                            }
                        }
                        else
                        {
                            _logger.LogDebug(
                                "[NotificationRetryWorker] Staff_{StaffId} offline, skipping notification {Id}",
                                staffDeviceId, note.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "[NotificationRetryWorker] Error processing Staff_{StaffId}",
                        staffDeviceId);
                }
            }
        }
    }
}