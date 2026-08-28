using FundManager.Common.Enum;
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface IApplicationImageService
    {
        Task<List<ApplicationImageResponse>> GetAllActiveAsync();
        Task<List<ApplicationImageResponse>> GetByTypeAsync(ImageTypeEnum type);
        Task<List<ApplicationImageResponse>> GetByTypeAsync(ImageTypeEnum type, int? propertyId, int? outletId = null);
        Task<ApplicationImageResponse?> GetByIdAsync(int id);
        Task DeleteAsync(int id, string deletedBy);
    }
}