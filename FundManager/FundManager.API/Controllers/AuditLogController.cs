using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [Route("api/audit-logs")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(IAuditLogService auditLogService, ILogger<AuditLogController> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated logs with multiple filter conditions
        /// Supports filtering by: UserName, Action, EntityType, IsSuccess, FromDate, ToDate
        /// </summary>
        [HttpPost("paginate")]
        public async Task<IActionResult> GetPaginatedLogs([FromBody] AuditLogPaginationRequest request)
        {
            try
            {
                _logger.LogInformation("[GetPaginatedLogs] START - Request: {@Request}", request);
                var result = await _auditLogService.GetPaginatedLogsAsync(request);
                _logger.LogInformation("[GetPaginatedLogs] END - Retrieved {Count} logs", result.Logs.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetPaginatedLogs] FAILED");
                throw new BadHttpRequestException($"Error retrieving paginated logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Write a new audit log entry
        /// </summary>
        [HttpPost("log")]
        public async Task<IActionResult> LogAction([FromBody] CreateAuditLogRequest request)
        {
            try
            {
                _logger.LogInformation("[LogAction] START - Action: {Action}, User: {UserName}", request.Action, request.UserName);
                await _auditLogService.LogActionAsync(request);
                _logger.LogInformation("[LogAction] END - Successfully logged action");
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LogAction] FAILED");
                throw new BadHttpRequestException($"Error logging action: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a specific audit log by ID
        /// </summary>
        [HttpGet("{auditLogId}")]
        public async Task<IActionResult> GetLogById(int auditLogId)
        {
            try
            {
                _logger.LogInformation("[GetLogById] START - ID: {Id}", auditLogId);
                var log = await _auditLogService.GetByIdAsync(auditLogId);

                if (log == null)
                {
                    _logger.LogWarning("[GetLogById] NOT FOUND - ID: {Id}", auditLogId);
                    throw new BadHttpRequestException("Audit log not found");
                }

                _logger.LogInformation("[GetLogById] END - Found log ID: {Id}", auditLogId);
                return Ok(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetLogById] FAILED - ID: {Id}", auditLogId);
                throw new BadHttpRequestException($"Error retrieving audit log by ID: {ex.Message}");
            }
        }

        #region Legacy APIs (kept for backward compatibility)

        [HttpGet("all")]
        public async Task<IActionResult> GetAllLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _auditLogService.GetAllAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetAllLogs] FAILED");
                throw new BadHttpRequestException($"Error retrieving all logs: {ex.Message}");
            }
        }

        [HttpGet("by-user/{userName}")]
        public async Task<IActionResult> GetLogsByUser(string userName, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _auditLogService.GetByUserNameAsync(userName, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetLogsByUser] FAILED");
                throw new BadHttpRequestException($"Error retrieving logs by user: {ex.Message}");
            }
        }

        [HttpGet("by-action/{action}")]
        public async Task<IActionResult> GetLogsByAction(string action, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _auditLogService.GetByActionAsync(action, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetLogsByAction] FAILED");
                throw new BadHttpRequestException($"Error retrieving logs by action: {ex.Message}");
            }
        }

        [HttpGet("failed")]
        public async Task<IActionResult> GetFailedLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _auditLogService.GetFailedLogsAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetFailedLogs] FAILED");
                throw new BadHttpRequestException($"Error retrieving failed logs: {ex.Message}");
            }
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetLogsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _auditLogService.GetByDateRangeAsync(startDate, endDate, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetLogsByDateRange] FAILED");
                throw new BadHttpRequestException($"Error retrieving logs by date range: {ex.Message}");
            }
        }

        #endregion
    }
}