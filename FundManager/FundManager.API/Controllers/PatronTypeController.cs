using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FundManager.API.Controllers
{
    [Route("api/patron-type")]
    [ApiController]
    [Authorize]
    public class PatronTypeController : ControllerBase
    {
        private readonly IPatronTypeService _patronTypeService;
        private readonly ILogger<PatronTypeController> _logger;

        public PatronTypeController(IPatronTypeService patronTypeService, ILogger<PatronTypeController> logger)
        {
            _patronTypeService = patronTypeService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _patronTypeService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PatronTypeController.GetAll] Error");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _patronTypeService.GetByIdAsync(id);
                if (result == null) return NotFound(new { message = $"PatronType {id} not found." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PatronTypeController.GetById] id={Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePatronTypeRequest request)
        {
            try
            {
                var createdBy = User.FindFirstValue(ClaimTypes.Name) ?? "system";
                var result = await _patronTypeService.CreateAsync(request, createdBy);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PatronTypeController.Create]");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdatePatronTypeRequest request)
        {
            try
            {
                var updatedBy = User.FindFirstValue(ClaimTypes.Name) ?? "system";
                var result = await _patronTypeService.UpdateAsync(request, updatedBy);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PatronTypeController.Update]");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deletedBy = User.FindFirstValue(ClaimTypes.Name) ?? "system";
                var result = await _patronTypeService.DeleteAsync(id, deletedBy);
                if (!result) return NotFound(new { message = $"PatronType {id} not found." });
                return Ok(new { message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PatronTypeController.Delete] id={Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var updatedBy = User.FindFirstValue(ClaimTypes.Name) ?? "system";
                var result = await _patronTypeService.ToggleActiveAsync(id, updatedBy);
                if (!result) return NotFound(new { message = $"PatronType {id} not found." });
                return Ok(new { message = "Status toggled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PatronTypeController.ToggleActive] id={Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}