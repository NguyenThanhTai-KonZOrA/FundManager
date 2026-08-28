using FundManager.API.Filters;
using FundManager.Common.Constants;
using FundManager.Implement.Repositories.Interface;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FundManager.API.Controllers
{
    [Route("api/employee-role")]
    [ApiController]
    [Authorize]
    public class EmployeeRoleController : ControllerBase
    {
        private readonly IEmployeeRoleService _employeeRoleService;
        private readonly ILogger<EmployeeRoleController> _logger;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeRoleController(IEmployeeRoleService employeeRoleService,
            ILogger<EmployeeRoleController> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRoleService = employeeRoleService;
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Get employee with their roles and permissions
        /// </summary>
        [HttpGet("roles/{employeeId}")]
        public async Task<IActionResult> GetEmployeeWithRoles(int employeeId)
        {
            try
            {
                _logger.LogInformation("[GetEmployeeWithRoles]: Retrieving employee {EmployeeId} with roles", employeeId);
                var result = await _employeeRoleService.GetEmployeeWithRolesAsync(employeeId);

                if (result == null)
                {
                    _logger.LogWarning("[GetEmployeeWithRoles]: Employee {EmployeeId} not found", employeeId);
                    return NotFound(new { message = $"Employee with ID {employeeId} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetEmployeeWithRoles]: Error retrieving employee {EmployeeId}", employeeId);
                throw new BadHttpRequestException($"Error retrieving employee roles: {ex.Message}");
            }
        }

        /// <summary>
        /// Assign roles to an employee
        /// </summary>
        [HttpPost("assign-roles")]
        [AuditLog("EmployeeRole", "AssignRoles")]
        public async Task<IActionResult> AssignRolesToEmployee([FromBody] AssignRoleToEmployeeRequest request)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? CommonConstants.UnknowUser;
                _logger.LogInformation("[AssignRolesToEmployee]: Assigning {Count} roles to employee {EmployeeId} by {User}",
                    request.RoleIds.Count, request.EmployeeId, userName);

                var result = await _employeeRoleService.AssignRolesToEmployeeAsync(request, userName);

                if (result)
                {
                    _logger.LogInformation("[AssignRolesToEmployee]: Roles assigned successfully to employee {EmployeeId}", request.EmployeeId);
                    return Ok(result);
                }

                return BadRequest(new { message = "Failed to assign roles", success = false });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "[AssignRolesToEmployee]: Validation error assigning roles to employee {EmployeeId}", request.EmployeeId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AssignRolesToEmployee]: Error assigning roles to employee {EmployeeId}", request.EmployeeId);
                throw new BadHttpRequestException($"Error assigning roles to employee {request.EmployeeId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all permissions for an employee (from all their roles)
        /// </summary>
        [HttpGet("permissions/{employeeId}")]
        public async Task<IActionResult> GetEmployeePermissions(int employeeId)
        {
            try
            {
                _logger.LogInformation("[GetEmployeePermissions]: Retrieving permissions for employee {EmployeeId}", employeeId);
                var result = await _employeeRoleService.GetEmployeePermissionsAsync(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetEmployeePermissions]: Error retrieving permissions for employee {EmployeeId}", employeeId);
                throw new BadHttpRequestException($"Error retrieving permissions for employee {employeeId}: {ex.Message}");
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetEmployeeListAsync()
        {
            try
            {
                _logger.LogInformation("[GetEmployeeListAsync]: called");
                var employees = await _employeeRepository.GetActiveEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetEmployeeListAsync]: Error occurred while retrieving employee list");
                throw new BadHttpRequestException($"Error retrieving employee list: {ex.Message}");
            }
        }
    }
}