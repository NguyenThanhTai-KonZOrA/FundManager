using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        Task<bool> PermissionCodeExistsAsync(string permissionCode, int? excludeId = null);
        Task<List<Permission>> GetByIdsAsync(List<int> permissionIds);
    }
}