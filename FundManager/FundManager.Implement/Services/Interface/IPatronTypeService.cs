using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;

namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface IPatronTypeService
    {
        Task<List<PatronTypeResponse>> GetAllAsync();
        Task<PatronTypeResponse?> GetByIdAsync(int id);
        Task<PatronTypeResponse> CreateAsync(CreatePatronTypeRequest request, string createdBy);
        Task<PatronTypeResponse> UpdateAsync(UpdatePatronTypeRequest request, string updatedBy);
        Task<bool> DeleteAsync(int id, string deletedBy);
        Task<bool> ToggleActiveAsync(int id, string updatedBy);
    }
}
