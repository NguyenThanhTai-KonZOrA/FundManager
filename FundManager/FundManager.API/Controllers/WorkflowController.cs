using FundManager.API.Filters;
using FundManager.API.Helpers;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [Route("api/workflow")]
    [ApiController]
    [Authorize]
    public class WorkflowController : ControllerBase
    {
        private readonly ILogger<WorkflowController> _logger;
        private readonly IWorkflowService _workflowService;

        public WorkflowController(ILogger<WorkflowController> logger, IWorkflowService workflowService)
        {
            _logger = logger;
            _workflowService = workflowService;
        }

        /// <summary>List all workflows with their steps.</summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                _logger.LogInformation("[WorkflowController.GetList]: called");
                var result = await _workflowService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WorkflowController.GetList]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>Get workflow detail by id.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                _logger.LogInformation("[WorkflowController.GetDetail]: id={Id}", id);
                var result = await _workflowService.GetByIdAsync(id);
                if (result == null) return NotFound("Not Found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WorkflowController.GetDetail]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        /// <summary>Get the active workflow for a given outlet (used at runtime by patron device).</summary>
        [HttpGet("by-outlet/{outletId:int}")]
        public async Task<IActionResult> GetByOutlet(int outletId)
        {
            try
            {
                _logger.LogInformation("[WorkflowController.GetByOutlet]: outletId={OutletId}", outletId);
                var result = await _workflowService.GetByOutletIdAsync(outletId);
                if (result == null) return NotFound("No active workflow for this outlet");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WorkflowController.GetByOutlet]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("create")]
        [AuditLog("Workflow", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateWorkflowRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[WorkflowController.Create]: name={Name}, outletId={OutletId}", request.Name, request.OutletId);
                var result = await _workflowService.CreateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WorkflowController.Create]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("update")]
        [AuditLog("Workflow", "Update")]
        public async Task<IActionResult> Update([FromBody] UpdateWorkflowRequest request)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[WorkflowController.Update]: id={Id}", request.Id);
                var result = await _workflowService.UpdateAsync(request, currentUser.Name);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WorkflowController.Update]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }

        [HttpPost("delete/{id:int}")]
        [AuditLog("Workflow", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUser = EmployeeHelper.CurrentEmployee(HttpContext);
                _logger.LogInformation("[WorkflowController.Delete]: id={Id}", id);
                await _workflowService.DeleteAsync(id, currentUser.Name);
                return Ok(true);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WorkflowController.Delete]: {ErrorMessage}", ex.Message);
                throw new BadHttpRequestException(ex.Message);
            }
        }
    }
}