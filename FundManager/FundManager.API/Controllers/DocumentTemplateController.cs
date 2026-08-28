using FundManager.API.Filters;
using FundManager.API.Helpers;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [Route("api/document-template")]
    [ApiController]
    [Authorize]
    public class DocumentTemplateController : ControllerBase
    {
        private readonly ILogger<DocumentTemplateController> _logger;
        private readonly IDocumentTemplateService _documentTemplateService;

        public DocumentTemplateController(ILogger<DocumentTemplateController> logger, IDocumentTemplateService documentTemplateService)
        {
            _logger = logger;
            _documentTemplateService = documentTemplateService;
        }

        /// <summary>List all active document templates (brief).</summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                _logger.LogInformation("[DocumentTemplateController.GetList]: called");
                var result = await _documentTemplateService.GetListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.GetList]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>Get a single document template with full content.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                _logger.LogInformation("[DocumentTemplateController.GetDetail]: id={Id}", id);
                var result = await _documentTemplateService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.GetDetail]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>Get document templates by type.</summary>
        [HttpGet("by-type/{documentType}")]
        public async Task<IActionResult> GetByType(DocumentType documentType)
        {
            try
            {
                var result = await _documentTemplateService.GetByTypeAsync(documentType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.GetByType]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>Get document templates for a specific outlet (includes global templates).</summary>
        [HttpGet("by-outlet/{outletId:int}")]
        public async Task<IActionResult> GetByOutlet(int outletId)
        {
            try
            {
                var result = await _documentTemplateService.GetByOutletAsync(outletId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.GetByOutlet]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("create")]
        [AuditLog("DocumentTemplate", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateDocumentTemplateRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[DocumentTemplateController.Create]: title={Title}, by={User}", request.Title, currentUser.Name);
                var result = await _documentTemplateService.CreateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.Create]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("update")]
        [AuditLog("DocumentTemplate", "Update")]
        public async Task<IActionResult> Update([FromBody] UpdateDocumentTemplateRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[DocumentTemplateController.Update]: id={Id}, by={User}", request.Id, currentUser.Name);
                var result = await _documentTemplateService.UpdateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.Update]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("delete/{id:int}")]
        [AuditLog("DocumentTemplate", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[DocumentTemplateController.Delete]: id={Id}, by={User}", id, currentUser.Name);
                await _documentTemplateService.DeleteAsync(id, currentUser.Name);
                return Ok(true);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.Delete]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        // ─── Template Translations ──────────────────────────────────────────────────
        /// <summary>GET /api/customer-sign/document-templates/{id}/translations</summary>
        [HttpGet("{documentTemplateId:int}/translations")]
        public async Task<IActionResult> GetDocumentTemplateTranslations(int documentTemplateId)
        {
            try
            {
                var result = await _documentTemplateService.GetDocumentTemplateTranslationsAsync(documentTemplateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.GetDocumentTemplateTranslations]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>POST /api/customer-sign/document-templates/translations — create or update translation</summary>
        [HttpPost("translations")]
        public async Task<IActionResult> UpsertDocumentTemplateTranslation([FromBody] UpsertDocumentTemplateTranslationRequest request)
        {
            try
            {
                var user = User?.Identity?.Name ?? "admin";
                var result = await _documentTemplateService.UpsertDocumentTemplateTranslationAsync(request, user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.UpsertDocumentTemplateTranslation]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        // ─── Version Histories ──────────────────────────────────────────────────
        /// <summary>GET /api/customer-sign/document-templates/{id}/history</summary>
        [HttpGet("{documentTemplateId:int}/history")]
        public async Task<IActionResult> GetDocumentTemplateHistory(int documentTemplateId)
        {
            try
            {
                var result = await _documentTemplateService.GetDocumentTemplateVersionHistoryAsync(documentTemplateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DocumentTemplateController.GetDocumentTemplateHistory]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }
    }
}