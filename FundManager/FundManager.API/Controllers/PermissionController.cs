using FundManager.API.Filters;
using FundManager.Common.Constants;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FundManager.API.Controllers
{
    [Route("api/permission")]
    [ApiController]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                _logger.LogInformation("[GetAllPermissions]: Retrieving all permissions");
                var result = await _permissionService.GetAllPermissionsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetAllPermissions]: Error retrieving permissions");
                throw new BadHttpRequestException($"Error retrieving permissions: {ex.Message}");
            }
        }

        [HttpGet("by-category")]
        public async Task<IActionResult> GetPermissionsByCategory()
        {
            try
            {
                _logger.LogInformation("[GetPermissionsByCategory]: Retrieving permissions grouped by category");
                var result = await _permissionService.GetPermissionsByCategoryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetPermissionsByCategory]: Error retrieving permissions by category");
                throw new BadHttpRequestException($"Error retrieving permissions by category: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try
            {
                _logger.LogInformation("[GetPermissionById]: Retrieving permission {Id}", id);
                var result = await _permissionService.GetPermissionByIdAsync(id);

                if (result == null)
                {
                    _logger.LogWarning("[GetPermissionById]: Permission {Id} not found", id);
                    return NotFound(new { message = $"Permission with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetPermissionById]: Error retrieving permission {Id}", id);
                throw new BadHttpRequestException($"Error retrieving permission: {ex.Message}");
            }
        }

        [HttpPost("create")]
        [AuditLog("Permission", "Create")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? CommonConstants.UnknowUser;
                _logger.LogInformation("[CreatePermission]: Creating permission {Code} by {User}", request.PermissionCode, userName);

                var result = await _permissionService.CreatePermissionAsync(request, userName);
                _logger.LogInformation("[CreatePermission]: Permission created successfully with ID {Id}", result.Id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "[CreatePermission]: Validation error creating permission");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreatePermission]: Error creating permission");
                throw new BadHttpRequestException($"Error creating permission: {ex.Message}");
            }
        }

        [HttpPost("update/{id}")]
        [AuditLog("Permission", "Update")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new { message = "ID in URL does not match ID in request body" });
                }

                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? CommonConstants.UnknowUser;
                _logger.LogInformation("[UpdatePermission]: Updating permission {Id} by {User}", id, userName);

                var result = await _permissionService.UpdatePermissionAsync(request, userName);
                _logger.LogInformation("[UpdatePermission]: Permission {Id} updated successfully", id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "[UpdatePermission]: Validation error updating permission {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdatePermission]: Error updating permission {Id}", id);
                throw new BadHttpRequestException($"Error updating permission: {ex.Message}");
            }
        }

        [HttpPost("delete/{id}")]
        [AuditLog("Permission", "Delete")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? CommonConstants.UnknowUser;
                _logger.LogInformation("[DeletePermission]: Deleting permission {Id} by {User}", id, userName);

                var result = await _permissionService.DeletePermissionAsync(id, userName);
                _logger.LogInformation("[DeletePermission]: Permission {Id} deleted successfully", id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "[DeletePermission]: Validation error deleting permission {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DeletePermission]: Error deleting permission {Id}", id);
                throw new BadHttpRequestException($"Error deleting permission: {ex.Message}");
            }
        }

        [HttpPost("change-status/{id}")]
        [AuditLog("Permission", "ToggleActive")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? CommonConstants.UnknowUser;
                _logger.LogInformation("[ToggleActive]: Toggling active status for permission {Id} by {User}", id, userName);

                var isActive = await _permissionService.ToggleActiveAsync(id, userName);
                _logger.LogInformation("[ToggleActive]: Permission {Id} active status toggled to {IsActive}", id, isActive);
                return Ok(isActive);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "[ToggleActive]: Validation error toggling permission {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ToggleActive]: Error toggling permission {Id}", id);
                throw new BadHttpRequestException($"Error toggling permission: {ex.Message}");
            }
        }
    }
}