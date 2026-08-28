using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        Task<bool> PermissionCodeExistsAsync(string permissionCode, int? excludeId = null);
        Task<List<Permission>> GetByIdsAsync(List<int> permissionIds);
    }
}