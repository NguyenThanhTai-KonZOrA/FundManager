using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Response;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FundManager.Implement.SignalRHubs
{
    public class PatronSignatureHub : Hub
    {
        private readonly ILogger<PatronSignatureHub> _logger;
        private readonly IPatronDeviceService _patronDeviceService;
        private readonly ISignalRService _signalRService;

        public PatronSignatureHub(
            ILogger<PatronSignatureHub> logger,
            IPatronDeviceService patronDeviceService,
            ISignalRService signalRService)
        {
            _logger = logger;
            _patronDeviceService = patronDeviceService;
            _signalRService = signalRService;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var httpContext = Context.GetHttpContext();
            var deviceName = httpContext?.Request.Query["deviceName"].ToString();
            var deviceType = httpContext?.Request.Query["deviceType"].ToString(); // "patron" or "staff"

            if (!string.IsNullOrEmpty(deviceName))
            {
                if (deviceType?.ToLower() == "staff")
                {
                    await _patronDeviceService.UpdateStaffDeviceConnectionIdAsync(deviceName, connectionId);
                    var staffDevice = await _patronDeviceService.GetStaffDeviceByHostNameAsync(deviceName);
                    if (staffDevice != null)
                    {
                        var groupName = $"Staff_{staffDevice.Id}";
                        await Groups.AddToGroupAsync(connectionId, groupName);

                        // Resend pending DB-backed notifications
                        await _signalRService.ResendPendingNotificationsForStaffAsync(staffDevice.Id, connectionId);
                    }
                }
                else
                {
                    await _patronDeviceService.UpdateConnectionIdAsync(deviceName, connectionId);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            //_logger.LogInformation("Client disconnected: {ConnectionId}, Exception: {Exception}", connectionId, exception?.Message);

            // Check if it's a StaffDevice
            var staffDevice = await _patronDeviceService.GetStaffDeviceByConnectionIdAsync(connectionId);
            if (staffDevice != null)
            {
                await _patronDeviceService.SetStaffDeviceOfflineAsync(connectionId);
                //_logger.LogInformation("StaffDevice {DeviceName} set offline", staffDevice.DeviceName);
            }
            else
            {
                // It's a PatronDevice
                await _patronDeviceService.SetDeviceOfflineAsync(connectionId);
                //_logger.LogInformation("PatronDevice set offline");
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Called by Staff Web App to register
        /// </summary>
        public async Task RegisterStaffDevice(string deviceName, int staffDeviceId)
        {
            try
            {
                var connectionId = Context.ConnectionId;

                _logger.LogInformation("[RegisterStaffDevice]: DeviceName={DeviceName}, StaffDeviceId={StaffDeviceId}, ConnectionId={ConnectionId}",
                    deviceName, staffDeviceId, connectionId);

                // Update ConnectionId
                var success = await _patronDeviceService.UpdateStaffDeviceConnectionIdAsync(deviceName, connectionId);

                if (success)
                {
                    // Join staff group for receiving signature completed events
                    await Groups.AddToGroupAsync(connectionId, $"Staff_{staffDeviceId}");

                    await Clients.Caller.SendAsync("StaffDeviceRegistered", new
                    {
                        success = true,
                        staffDeviceId = staffDeviceId,
                        deviceName = deviceName,
                        connectionId = connectionId,
                        message = "Staff device registered successfully"
                    });

                    _logger.LogInformation("StaffDevice {DeviceName} registered and joined group Staff_{StaffDeviceId}",
                        deviceName, staffDeviceId);
                }
                else
                {
                    await Clients.Caller.SendAsync("StaffDeviceRegistered", new
                    {
                        success = false,
                        message = $"StaffDevice '{deviceName}' not found in database"
                    });

                    _logger.LogWarning("❌ StaffDevice {DeviceName} not found", deviceName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering staff device {DeviceName}", deviceName);

                await Clients.Caller.SendAsync("StaffDeviceRegistered", new
                {
                    success = false,
                    message = $"Registration failed: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Called by iPad client to register itself
        /// </summary>
        public async Task RegisterPatronDevice(string deviceName, string? macAddress, string? ipAddress)
        {
            try
            {
                var connectionId = Context.ConnectionId;

                _logger.LogInformation("Registering PatronDevice: {DeviceName}, ConnectionId: {ConnectionId}",
                    deviceName, connectionId);

                // Add or update device
                var device = await _patronDeviceService.AddOrUpdatePatronDeviceAsync(
                    deviceName,
                    connectionId,
                    macAddress,
                    ipAddress
                );

                await Clients.Caller.SendAsync("DeviceRegistered", new
                {
                    success = true,
                    deviceId = device.Id,
                    deviceName = device.DeviceName,
                    connectionId = device.ConnectionId,
                    message = "Device registered successfully"
                });

                _logger.LogInformation("PatronDevice {DeviceName} registered with ID {DeviceId}",
                    deviceName, device.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering PatronDevice {DeviceName}", deviceName);

                await Clients.Caller.SendAsync("DeviceRegistered", new
                {
                    success = false,
                    message = $"Registration failed: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Called by iPad client to confirm signature submission
        /// </summary>
        public async Task SignatureSubmitted(int sessionId, string signatureDataUrl)
        {
            try
            {
                _logger.LogInformation("Signature submitted for session: {SessionId}", sessionId);

                var (session, patron) = await _patronDeviceService.CompleteSignatureSessionAsync(sessionId, signatureDataUrl);

                // Notify staff admin panel via group
                var groupName = $"Staff_{session.StaffDeviceId}";
                _logger.LogInformation("Sending SignatureCompleted to group: {GroupName}", groupName);

                await Clients.Group(groupName).SendAsync("signatureCompleted", new
                {
                    sessionId = sessionId,
                    patronId = patron.Id,
                    success = true,
                    completedAt = DateTime.Now,
                    fullName = $"{patron.FirstName} {patron.LastName}",
                    mobilePhone = patron.PhoneNumber
                });

                await Clients.Caller.SendAsync("SignatureProcessed", new
                {
                    success = true,
                    message = "Signature saved successfully"
                });

                _logger.LogInformation("Signature processed for session {SessionId}", sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing signature submission");
                await Clients.Caller.SendAsync("SignatureProcessed", new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Heartbeat to keep connection alive
        /// </summary>
        public async Task SendHeartbeat(string deviceType, string deviceId)
        {
            if (deviceType?.ToLower() == "staff")
            {
                var connectionId = Context.ConnectionId;
                // _logger.LogInformation("Received heartbeat from StaffDevice: {DeviceId}", deviceId);
                if (int.TryParse(deviceId, out int deviceStaffId))
                {
                    var staffDevice = await _patronDeviceService.GetStaffDeviceByIdAsync(deviceStaffId);
                    if (staffDevice != null && staffDevice.ConnectionId != connectionId)
                    {
                        await _patronDeviceService.UpdateStaffDeviceConnectionIdAsync(staffDevice.DeviceName, connectionId);
                    }
                }
                await _patronDeviceService.UpdateHeartbeatAsync(connectionId, deviceType);
                //var staffDevice = await _patronDeviceService.GetStaffDeviceByConnectionIdAsync(Context.ConnectionId);
                //if (staffDevice != null)
                //{
                //    await _patronDeviceService.UpdateHeartbeatAsync(Context.ConnectionId, deviceType);
                //}
            }
            else
            {
                _logger.LogInformation("Received heartbeat from PatronDevice: {DeviceId}", deviceId);
                await _patronDeviceService.UpdateHeartbeatAsync(Context.ConnectionId, deviceType!);
            }

            // _logger.LogInformation("Heartbeat processed for {DeviceType} {DeviceId}", deviceType, deviceId);
            await Clients.Caller.SendAsync("HeartbeatAck", DateTime.Now);
        }

        // Client calls this to ACK a specific session (server will mark DB record delivered)
        public async Task AcknowledgeSignatureCompleted(int sessionId)
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var deviceName = httpContext?.Request.Query["deviceName"].ToString();
                var staffDevice = deviceName != null ? await _patronDeviceService.GetStaffDeviceByHostNameAsync(deviceName) : null;
                if (staffDevice != null)
                {
                    await _signalRService.AcknowledgeNotificationAsync(staffDevice.Id, sessionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed processing ACK for session {SessionId}", sessionId);
            }
        }

        /// <summary>
        /// Check if a StaffDevice is registered with the backend
        /// </summary>
        /// <param name="staffDeviceId">The ID of the staff device to check</param>
        /// <returns>StaffDeviceRegistrationResponse with registration status</returns>
        public async Task<StaffDeviceRegistrationResponse> CheckStaffDeviceRegistration(int staffDeviceId)
        {
            try
            {
                _logger.LogInformation("Checking registration for StaffDeviceId: {StaffDeviceId}", staffDeviceId);

                var staffDevice = await _patronDeviceService.GetStaffDeviceByIdAsync(staffDeviceId);

                if (staffDevice == null)
                {
                    _logger.LogWarning("❌ StaffDevice with ID {StaffDeviceId} not found", staffDeviceId);

                    return new StaffDeviceRegistrationResponse
                    {
                        IsRegistered = false,
                        StaffDeviceId = null,
                        DeviceName = null,
                        ConnectionId = null,
                        IsOnline = false,
                        Message = $"Staff device with ID {staffDeviceId} is not registered in the system"
                    };
                }

                var isOnline = !string.IsNullOrEmpty(staffDevice.ConnectionId) &&
                               staffDevice.IsActive == true;

                var response = new StaffDeviceRegistrationResponse
                {
                    IsRegistered = true,
                    StaffDeviceId = staffDevice.Id,
                    DeviceName = staffDevice.DeviceName,
                    ConnectionId = staffDevice.ConnectionId,
                    IsOnline = isOnline,
                    StaffUserName = staffDevice.StaffUserName,
                    LastHeartbeat = staffDevice.LastHeartbeat,
                    Message = isOnline
                        ? $"Staff device '{staffDevice.DeviceName}' is registered and online"
                        : $"Staff device '{staffDevice.DeviceName}' is registered but offline"
                };

                _logger.LogInformation("StaffDevice check completed: {DeviceName}, Online: {IsOnline}",
                    staffDevice.DeviceName, isOnline);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking staff device registration for ID {StaffDeviceId}", staffDeviceId);

                return new StaffDeviceRegistrationResponse
                {
                    IsRegistered = false,
                    Message = $"Error checking device registration: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Check if a StaffDevice is registered by device name
        /// </summary>
        /// <param name="deviceName">The name/hostname of the staff device</param>
        /// <returns>StaffDeviceRegistrationResponse with registration status</returns>
        public async Task<StaffDeviceRegistrationResponse> CheckStaffDeviceRegistrationByName(string deviceName)
        {
            try
            {
                _logger.LogInformation("Checking registration for StaffDevice: {DeviceName}", deviceName);

                var staffDevice = await _patronDeviceService.GetStaffDeviceByHostNameAsync(deviceName);

                if (staffDevice == null)
                {
                    _logger.LogWarning("❌ StaffDevice '{DeviceName}' not found", deviceName);

                    return new StaffDeviceRegistrationResponse
                    {
                        IsRegistered = false,
                        DeviceName = deviceName,
                        Message = $"Staff device '{deviceName}' is not registered in the system"
                    };
                }

                var isOnline = !string.IsNullOrEmpty(staffDevice.ConnectionId) &&
                               staffDevice.IsActive == true;

                var response = new StaffDeviceRegistrationResponse
                {
                    IsRegistered = true,
                    StaffDeviceId = staffDevice.Id,
                    DeviceName = staffDevice.DeviceName,
                    ConnectionId = staffDevice.ConnectionId,
                    IsOnline = isOnline,
                    StaffUserName = staffDevice.StaffUserName,
                    LastHeartbeat = staffDevice.LastHeartbeat,
                    Message = isOnline
                        ? $"Staff device '{staffDevice.DeviceName}' is registered and online"
                        : $"Staff device '{staffDevice.DeviceName}' is registered but offline"
                };

                _logger.LogInformation("StaffDevice check completed: {DeviceName}, Online: {IsOnline}",
                    staffDevice.DeviceName, isOnline);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking staff device registration for name {DeviceName}", deviceName);

                return new StaffDeviceRegistrationResponse
                {
                    IsRegistered = false,
                    DeviceName = deviceName,
                    Message = $"Error checking device registration: {ex.Message}"
                };
            }
        }
    }
}