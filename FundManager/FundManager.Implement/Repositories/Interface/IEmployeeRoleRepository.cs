using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IEmployeeRoleRepository : IGenericRepository<EmployeeRole>
    {
        Task<List<EmployeeRole>> GetByEmployeeIdAsync(int employeeId);
        Task RemoveByEmployeeIdAsync(int employeeId);
        Task<List<int>> GetRoleIdsByEmployeeIdAsync(int employeeId);
        Task<List<string>> GetPermissionsByEmployeeIdAsync(int employeeId);
    }
}