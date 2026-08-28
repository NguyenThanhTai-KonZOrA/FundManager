using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface IEmployeeRoleService
    {
        Task<EmployeeWithRolesResponse?> GetEmployeeWithRolesAsync(int employeeId);
        Task<bool> AssignRolesToEmployeeAsync(AssignRoleToEmployeeRequest request, string assignedBy);
        Task<List<string>> GetEmployeePermissionsAsync(int employeeId);
    }
}