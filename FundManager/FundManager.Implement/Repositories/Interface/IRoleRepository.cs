using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetRoleWithPermissionsAsync(int roleId);
        Task<List<Role>> GetAllRolesWithPermissionsAsync();
        Task<bool> RoleNameExistsAsync(string roleName, int? excludeId = null);
    }
}