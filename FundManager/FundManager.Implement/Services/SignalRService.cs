using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.SignalRHubs;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<PatronSignatureHub> _hubContext;
        private readonly IPatronDeviceService _patronDeviceService;
        private readonly IPatronRepository _patronRepository;
        private readonly ILogger<SignalRService> _logger;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SignalRService(
            IHubContext<PatronSignatureHub> hubContext,
            IPatronDeviceService patronDeviceService,
            IPatronRepository patronRepository,
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork,
            ILogger<SignalRService> logger)
        {
            _hubContext = hubContext;
            _patronDeviceService = patronDeviceService;
            _patronRepository = patronRepository;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SendSignatureRequestToDeviceAsync(int patronId, int staffDeviceId)
        {
            try
            {
                // Find iPad device available
                var patronDevice = await _patronDeviceService.GetAvailableDeviceForStaffAsync(staffDeviceId);

                if (patronDevice == null)
                {
                    _logger.LogWarning("No available iPad device found for staff {StaffDeviceId}", staffDeviceId);
                    throw new InvalidOperationException($"No available iPad found for staff device {staffDeviceId}");
                }

                _logger.LogInformation("Found device: {DeviceName}, ConnectionId: {ConnectionId}, IsOnline: {IsOnline}",
                    patronDevice.DeviceName,
                    patronDevice.ConnectionId,
                    patronDevice.IsOnline);

                // Verify connection exists
                if (string.IsNullOrEmpty(patronDevice.ConnectionId))
                {
                    _logger.LogError("Device {DeviceName} has empty ConnectionId", patronDevice.DeviceName);
                    throw new InvalidOperationException($"Device {patronDevice.DeviceName} has no valid connection");
                }

                // Get patron information
                var patron = await _patronRepository.GetPatronByIdAsync(patronId);

                if (patron == null)
                {
                    _logger.LogWarning("Patron {PatronId} not found", patronId);
                    throw new InvalidOperationException($"Patron {patronId} not found");
                }

                // LOGIC: Check if patron has pending session from ANY staff device
                //var signatureCompleted = await _patronDeviceService.GetCompletedSessionByPatronIdAsync(patronId);

                var anyPendingSession = await _patronDeviceService.GetPendingSessionByPatronIdAsync(patronId);

                if (anyPendingSession != null)
                {
                    // Check if pending session is from ANOTHER staff device
                    if (anyPendingSession.StaffDeviceId != staffDeviceId)
                    {
                        var otherStaffDevice = anyPendingSession.StaffDevice?.DeviceName ?? $"StaffDevice ID {anyPendingSession.StaffDeviceId}";
                        var otherPatronDevice = anyPendingSession.PatronDevice?.DeviceName ?? $"iPad ID {anyPendingSession.PatronDeviceId}";

                        _logger.LogWarning(
                            "Patron {PatronId} already has a pending session {SessionId} from another PC '{OtherStaffDevice}' → iPad '{OtherPatronDevice}'. Current request from StaffDevice {StaffDeviceId}",
                            patronId,
                            anyPendingSession.Id,
                            otherStaffDevice,
                            otherPatronDevice,
                            staffDeviceId);

                        if (anyPendingSession.StaffDevice?.StaffUserName != "Unknown_Employee")
                            throw new InvalidOperationException(
                          $"This patron is already being served by {otherStaffDevice} on {otherPatronDevice} - Employee {anyPendingSession.StaffDevice?.StaffUserName}.");

                        throw new InvalidOperationException(
                            $"This patron is already being served by {otherStaffDevice} on {otherPatronDevice}.");
                    }

                    // Session is from SAME staff device → Allow re-send (patron still being served here)
                    _logger.LogInformation(
                        "Patron {PatronId} has a pending session {SessionId} from the SAME StaffDevice {StaffDeviceId}. Allowing re-send...",
                        patronId,
                        anyPendingSession.Id,
                        staffDeviceId);
                }

                // Create new session (or re-send to existing session's device)
                var session = await _patronDeviceService.CreateSignatureSessionAsync(
                        patronId,
                        staffDeviceId,
                        patronDevice.Id);

                _logger.LogInformation("Created session {SessionId} for patron {PatronId}", session.Id, patronId);

                // Prepare message
                var message = new
                {
                    sessionId = session.Id,
                    patronId = patron.Id,
                    fullName = $"{patron.FirstName} {patron.LastName}",
                    requesterName = CommonConstants.SystemUser,
                    requestTime = DateTime.Now,
                    expiryTime = DateTime.Now.AddMinutes(30),
                    staffDeviceId = staffDeviceId,
                    language = patron.Language,
                    patronData = new
                    {
                        firstName = patron.FirstName,
                        lastName = patron.LastName,
                        birthday = patron.Birthday,
                        address = patron.Address,
                        language = patron.Language
                    }
                };

                // Log message details
                _logger.LogInformation("Sending message to ConnectionId: {ConnectionId}, Message: {@Message}",
                    patronDevice.ConnectionId, message);

                // Send to specific client using ConnectionId
                var device = await _patronDeviceService.GetAvailableDeviceForStaffAsync(staffDeviceId);
                await _hubContext.Clients.Client(device!.ConnectionId)
                    .SendAsync("ShowSignatureRequest", message);

                _logger.LogInformation("Successfully sent signature request to iPad {DeviceName} (ConnectionId: {ConnectionId}) for patron {PatronId}",
                    patronDevice.DeviceName, patronDevice.ConnectionId, patronId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending signature request for patron {PatronId}", patronId);
                throw;
            }
        }
        public async Task NotifyNewRegistrationAsync(Patron patron)
        {
            try
            {
                _logger.LogInformation(
                    "[NotifyNewRegistration] Broadcasting new registration - PatronId: {PatronId}",
                    patron.Id);

                var message = new
                {
                    patronId = patron.Id,
                    timestamp = DateTime.Now,
                    fullName = $"{patron.LastName} {patron.FirstName}",
                    message = "New patron registered"
                };

                // Broadcast to ALL connected clients
                await _hubContext.Clients.All.SendAsync("NewRegistration", message);

                _logger.LogInformation(
                    "[NotifyNewRegistration] Successfully broadcasted - PatronId: {PatronId}",
                    patron.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[NotifyNewRegistration] Failed to broadcast - PatronId: {PatronId}",
                    patron.Id);
                throw;
            }
        }
        public async Task NotifyStaffSignatureCompletedAsync(int staffDeviceId, int sessionId, Patron patron)
        {
            // Generate correlation ID for tracking
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            // Build payload
            var payload = new
            {
                correlationId = correlationId, // Add for client logging
                sessionId = sessionId,
                patronId = patron.Id,
                success = true,
                completedAt = DateTime.Now,
                fullName = $"{patron.FirstName} {patron.LastName}",
                mobilePhone = patron.PhoneNumber
            };

            var payloadJson = JsonSerializer.Serialize(payload);

            // Persist notification BEFORE attempting send
            var notification = new Notification
            {
                StaffDeviceId = staffDeviceId,
                SessionId = sessionId,
                PayloadJson = payloadJson,
                Status = NotificationStatus.Pending, // Keep Pending until ACK received
                AttemptCount = 0,
                CreatedAt = DateTime.Now,
                CreatedBy = CommonConstants.SystemUser
            };

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "[SignalR][{CorrelationId}] Created notification {NotificationId} for Staff_{StaffId}, Session {SessionId}",
                correlationId, notification.Id, staffDeviceId, sessionId);

            // Try immediate delivery
            try
            {
                var staffDevice = await _patronDeviceService.GetStaffDeviceByIdAsync(staffDeviceId);

                if (staffDevice == null)
                {
                    _logger.LogWarning(
                        "[SignalR][{CorrelationId}] Staff_{StaffId} not found, notification {NotificationId} queued",
                        correlationId, staffDeviceId, notification.Id);
                    return; // Keep status as "Pending" for retry
                }

                if (string.IsNullOrEmpty(staffDevice.ConnectionId))
                {
                    _logger.LogWarning(
                        "[SignalR][{CorrelationId}] Staff_{StaffId} offline, notification {NotificationId} queued",
                        correlationId, staffDeviceId, notification.Id);
                    return; // Keep status as "Pending" for retry
                }

                // Attempt send
                _logger.LogInformation(
                    "[SignalR][{CorrelationId}] Sending to ConnectionId {ConnectionId}",
                    correlationId, staffDevice.ConnectionId);

                await _hubContext.Clients.Client(staffDevice.ConnectionId)
                    .SendAsync("signatureCompleted", payload);

                // Mark as "Sent" (waiting for ACK), NOT "Delivered"
                // Status will change to "Delivered" only when client sends ACK
                notification.Status = NotificationStatus.Sent; // Sent but not confirmed
                notification.SentAt = DateTime.Now;
                notification.AttemptCount++;
                notification.UpdatedAt = DateTime.Now;
                _notificationRepository.Update(notification);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "[SignalR][{CorrelationId}] Sent notification {NotificationId} to ConnectionId {ConnectionId}. Waiting for ACK...",
                    correlationId, notification.Id, staffDevice.ConnectionId);
            }
            catch (Exception ex)
            {
                // On failure, keep status as "Pending" for retry
                notification.AttemptCount++;
                notification.LastError = $"[{correlationId}] {ex.Message}";
                notification.UpdatedAt = DateTime.Now;
                _notificationRepository.Update(notification);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogWarning(ex,
                    "[SignalR][{CorrelationId}] Failed to send notification {NotificationId}, will retry",
                    correlationId, notification.Id);
            }
        }
        public async Task ResendPendingNotificationsForStaffAsync(int staffDeviceId, string connectionId)
        {
            // This already handles "Pending" correctly on reconnect
            var pending = await _notificationRepository.GetPendingByStaffAsync(staffDeviceId);
            if (pending == null || pending.Count == 0) return;

            _logger.LogInformation(
                "[SignalR] Resending {Count} pending notifications to Staff_{StaffId}",
                pending.Count, staffDeviceId);

            foreach (var note in pending)
            {
                try
                {
                    var payload = JsonSerializer.Deserialize<object>(note.PayloadJson) ?? note.PayloadJson;

                    await _hubContext.Clients.Client(connectionId).SendAsync("signatureCompleted", payload);

                    // Mark as "Sent" (waiting for ACK)
                    note.Status = NotificationStatus.Sent;
                    note.SentAt = DateTime.Now;
                    note.AttemptCount++;
                    note.UpdatedAt = DateTime.Now;
                    _notificationRepository.Update(note);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation(
                        "[SignalR] Resent notification {NotificationId} to Staff_{StaffId}. Waiting for ACK...",
                        note.Id, staffDeviceId);
                }
                catch (Exception ex)
                {
                    note.AttemptCount++;
                    note.LastError = ex.Message;
                    note.UpdatedAt = DateTime.Now;
                    _notificationRepository.Update(note);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogWarning(ex,
                        "[SignalR] Failed to resend notification {NotificationId}",
                        note.Id);
                }
            }
        }
        public async Task AcknowledgeNotificationAsync(int staffDeviceId, int sessionId)
        {
            var allNotes = await _notificationRepository.GetAllPendingOrSentBySessionAndStaffAsync(sessionId, staffDeviceId);
            if (!allNotes.Any())
            {
                _logger.LogInformation("[SignalR] No pending/sent notifications found for session {SessionId}, staff {StaffId}",
                    sessionId, staffDeviceId);
                return;
            }

            _logger.LogInformation("[SignalR] Found {Count} notifications to acknowledge for session {SessionId}, staff {StaffId}",
                allNotes.Count, sessionId, staffDeviceId);

            var now = DateTime.Now;
            foreach (var itemNote in allNotes)
            {
                itemNote.Status = NotificationStatus.Delivered;
                itemNote.DeliveredAt = now;
                itemNote.UpdatedAt = now;
                _notificationRepository.Update(itemNote);

                _logger.LogDebug("[SignalR] Marking notification {NotificationId} as Delivered", itemNote.Id);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("[SignalR] Successfully acknowledged {Count} notification(s) for session {SessionId}, staff {StaffId}",
                allNotes.Count, sessionId, staffDeviceId);
        }

        //public async Task NotifyStaffSignatureCompletedAsync(int staffDeviceId, int sessionId, Patron patron)
        //{
        //    try
        //    {
        //        var message = new
        //        {
        //            sessionId = sessionId,
        //            patronId = patron.ID,
        //            success = true,
        //            fullName = $"{patron.FirstName} {patron.LastName}",
        //            mobilePhone = patron.PhoneNumber
        //        };

        //        var groupName = $"Staff_{staffDeviceId}";

        //        _logger.LogInformation("📤 Sending completion notification to group '{GroupName}': {@Message}",
        //            groupName, message);

        //        await _hubContext.Clients.Group(groupName)
        //            .SendAsync("SignatureCompleted", message);

        //        _logger.LogInformation("Successfully sent notification to {GroupName} for patron {PatronId}",
        //            groupName, patron.ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "❌ Error notifying staff {StaffDeviceId} about signature completion for patron {PatronId}",
        //            staffDeviceId, patron.ID);
        //        throw;
        //    }
        //}
    }
}