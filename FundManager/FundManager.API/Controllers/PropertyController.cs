using FundManager.API.Filters;
using FundManager.API.Helpers;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [Route("api/property")]
    [ApiController]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly ILogger<PropertyController> _logger;
        private readonly IPropertyService _propertyService;

        public PropertyController(ILogger<PropertyController> logger, IPropertyService propertyService)
        {
            _logger = logger;
            _propertyService = propertyService;
        }

        [HttpGet("list")]
        //[AuditLog("Property", "GetList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                _logger.LogInformation("[PropertyController.GetList]: called");
                var result = await _propertyService.GetActivePropertiesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PropertyController.GetList]: {ErrorMessage}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        //[AuditLog("Property", "GetDetail")]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                _logger.LogInformation("[PropertyController.GetDetail]: id={Id}", id);
                var result = await _propertyService.GetPropertyByIdAsync(id);
                if (result == null)
                    throw new BadHttpRequestException("Not Found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PropertyController.GetDetail]: {ErrorMessage}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("create")]
        [AuditLog("Property", "Create")]
        public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[PropertyController.Create]: name={Name}, by={User}", request.Name, currentUser.Name);
                var result = await _propertyService.CreatePropertyAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PropertyController.Create]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("update")]
        [AuditLog("Property", "Update")]
        public async Task<IActionResult> Update([FromBody] UpdatePropertyRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[PropertyController.Update]: id={Id}, by={User}", request.Id, currentUser.Name);
                var result = await _propertyService.UpdatePropertyAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PropertyController.Update]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("delete/{id:int}")]
        [AuditLog("Property", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[PropertyController.Delete]: id={Id}, by={User}", id, currentUser.Name);
                await _propertyService.DeletePropertyAsync(id, currentUser.Name);
                return Ok(true);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PropertyController.Delete]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }
    }
}