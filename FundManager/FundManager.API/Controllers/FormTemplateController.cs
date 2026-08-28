using DigitalDocumentPlatform.API.Filters;
using DigitalDocumentPlatform.API.Helpers;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalDocumentPlatform.API.Controllers
{
    [Route("api/form-template")]
    [ApiController]
    [Authorize]
    public class FormTemplateController : ControllerBase
    {
        private readonly ILogger<FormTemplateController> _logger;
        private readonly IFormTemplateService _formTemplateService;

        public FormTemplateController(ILogger<FormTemplateController> logger, IFormTemplateService formTemplateService)
        {
            _logger = logger;
            _formTemplateService = formTemplateService;
        }

        /// <summary>List all active form templates (brief, no questions).</summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                _logger.LogInformation("[FormTemplateController.GetList]: called");
                var result = await _formTemplateService.GetAllActiveAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.GetList]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>Get a single template including all questions and options.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                _logger.LogInformation("[FormTemplateController.GetDetail]: id={Id}", id);
                var result = await _formTemplateService.GetByIdAsync(id);
                if (result == null) return NotFound("Not Found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.GetDetail]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("create")]
        [AuditLog("FormTemplate", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateFormTemplateRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[FormTemplateController.Create]: title={Title}, by={User}", request.Title, currentUser.Name);
                var result = await _formTemplateService.CreateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.Create]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("update")]
        [AuditLog("FormTemplate", "Update")]
        public async Task<IActionResult> Update([FromBody] UpdateFormTemplateRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[FormTemplateController.Update]: id={Id}, by={User}", request.Id, currentUser.Name);
                var result = await _formTemplateService.UpdateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.Update]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("delete/{id:int}")]
        [AuditLog("FormTemplate", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[FormTemplateController.Delete]: id={Id}, by={User}", id, currentUser.Name);
                await _formTemplateService.DeleteAsync(id, currentUser.Name);
                return Ok(true);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.Delete]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        // ─── Question endpoints ──────────────────────────────────────────────────

        [HttpPost("question/add")]
        [AuditLog("FormTemplate", "AddQuestion")]
        public async Task<IActionResult> AddQuestion([FromBody] CreateFormQuestionRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[FormTemplateController.AddQuestion]: templateId={TemplateId}", request.FormTemplateId);
                var result = await _formTemplateService.AddQuestionAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.AddQuestion]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("question/update")]
        [AuditLog("FormTemplate", "UpdateQuestion")]
        public async Task<IActionResult> UpdateQuestion([FromBody] UpdateFormQuestionRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[FormTemplateController.UpdateQuestion]: id={Id}", request.Id);
                var result = await _formTemplateService.UpdateQuestionAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.UpdateQuestion]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("question/delete/{questionId:int}")]
        [AuditLog("FormTemplate", "DeleteQuestion")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[FormTemplateController.DeleteQuestion]: id={Id}", questionId);
                await _formTemplateService.DeleteQuestionAsync(questionId, currentUser.Name);
                return Ok(true);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.DeleteQuestion]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("question/reorder")]
        [AuditLog("FormTemplate", "ReorderQuestions")]
        public async Task<IActionResult> ReorderQuestions([FromBody] ReorderQuestionsRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[FormTemplateController.ReorderQuestions]: templateId={TemplateId}", request.FormTemplateId);
                await _formTemplateService.ReorderQuestionsAsync(request, currentUser.Name);
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.ReorderQuestions]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        // ─── Template Translations ──────────────────────────────────────────────────
        /// <summary>GET /api/customer-sign/form-templates/{id}/translations</summary>
        [HttpGet("{formTemplateId:int}/translations")]
        public async Task<IActionResult> GetFormTemplateTranslations(int formTemplateId)
        {
            try
            {
                var result = await _formTemplateService.GetFormTemplateTranslationsAsync(formTemplateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.GetFormTemplateTranslations]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>POST /api/form-template/translations — create or update translation</summary>
        [HttpPost("translations")]
        [AuditLog("FormTemplate", "UpsertTranslation")]
        public async Task<IActionResult> UpsertFormTemplateTranslation([FromBody] UpsertFormTemplateTranslationRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                var result = await _formTemplateService.UpsertFormTemplateTranslationAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.UpsertFormTemplateTranslation]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        // ─── Version Histories ──────────────────────────────────────────────────
        /// <summary>GET /api/customer-sign/form-templates/{id}/history</summary>
        [HttpGet("{formTemplateId:int}/history")]
        public async Task<IActionResult> GetFormTemplateHistory(int formTemplateId)
        {
            try
            {
                var result = await _formTemplateService.GetFormTemplateVersionHistoryAsync(formTemplateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FormTemplateController.GetFormTemplateHistory]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }
    }
}