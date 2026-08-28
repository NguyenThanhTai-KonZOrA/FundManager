using DigitalDocumentPlatform.API.Filters;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalDocumentPlatform.API.Controllers
{
    [Route("api/language")]
    [ApiController]
    [Authorize]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageService _languageService;
        private readonly ILogger<LanguageController> _logger;

        public LanguageController(ILanguageService languageService, ILogger<LanguageController> logger)
        {
            _languageService = languageService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _languageService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LanguageController.GetAll] Error");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _languageService.GetByIdAsync(id);
                if (result == null) return NotFound(new { message = $"Language {id} not found." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LanguageController.GetById] id={Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateLanguageRequest request)
        {
            try
            {
                var createdBy = User.FindFirstValue(ClaimTypes.Name) ?? "system";
                var result = await _languageService.CreateAsync(request, createdBy);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LanguageController.Create]");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("update")]
        [AuditLog("Language", "Update")]
        public async Task<IActionResult> Update([FromBody] UpdateLanguageRequest request)
        {
            try
            {
                var updatedBy = User.FindFirstValue(ClaimTypes.Name) ?? "system";
                var result = await _languageService.UpdateAsync(request, updatedBy);
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
                _logger.LogError(ex, "[LanguageController.Update]");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("delete/{id}")]
        [AuditLog("Language", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deletedBy = User.FindFirstValue(ClaimTypes.Name) ?? "system";
                var result = await _languageService.DeleteAsync(id, deletedBy);
                if (!result) return NotFound(new { message = $"Language {id} not found." });
                return Ok(new { message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LanguageController.Delete] id={Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("{id}/toggle-active")]
        [AuditLog("Language", "ToggleActive")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var updatedBy = User.FindFirstValue(ClaimTypes.Name) ?? "system";
                var result = await _languageService.ToggleActiveAsync(id, updatedBy);
                if (!result) return NotFound(new { message = $"Language {id} not found." });
                return Ok(new { message = "Status toggled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LanguageController.ToggleActive] id={Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}