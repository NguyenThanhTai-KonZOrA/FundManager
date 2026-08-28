using FundManager.API.Helpers;
using FundManager.Implement.Models.Request;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [ApiController]
    [Route("api/patron-device")]
    public class PatronDeviceController : ControllerBase
    {
        private readonly IPatronDeviceService _deviceService;
        private readonly ILogger<PatronDeviceController> _logger;

        public PatronDeviceController(
            IPatronDeviceService deviceService,
            ILogger<PatronDeviceController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        /// <summary>
        /// Register or update iPad device
        /// </summary>
        /// <summary>
        /// Register or update iPad device
        /// </summary>
        [HttpPost("register-device")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            try
            {
#if DEBUG
                request.DeviceName = "Fake_Ipad_HOSTNAME";
#elif RELEASE
                request.DeviceName = await IpAddressHepler.GetClientComputerNameAsync(HttpContext) ?? "Fake_Ipad_HOSTNAME";                
#endif
                _logger.LogInformation("[RegisterDevice]: Called with DeviceName: {DeviceName}, MacAddress: {MacAddress}, IpAddress: {IpAddress}",
                    request.DeviceName, request.MacAddress, request.IpAddress);

                if (string.IsNullOrEmpty(request.DeviceName))
                {
                    _logger.LogWarning("[RegisterDevice]: Device name is missing in the request");
                    return BadRequest(new { success = false, message = "Device name is required" });
                }

                // Step 1: Register or update the iPad device (only update ConnectionId if already exists)
                var device = await _deviceService.AddOrUpdatePatronDeviceAsync(
                    request.DeviceName,
                    request.ConnectionId,
                    request.MacAddress,
                    request.IpAddress
                );

                // Step 2: Check if this iPad is mapped to any Staff PC
                var mapping = await _deviceService.GetMappingByPatronDeviceNameAsync(request.DeviceName);
                int? staffDeviceId = mapping?.StaffDeviceId;

                _logger.LogInformation("[RegisterDevice]: Device '{DeviceName}' registered with ID: {DeviceId}, Mapped StaffDevice: {StaffDeviceId}",
                    request.DeviceName, device.Id, staffDeviceId ?? 0);

                var data = new
                {
                    id = device.Id,
                    deviceName = device.DeviceName,
                    connectionId = device.ConnectionId,
                    isOnline = device.IsOnline,
                    isAvailable = device.IsAvailable,
                    macAddress = device.MacAddress,
                    ipAddress = device.IpAddress,
                    staffDeviceId = staffDeviceId,
                    isMapped = staffDeviceId.HasValue,
                    location = mapping?.Location,
                    message = staffDeviceId.HasValue
                        ? $"Device registered and mapped to staff device (Location: {mapping?.Location ?? "N/A"})"
                        : "Device registered but not mapped - please configure mapping via admin panel",
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RegisterDevice]: Error registering device");
                throw new BadHttpRequestException($"Error registering device: {ex.Message}");
            }
        }

        /// <summary>
        /// Update ConnectionId for existing device
        /// </summary>
        [HttpPost("update-connection")]
        public async Task<IActionResult> UpdateConnection([FromBody] UpdateConnectionRequest request)
        {
            try
            {
#if DEBUG
                request.DeviceName = "Fake_Ipad_HOSTNAME";
#elif RELEASE
                request.DeviceName = await IpAddressHepler.GetClientComputerNameAsync(HttpContext) ?? "Fake_Ipad_HOSTNAME";                
#endif
                _logger.LogInformation("[UpdateConnection]: Updating ConnectionId for DeviceName: {DeviceName}", request.DeviceName);
                if (string.IsNullOrEmpty(request.DeviceName) || string.IsNullOrEmpty(request.ConnectionId))
                {
                    _logger.LogWarning("[UpdateConnection]: Missing DeviceName or ConnectionId in the request");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Device name and ConnectionId are required"
                    });
                }

                _logger.LogInformation("[UpdateConnection]: Updating device...");
                var device = await _deviceService.AddOrUpdatePatronDeviceAsync(
                    request.DeviceName,
                    request.ConnectionId,
                    request.MacAddress,
                    request.IpAddress
                );

                var data = new
                {
                    id = device.Id,
                    deviceName = device.DeviceName,
                    connectionId = device.ConnectionId,
                    isOnline = device.IsOnline,
                    message = "ConnectionId updated successfully",
                };

                _logger.LogInformation("[UpdateConnection]: ConnectionId updated successfully for Device ID: {DeviceId}", device.Id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdateConnection]: Error updating ConnectionId");
                throw new BadHttpRequestException($"Error updating ConnectionId: {ex.Message}");
            }
        }

        /// <summary>
        /// Get device by name
        /// </summary>
        [HttpGet("by-name/{deviceName}")]
        public async Task<IActionResult> GetDeviceByName(string deviceName)
        {
            try
            {
                var device = await _deviceService.GetDeviceByNameAsync(deviceName);

                if (device == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Device '{deviceName}' not found"
                    });
                }

                var data = new
                {
                    id = device.Id,
                    deviceName = device.DeviceName,
                    connectionId = device.ConnectionId,
                    isOnline = device.IsOnline,
                    isAvailable = device.IsAvailable,
                    macAddress = device.MacAddress,
                    ipAddress = device.IpAddress,
                    lastHeartbeat = device.LastHeartbeat
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device by name");
                throw new BadHttpRequestException($"Error getting device by name: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all online devices
        /// </summary>
        /// <summary>
        /// Get all online devices
        /// </summary>
        [HttpGet("online-devices")]
        public async Task<IActionResult> GetOnlineDevices()
        {
            try
            {
                // _logger.LogInformation("[GetOnlineDevices]: Getting online devices...");
                var devices = await _deviceService.GetOnlineDevicesAsync();

                // Get all active mappings
                var mappings = await _deviceService.GetAllActiveMappingsAsync();
                var mappingDict = mappings.ToDictionary(m => m.PatronDeviceId, m => m.StaffDeviceId);

                var result = devices.Select(d => new GetOnlineDevicesResponse
                {
                    Id = d.Id,
                    DeviceName = d.DeviceName,
                    ConnectionId = d.ConnectionId,
                    IsOnline = d.IsOnline,
                    IsAvailable = d.IsAvailable,
                    MacAddress = d.MacAddress!,
                    IpAddress = d.IpAddress!,
                    LastHeartbeat = d.LastHeartbeat,
                    StaffDeviceId = mappingDict.GetValueOrDefault(d.Id) // Get from mapping
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetOnlineDevices]: Error getting online devices");
                throw new BadHttpRequestException($"Error getting online devices: {ex.Message}");
            }
        }

        /// <summary>
        /// Set device offline
        /// </summary>
        [HttpPost("{deviceName}/offline")]
        public async Task<IActionResult> SetDeviceOffline(string deviceName)
        {
            try
            {
                var device = await _deviceService.GetDeviceByNameAsync(deviceName);

                if (device == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Device '{deviceName}' not found"
                    });
                }

                await _deviceService.SetDeviceOfflineAsync(device.ConnectionId);

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting device offline");
                throw new BadHttpRequestException($"Error setting device offline: {ex.Message}");
            }
        }

        [HttpGet("current-staff-device")]
        public async Task<IActionResult> GetCurrentStaffDevice()
        {
            try
            {
                var clientData = new ClientNameResponse();
                _logger.LogInformation("[GetCurrentStaffDevice]: Retrieving current staff device...");
                clientData.Ip = IpAddressHepler.GetClientIp(HttpContext)!;

#if DEBUG
                clientData.ComputerName = "Fake_PC_HOSTNAME" ?? "Fake_PC_HOSTNAME";
#elif RELEASE
                clientData.ComputerName = await IpAddressHepler.GetClientComputerNameAsync(HttpContext) ?? "Fake_PC_HOSTNAME";
#endif

                string employeeUserName = EmployeeHelper.GetCurrentUsername(HttpContext) ?? "Unknown_Employee";

                _logger.LogInformation("[GetCurrentStaffDevice]: Retrieved Client IP: {IP}, ComputerName: {ComputerName}, EmployeeUserName: {EmployeeUserName}",
                    clientData.Ip, clientData.ComputerName, employeeUserName);

                var staffDevice = await _deviceService.CreateOrUpdateStaffDevice(
                    clientData.ComputerName,
                    clientData.Ip!,
                    employeeUserName
                );

                _logger.LogInformation("[GetCurrentStaffDevice]: Staff device resolved - ID: {StaffDeviceId}, HostName: {HostName}, IP: {IP}",
                    staffDevice.Id, staffDevice.DeviceName, staffDevice.IpAddress);

                var data = new
                {
                    id = staffDevice.Id,
                    staffDeviceId = staffDevice.Id,
                    hostName = staffDevice.DeviceName,
                    location = staffDevice.StaffUserName,
                    assignedStaffId = staffDevice.IpAddress,
                    connectionId = staffDevice.ConnectionId,
                    deviceName = staffDevice.DeviceName
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetCurrentStaffDevice]: Error getting current staff device");
                throw new BadHttpRequestException($"Error getting current staff device: {ex.Message}");
            }
        }

        [HttpGet("online-staff-devices")]
        public async Task<IActionResult> GetOnlineStaffDevices()
        {
            try
            {
                // _logger.LogInformation("[GetOnlineStaffDevices]: Getting online staff devices...");
                var devices = await _deviceService.GetOnlineStaffDevicesAsync();
                var result = devices.Select(d => new
                {
                    id = d.Id,
                    deviceName = d.DeviceName,
                    connectionId = d.ConnectionId,
                    isOnline = d.IsOnline,
                    ipAddress = d.IpAddress,
                    staffUserName = d.StaffUserName
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetOnlineStaffDevices]: Error getting online staff devices");
                throw new BadHttpRequestException($"Error getting online staff devices: {ex.Message}");
            }
        }

        [HttpGet("get-infor")]
        public async Task<IActionResult> GetMappingPatronDeviceAsync()
        {
            try
            {
#if DEBUG
                var deviceName = "Fake_Ipad_HOSTNAME";
#elif RELEASE
                var deviceName = await IpAddressHepler.GetClientComputerNameAsync(HttpContext) ?? "Fake_Ipad_HOSTNAME";                
#endif
                var deviceMapping = await _deviceService.GetFullInformationMappingByPatronDeviceNameAsync(deviceName);

                if (deviceMapping?.Outlet == null)
                    throw new BadHttpRequestException($"Not found device mapping for '{deviceName}'");

                return Ok(deviceMapping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetPatronDeviceByName]: Error getting device by name");
                throw new BadHttpRequestException($"Error getting device by name: {ex.Message}");
            }
        }

        [HttpGet("client-name")]
        public async Task<IActionResult> GetClientName()
        {
            try
            {
                var clientData = new ClientNameResponse();
                _logger.LogInformation("[GetCurrentStaffDevice]: Retrieving current staff device...");
                clientData.Ip = IpAddressHepler.GetClientIp(HttpContext) ?? "Fake_IpAddress";
#if DEBUG
                clientData.ComputerName = "Fake_PC_HOSTNAME" ?? "Fake_PC_HOSTNAME";
#elif RELEASE
                clientData.ComputerName = await IpAddressHepler.GetClientComputerNameAsync(HttpContext) ?? "Fake_PC_HOSTNAME";
#endif
                _logger.LogInformation("[GetCurrentStaffDevice]: Retrieved Client IP: {IP}, ComputerName: {ComputerName}", clientData.Ip, clientData.ComputerName);
                return Ok(clientData);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetClientName]: Error occurred while retrieving client name: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }
    }
}