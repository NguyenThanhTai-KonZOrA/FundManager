using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;

namespace DigitalDocumentPlatform.API.Controllers
{
    [Route("api/manage-device")]
    [ApiController]
    public class ManageDeviceController : ControllerBase
    {
        private readonly IPatronDeviceService _deviceService;
        private readonly ILogger<ManageDeviceController> _logger;

        public ManageDeviceController(
            IPatronDeviceService deviceService,
            ILogger<ManageDeviceController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        /// <summary>
        /// 1. Get all devices (both patron and staff devices)
        /// </summary>
        [HttpGet("all-devices")]
        public async Task<IActionResult> GetAllDevices()
        {
            try
            {
                _logger.LogInformation("[GetAllDevices]: Retrieving all devices...");

                var devices = await _deviceService.GetAllStaffAndPatronDevicesAsync();

                _logger.LogInformation("[GetAllDevices]: Retrieved {StaffCount} staff devices and {PatronCount} patron devices",
                    devices.StaffDevices.Count, devices.PatronDevices.Count);

                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetAllDevices]: Error retrieving devices");
                throw new BadHttpRequestException($"Error retrieving devices: {ex.Message}");
            }
        }

        /// <summary>
        /// 2. Activate or Deactivate a device (works for both staff and patron devices)
        /// </summary>
        [HttpPost("toggle-active")]
        public async Task<IActionResult> ToggleDeviceActive([FromBody] ToggleDeviceRequest request)
        {
            try
            {
                if (request.DeviceId <= 0 || string.IsNullOrEmpty(request.DeviceType))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "DeviceId and DeviceType are required"
                    });
                }

                _logger.LogInformation("[ToggleDeviceActive]: DeviceId={DeviceId}, DeviceType={DeviceType}, IsActive={IsActive}",
                    request.DeviceId, request.DeviceType, request.IsActive);

                var result = await _deviceService.ToggleDeviceActiveAsync(request.DeviceId, request.DeviceType, request.IsActive);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ToggleDeviceActive]: Error toggling device active status");
                throw new BadHttpRequestException($"Error toggling device active status: {ex.Message}");
            }
        }

        /// <summary>
        /// 3. Delete a device (soft delete)
        /// </summary>
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteDevice([FromBody] DeleteDeviceRequest request)
        {
            try
            {
                if (request.DeviceId <= 0 || string.IsNullOrEmpty(request.DeviceType))
                {
                    throw new BadHttpRequestException($"DeviceId and DeviceType are required. Received DeviceId={request.DeviceId}, DeviceType={request.DeviceType}");
                }

                _logger.LogInformation("[DeleteDevice]: DeviceId={DeviceId}, DeviceType={DeviceType}",
                    request.DeviceId, request.DeviceType);

                var deleted = await _deviceService.DeleteDeviceAsync(request.DeviceId, request.DeviceType);

                if (!deleted)
                {
                    throw new BadHttpRequestException($"Failed to delete device. DeviceId={request.DeviceId}, DeviceType={request.DeviceType}");
                }

                return Ok(new { success = true, message = "Device deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DeleteDevice]: Error deleting device");
                throw new BadHttpRequestException($"Error deleting device: {ex.Message}");
            }
        }


        /// <summary>
        /// 4. Change device hostname
        /// </summary>
        [HttpPost("change-hostname")]
        public async Task<IActionResult> ChangeDeviceHostname([FromBody] ChangeHostnameRequest request)
        {
            try
            {
                if (request.DeviceId <= 0 || string.IsNullOrEmpty(request.DeviceType) || string.IsNullOrWhiteSpace(request.NewHostname))
                {
                    throw new BadHttpRequestException($"DeviceId, DeviceType, and NewHostname are required. Received DeviceId={request.DeviceId}, DeviceType={request.DeviceType}, NewHostname={request.NewHostname}");
                }

                _logger.LogInformation("[ChangeDeviceHostname]: DeviceId={DeviceId}, DeviceType={DeviceType}, NewHostname={NewHostname}",
                    request.DeviceId, request.DeviceType, request.NewHostname);

                var result = await _deviceService.ChangeDeviceHostnameAsync(request.DeviceId, request.DeviceType, request.NewHostname);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChangeDeviceHostname]: Error changing device hostname");
                throw new BadHttpRequestException($"Error changing device hostname: {ex.Message}");
            }
        }
    }
}