using FundManager.API.Filters;
using FundManager.API.Helpers;
using FundManager.Common.Constants;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [Route("api/customer-sign")]
    [ApiController]
    public class CustomerSignController : ControllerBase
    {
        private readonly ILogger<CustomerSignController> _logger;
        private readonly ICustomerSignService _customerSignService;
        private readonly IPatronDeviceService _patronDeviceService;
        private readonly ISignalRService _signalRService;

        public CustomerSignController(
            ILogger<CustomerSignController> logger,
            ICustomerSignService customerSignService,
            IPatronDeviceService patronDeviceService,
            ISignalRService signalRService)
        {
            _logger = logger;
            _customerSignService = customerSignService;
            _patronDeviceService = patronDeviceService;
            _signalRService = signalRService;
        }

        /// <summary>
        /// GET /api/customer-sign/form-template/{id}
        /// Returns the full form template (title, description, all questions + options)
        /// that the patron needs to fill in during the FillForm workflow step.
        /// </summary>
        [HttpGet("form-template/{id:int}")]
        public async Task<IActionResult> GetFormTemplate(int id, [FromQuery] string? language)
        {
            if (string.IsNullOrEmpty(language))
            {
                _logger.LogWarning("[CustomerSignController.GetFormTemplate]: No language specified, defaulting to 'en'");
                language = CommonConstants.DefaultLanguage;
            }

            try
            {
                _logger.LogInformation("[CustomerSignController.GetFormTemplate]: id={Id}, language={Language}", id, language);
                var result = await _customerSignService.GetFormTemplateAsync(id, language);
                if (result == null) return NotFound($"FormTemplate {id} not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CustomerSignController.GetFormTemplate]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>
        /// GET /api/customer-sign/document-template/{id}
        /// Returns the document template HTML content for the patron to read
        /// before acknowledging / signing (Acknowledgement workflow step).
        /// </summary>
        [HttpGet("document-template/{id:int}")]
        public async Task<IActionResult> GetDocumentTemplate(int id, [FromQuery] string? language)
        {
            if (string.IsNullOrEmpty(language))
            {
                _logger.LogWarning("[CustomerSignController.GetDocumentTemplate]: No language specified, defaulting to 'en'");
                language = CommonConstants.DefaultLanguage;
            }

            try
            {
                _logger.LogInformation("[CustomerSignController.GetDocumentTemplate]: id={Id}, language={Language}", id, language);
                var result = await _customerSignService.GetDocumentTemplateAsync(id, language);
                if (result == null) return NotFound($"DocumentTemplate {id} not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CustomerSignController.GetDocumentTemplate]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>
        /// POST /api/customer-sign/submit
        /// Full spa session submit:
        ///   - Saves Patron record
        ///   - Saves FormSubmission + FormSubmissionAnswers
        ///   - Generates ConsultationForm PDF  → PatronSignature (DocumentType=ConsultationForm)
        ///   - Generates PDP document PDF      → PatronSignature (DocumentType=PdpForm)
        /// </summary>
        [HttpPost("submit")]
        [AuditLog("CustomerSign", "SubmitSignature")]
        public async Task<IActionResult> SubmitSignature([FromBody] CustomerSessionSubmitRequest request)
        {
            if (string.IsNullOrEmpty(request.PatronDeviceName))
            {
                _logger.LogWarning("[CustomerSignController.SubmitSignature]: No PatronDeviceName specified");
#if DEBUG
                request.PatronDeviceName = "Fake_Ipad_HOSTNAME";
#elif RELEASE
                request.PatronDeviceName = await IpAddressHepler.GetClientComputerNameAsync(HttpContext) ?? "Fake_Ipad_HOSTNAME";
#endif
            }

            try
            {
                _logger.LogInformation("[CustomerSignController.SubmitSignature]: customerType={CustomerType}", request.CustomerType);
                var result = await _customerSignService.SubmitSignatureSessionAsync(request);
                if (result.Success && request.SessionId.HasValue)
                {
                    _logger.LogInformation("[CustomerSignController.SubmitSignature]: Submission successful for PatronId={PatronId}", result.PatronId);
                    await _patronDeviceService.CompleteSignatureSessionAsync(request.SessionId.Value, string.Empty);
                }
                else
                {
                    _logger.LogWarning("[CustomerSignController.SubmitSignature]: Submission failed for PatronId={PatronId}, Message={Message}", result.PatronId, result.Message);
                }
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CustomerSignController.SubmitSignature]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // ADMIN: Signed customers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>GET /api/customer-sign/admin/signed-customers — server-paged list with filters</summary>
        [HttpGet("admin/signed-customers")]
        public async Task<IActionResult> GetSignedCustomers([FromQuery] SignedCustomerListRequest request)
        {
            try
            {
                var result = await _customerSignService.GetSignedCustomersAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CustomerSignController.GetSignedCustomers]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>GET /api/customer-sign/admin/signed-customers/{patronId} — detail of one patron + documents</summary>
        [HttpGet("admin/signed-customers/{patronId:int}")]
        public async Task<IActionResult> GetSignedCustomerDetail(int patronId)
        {
            try
            {
                var result = await _customerSignService.GetSignedCustomerDetailAsync(patronId);
                if (result == null) return NotFound($"Patron {patronId} not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CustomerSignController.GetSignedCustomerDetail]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>
        /// GET /api/customer-sign/admin/session-prefill/{patronId}
        /// Returns the last known patron info + form answers so the iPad can pre-fill the form
        /// (called after RequestDocumentSignature sends the SignalR duplicate-request notification).
        /// </summary>
        [HttpGet("admin/session-prefill/{patronId:int}")]
        public async Task<IActionResult> GetSessionPrefill(int patronId, [FromQuery] string language)
        {
            try
            {
                var result = await _customerSignService.GetSessionPrefillAsync(patronId, language);
                if (result == null) return NotFound($"Patron {patronId} not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CustomerSignController.GetSessionPrefill]: {Message}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────────────

        [HttpPost("request")]
        [AuditLog("CustomerSign", "RequestSignature")]
        public async Task<IActionResult> RequestDocumentSignatureAsync(StaffSignatureRequest staffRequest)
        {
            try
            {
                var clientData = new ClientNameResponse();
                _logger.LogInformation("[RequestDocumentSignatureAsync]: Retrieving current staff device...");
                clientData.Ip = IpAddressHepler.GetClientIp(HttpContext) ?? "Fake_IpAddress";
                clientData.ComputerName = await IpAddressHepler.GetClientComputerNameAsync(HttpContext) ?? "Fake_PC_HOSTNAME";
                _logger.LogInformation("[RequestDocumentSignatureAsync]: Retrieved Client IP: {IP}, ComputerName: {ComputerName}", clientData.Ip, clientData.ComputerName);

                var staffDevice = await _patronDeviceService.GetStaffDeviceByHostNameAsync(clientData.ComputerName);

                _logger.LogInformation(
                    "[RequestDocumentSignatureAsync] Sending SignalR notification - PatronId: {PatronId}, StaffRequest: {StaffRequest}",
                    staffRequest?.PatronId, clientData.ComputerName);

                if (staffDevice != null)
                {
                    await _signalRService.SendSignatureRequestToDeviceAsync(staffRequest!.PatronId, staffDevice.Id);

                    _logger.LogInformation(
                        "[RequestDocumentSignatureAsync] SignalR sent successfully - PatronId: {PatronId}, StaffDeviceId: {StaffDeviceId}",
                        staffRequest!.PatronId, staffDevice.Id);
                }
                else
                {
                    _logger.LogWarning(
                        "[RequestDocumentSignatureAsync] No StaffDeviceId in header - PatronId: {PatronId}",
                        staffRequest!.PatronId);
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RequestDocumentSignatureAsync]: ❌ Error initiating signature request");
                throw new BadHttpRequestException(ex.Message);
            }
        }
    }
}