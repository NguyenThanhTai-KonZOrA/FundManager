using FundManager.API.Filters;
using FundManager.API.Helpers;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [ApiController]
    [Route("api/device-mapping")]
    public class DeviceMappingController : ControllerBase
    {
        private readonly IPatronDeviceService _deviceService;
        private readonly ILogger<DeviceMappingController> _logger;

        public DeviceMappingController(IPatronDeviceService deviceService, ILogger<DeviceMappingController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        /// <summary>
        /// Create or update device mapping (PC <-> iPad)
        /// </summary>
        [HttpPost("create")]
        [AuditLog("DeviceMapping", "Create")]
        public async Task<IActionResult> CreateMapping([FromBody] CreateMappingRequest request)
        {
            if (string.IsNullOrEmpty(request.StaffDeviceName) || string.IsNullOrEmpty(request.PatronDeviceName))
            {
                _logger.LogWarning("[CreateMapping]: Both StaffDeviceName and PatronDeviceName are required");
                throw new BadHttpRequestException("Both StaffDeviceName and PatronDeviceName are required");
            }

            try
            {
                var mapping = await _deviceService.CreateOrUpdateMappingAsync(request);

                return Ok(mapping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateMapping]: Error creating mapping");
                throw new BadHttpRequestException($"Error creating mapping: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all active mappings
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetAllMappings()
        {
            try
            {
                var mappings = await _deviceService.GetAllActiveMappingsAsync();
                return Ok(mappings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetAllMappings]: Error getting mappings");
                throw new BadHttpRequestException($"Error getting mappings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get mapping by staff device name
        /// </summary>
        [HttpGet("by-staff/{staffDeviceName}")]
        public async Task<IActionResult> GetMappingByStaffDevice(string staffDeviceName)
        {
#if DEBUG
            staffDeviceName = "Fake_PC_HOSTNAME";
#elif RELEASE
            staffDeviceName = await IpAddressHepler.GetClientComputerNameAsync(HttpContext) ?? "Fake_PC_HOSTNAME";
#endif
            try
            {
                var mapping = await _deviceService.GetMappingByStaffDeviceNameAsync(staffDeviceName);
                if (mapping == null)
                {
                    _logger.LogWarning("[GetMappingByStaffDevice]: No mapping found for staff device '{StaffDeviceName}'", staffDeviceName);
                    throw new BadHttpRequestException($"No mapping found for staff device '{staffDeviceName}'");
                }

                var data = new
                {
                    id = mapping.Id,
                    staffDeviceName = mapping.StaffDevice.DeviceName,
                    patronDeviceName = mapping.PatronDevice.DeviceName,
                    patronDeviceId = mapping.PatronDeviceId,
                    location = mapping.Location,
                    isActive = mapping.IsActive,
                    lastVerified = mapping.LastVerified
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetMappingByStaffDevice]: Error getting mapping");
                throw new BadHttpRequestException($"Error getting mapping: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete (deactivate) mapping
        /// </summary>
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteMapping(int id)
        {
            try
            {
                var result = await _deviceService.DeleteMappingAsync(id);

                if (!result)
                    return NotFound(new { success = false, message = "Mapping not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DeleteMapping]: Error deleting mapping");
                throw new BadHttpRequestException($"Error deleting mapping: {ex.Message}");
            }
        }

        /// <summary>
        /// Update existing device mapping
        /// </summary>
        [HttpPost("update")]
        [AuditLog("DeviceMapping", "Update")]
        public async Task<IActionResult> UpdateMapping([FromBody] UpdateMappingRequest request)
        {
            try
            {
                var mapping = await _deviceService.UpdateMappingAsync(request);
                return Ok(mapping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdateMapping]: Error updating mapping ID: {MappingId}", request.Id);
                throw new BadHttpRequestException($"Error updating mapping: {ex.Message}");
            }
        }

        [HttpGet("staff-and-patron-devices")]
        public async Task<IActionResult> GetAllStaffAndPatronDevices()
        {
            try
            {
                var devices = await _deviceService.GetAllStaffAndPatronDevicesSummaryAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetAllStaffDevices]: Error getting staff devices");
                throw new BadHttpRequestException($"Error getting staff devices: {ex.Message}");
            }
        }
    }
}