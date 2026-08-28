using FundManager.API.Filters;
using FundManager.API.Helpers;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [Route("api/outlet")]
    [ApiController]
    [Authorize]
    public class OutletController : ControllerBase
    {
        private readonly ILogger<OutletController> _logger;
        private readonly IOutletService _outletService;

        public OutletController(ILogger<OutletController> logger, IOutletService outletService)
        {
            _logger = logger;
            _outletService = outletService;
        }

        [AllowAnonymous]
        [HttpGet("list")]
        //[AuditLog("Outlet", "GetList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                _logger.LogInformation("[OutletController.GetList]: called");
                var result = await _outletService.GetAllActiveAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OutletController.GetList]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpGet("by-property/{propertyId:int}")]
        //[AuditLog("Outlet", "GetByProperty")]
        public async Task<IActionResult> GetByProperty(int propertyId)
        {
            try
            {
                _logger.LogInformation("[OutletController.GetByProperty]: propertyId={PropertyId}", propertyId);
                var result = await _outletService.GetByPropertyIdAsync(propertyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OutletController.GetByProperty]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        //[AuditLog("Outlet", "GetDetail")]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                _logger.LogInformation("[OutletController.GetDetail]: id={Id}", id);
                var result = await _outletService.GetByIdAsync(id);
                if (result == null)
                    return NotFound("Not Found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OutletController.GetDetail]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("create")]
        [AuditLog("Outlet", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateOutletRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[OutletController.Create]: name={Name}, by={User}", request.Name, currentUser.Name);
                var result = await _outletService.CreateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OutletController.Create]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("update")]
        [AuditLog("Outlet", "Update")]
        public async Task<IActionResult> Update([FromBody] UpdateOutletRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[OutletController.Update]: id={Id}, by={User}", request.Id, currentUser.Name);
                var result = await _outletService.UpdateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OutletController.Update]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("delete/{id:int}")]
        [AuditLog("Outlet", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[OutletController.Delete]: id={Id}, by={User}", id, currentUser.Name);
                await _outletService.DeleteAsync(id, currentUser.Name);
                return Ok(true);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OutletController.Delete]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpGet("{id:int}/staff-devices")]
        public async Task<IActionResult> GetStaffDevices(int id)
        {
            try
            {
                _logger.LogInformation("[OutletController.GetStaffDevices]: outletId={OutletId}", id);
                var result = await _outletService.GetStaffDevicesByOutletAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OutletController.GetStaffDevices]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("{id:int}/staff-devices/assign")]
        [AuditLog("Outlet", "AssignStaffDevice")]
        public async Task<IActionResult> AssignStaffDevice(int id, [FromBody] AssignStaffDeviceToOutletRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[OutletController.AssignStaffDevice]: outletId={OutletId}, staffDeviceId={StaffDeviceId}", id, request.StaffDeviceId);
                var result = await _outletService.AssignStaffDeviceAsync(id, request.StaffDeviceId, currentUser.Name);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OutletController.AssignStaffDevice]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("{id:int}/staff-devices/{staffDeviceId:int}/unassign")]
        [AuditLog("Outlet", "UnassignStaffDevice")]
        public async Task<IActionResult> UnassignStaffDevice(int id, int staffDeviceId)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[OutletController.UnassignStaffDevice]: outletId={OutletId}, staffDeviceId={StaffDeviceId}", id, staffDeviceId);
                var result = await _outletService.UnassignStaffDeviceAsync(id, staffDeviceId, currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OutletController.UnassignStaffDevice]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }
    }
}