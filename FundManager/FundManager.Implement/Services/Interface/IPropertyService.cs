using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
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