using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;

namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface IPropertyService
    {
        Task<List<PropertyResponse>> GetActivePropertiesAsync();
        Task<PropertyResponse?> GetPropertyByIdAsync(int id);
        Task<PropertyResponse> CreatePropertyAsync(CreatePropertyRequest request, string createdBy);
        Task<PropertyResponse> UpdatePropertyAsync(UpdatePropertyRequest request, string updatedBy);
        Task DeletePropertyAsync(int id, string deletedBy);
    }
}