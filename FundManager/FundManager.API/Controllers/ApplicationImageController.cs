using FundManager.API.Filters;
using FundManager.API.Helpers;
using FundManager.Common.Enum;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [Route("api/application-image")]
    [ApiController]
    [Authorize]
    public class ApplicationImageController : ControllerBase
    {
        private readonly ILogger<ApplicationImageController> _logger;
        private readonly IApplicationImageService _applicationImageService;

        public ApplicationImageController(ILogger<ApplicationImageController> logger, IApplicationImageService applicationImageService)
        {
            _logger = logger;
            _applicationImageService = applicationImageService;
        }

        [HttpGet("list")]
        //[AuditLog("ApplicationImage", "GetList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                _logger.LogInformation("[ApplicationImageController.GetList]: called");
                var result = await _applicationImageService.GetAllActiveAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ApplicationImageController.GetList]: {ErrorMessage}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("by-type/{type}")]
        //[AuditLog("ApplicationImage", "GetByType")]
        public async Task<IActionResult> GetByType(ImageTypeEnum type)
        {
            try
            {
                _logger.LogInformation("[ApplicationImageController.GetByType]: type={Type}", type);
                var result = await _applicationImageService.GetByTypeAsync(type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ApplicationImageController.GetByType]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        //[AuditLog("ApplicationImage", "GetDetail")]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                _logger.LogInformation("[ApplicationImageController.GetDetail]: id={Id}", id);
                var result = await _applicationImageService.GetByIdAsync(id);
                if (result == null)
                    return NotFound("Not Found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ApplicationImageController.GetDetail]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        [AuditLog("ApplicationImage", "Create")]
        public async Task<IActionResult> Create([FromForm] CreateApplicationImageRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[ApplicationImageController.Create]: name={Name}, type={Type}, by={User}", request.Name, request.Type, currentUser.Name);
                var result = await _applicationImageService.CreateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ApplicationImageController.Create]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("update")]
        [Consumes("multipart/form-data")]
        [AuditLog("ApplicationImage", "Update")]
        public async Task<IActionResult> Update([FromForm] UpdateApplicationImageRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[ApplicationImageController.Update]: id={Id}, by={User}", request.Id, currentUser.Name);
                var result = await _applicationImageService.UpdateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest("Some thing went wrong: " + ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest("Some thing went wrong: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ApplicationImageController.Update]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("delete/{id:int}")]
        [AuditLog("ApplicationImage", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[ApplicationImageController.Delete]: id={Id}, by={User}", id, currentUser.Name);
                await _applicationImageService.DeleteAsync(id, currentUser.Name);
                return Ok(true);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest("Some thing went wrong: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ApplicationImageController.Delete]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }
    }
}