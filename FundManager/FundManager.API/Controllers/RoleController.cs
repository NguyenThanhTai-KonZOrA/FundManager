using DigitalDocumentPlatform.API.Filters;
using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalDocumentPlatform.API.Controllers
{
    [Route("api/role")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                _logger.LogInformation("[GetAllRoles]: Retrieving all roles");
                var result = await _roleService.GetAllRolesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetAllRoles]: Error retrieving roles");
                throw new BadHttpRequestException($"Error retrieving roles: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                _logger.LogInformation("[GetRoleById]: Retrieving role {Id}", id);
                var result = await _roleService.GetRoleByIdAsync(id);

                if (result == null)
                {
                    _logger.LogWarning("[GetRoleById]: Role {Id} not found", id);
                    return NotFound(new { message = $"Role with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetRoleById]: Error retrieving role {Id}", id);
                throw new BadHttpRequestException($"Error retrieving role: {ex.Message}");
            }
        }

        [HttpPost("create")]
        [AuditLog("Role", "Create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? CommonConstants.UnknowUser;
                _logger.LogInformation("[CreateRole]: Creating role {Name} by {User}", request.RoleName, userName);

                var result = await _roleService.CreateRoleAsync(request, userName);
                _logger.LogInformation("[CreateRole]: Role created successfully with ID {Id}", result.Id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "[CreateRole]: Validation error creating role");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateRole]: Error creating role");
                throw new BadHttpRequestException($"Error creating role: {ex.Message}");
            }
        }

        [HttpPost("update/{id}")]
        [AuditLog("Role", "Update")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new { message = "ID in URL does not match ID in request body" });
                }

                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? CommonConstants.UnknowUser;
                _logger.LogInformation("[UpdateRole]: Updating role {Id} by {User}", id, userName);

                var result = await _roleService.UpdateRoleAsync(request, userName);
                _logger.LogInformation("[UpdateRole]: Role {Id} updated successfully", id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "[UpdateRole]: Validation error updating role {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdateRole]: Error updating role {Id}", id);
                throw new BadHttpRequestException($"Error updating role: {ex.Message}");
            }
        }

        [HttpPost("delete/{id}")]
        [AuditLog("Role", "Delete")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? CommonConstants.UnknowUser;
                _logger.LogInformation("[DeleteRole]: Deleting role {Id} by {User}", id, userName);

                var result = await _roleService.DeleteRoleAsync(id, userName);
                _logger.LogInformation("[DeleteRole]: Role {Id} deleted successfully", id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "[DeleteRole]: Validation error deleting role {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DeleteRole]: Error deleting role {Id}", id);
                throw new BadHttpRequestException($"Error deleting role: {ex.Message}");
            }
        }

        [HttpPost("change-status/{id}")]
        [AuditLog("Role", "ToggleActive")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? CommonConstants.UnknowUser;
                _logger.LogInformation("[ToggleActive]: Toggling active status for role {Id} by {User}", id, userName);

                var isActive = await _roleService.ToggleActiveAsync(id, userName);
                _logger.LogInformation("[ToggleActive]: Role {Id} active status toggled to {IsActive}", id, isActive);
                return Ok(isActive);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "[ToggleActive]: Validation error toggling role {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ToggleActive]: Error toggling role {Id}", id);
                throw new BadHttpRequestException($"Error toggling role: {ex.Message}");
            }
        }
    }
}