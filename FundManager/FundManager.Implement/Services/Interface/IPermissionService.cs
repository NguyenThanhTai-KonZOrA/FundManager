using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface IPermissionService
    {
        Task<List<PermissionResponse>> GetAllPermissionsAsync();
        Task<PermissionResponse?> GetPermissionByIdAsync(int permissionId);
        Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request, string createdBy);
        Task<PermissionResponse> UpdatePermissionAsync(UpdatePermissionRequest request, string updatedBy);
        Task<bool> DeletePermissionAsync(int permissionId, string deletedBy);
        Task<bool> ToggleActiveAsync(int permissionId, string updatedBy);
        Task<Dictionary<string, List<PermissionResponse>>> GetPermissionsByCategoryAsync();
    }
}