using DigitalDocumentPlatform.Common.Enum;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;

namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface IApplicationImageService
    {
        Task<List<ApplicationImageResponse>> GetAllActiveAsync();
        Task<List<ApplicationImageResponse>> GetByTypeAsync(ImageTypeEnum type);
        Task<List<ApplicationImageResponse>> GetByTypeAsync(ImageTypeEnum type, int? propertyId, int? outletId = null);
        Task<ApplicationImageResponse?> GetByIdAsync(int id);
        Task<ApplicationImageResponse> CreateAsync(CreateApplicationImageRequest request, string createdBy);
        Task<ApplicationImageResponse> UpdateAsync(UpdateApplicationImageRequest request, string updatedBy);
        Task DeleteAsync(int id, string deletedBy);
    }
}