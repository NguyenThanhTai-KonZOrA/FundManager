using DigitalDocumentPlatform.API.Filters;
using DigitalDocumentPlatform.API.Helpers;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalDocumentPlatform.API.Controllers
{
    [Route("api/form-submission")]
    [ApiController]
    [Authorize]
    public class FormSubmissionController : ControllerBase
    {
        private readonly ILogger<FormSubmissionController> _logger;
        private readonly IFormSubmissionService _formSubmissionService;

        public FormSubmissionController(ILogger<FormSubmissionController> logger, IFormSubmissionService formSubmissionService)
        {
            _logger = logger;
            _formSubmissionService = formSubmissionService;
        }

        /// <summary>Submit a completed form. Returns the saved submission with answers.</summary>
        [HttpPost("submit")]
        [AuditLog("FormSubmission", "Submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitFormRequest request)
        {
            try
            {
                // Validation
                if (request == null)
                    return BadRequest("Request body cannot be null");

                if (request.FormTemplateId <= 0)
                    return BadRequest("Invalid FormTemplateId");

                if (request.Answers == null || !request.Answers.Any())
                    return BadRequest("Form submission must contain at least one answer");

                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[FormSubmissionController.Submit]: templateId={TemplateId}, answersCount={Count}", 
                    request.FormTemplateId, request.Answers.Count);
                var result = await _formSubmissionService.SubmitAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "[FormSubmissionController.Submit]: Template not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormSubmissionController.Submit]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>Get one submission with all answers (for review).</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid submission ID");

                _logger.LogInformation("[FormSubmissionController.GetDetail]: id={Id}", id);
                var result = await _formSubmissionService.GetByIdAsync(id);
                if (result == null) 
                {
                    _logger.LogWarning("[FormSubmissionController.GetDetail]: Submission {Id} not found", id);
                    return NotFound($"Submission with ID {id} not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormSubmissionController.GetDetail]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>Get submission history for a PatronDevice (all versions).</summary>
        [HttpGet("patron-device/{patronDeviceId:int}")]
        public async Task<IActionResult> GetByPatronDevice(int patronDeviceId)
        {
            try
            {
                if (patronDeviceId <= 0)
                    return BadRequest("Invalid patron device ID");

                _logger.LogInformation("[FormSubmissionController.GetByPatronDevice]: patronDeviceId={PatronDeviceId}", patronDeviceId);
                var result = await _formSubmissionService.GetByPatronDeviceIdAsync(patronDeviceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormSubmissionController.GetByPatronDevice]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>Get all submissions for a template (admin review).</summary>
        [HttpGet("by-template/{templateId:int}")]
        public async Task<IActionResult> GetByTemplate(int templateId)
        {
            try
            {
                if (templateId <= 0)
                    return BadRequest("Invalid template ID");

                _logger.LogInformation("[FormSubmissionController.GetByTemplate]: templateId={TemplateId}", templateId);
                var result = await _formSubmissionService.GetByTemplateIdAsync(templateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormSubmissionController.GetByTemplate]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }
    }
}