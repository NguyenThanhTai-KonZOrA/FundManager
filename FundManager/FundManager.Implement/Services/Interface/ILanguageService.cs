using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface ILanguageService
    {
        Task<List<LanguageResponse>> GetAllAsync();
        Task<LanguageResponse?> GetByIdAsync(int id);
        Task<LanguageResponse> CreateAsync(CreateLanguageRequest request, string createdBy);
        Task<LanguageResponse> UpdateAsync(UpdateLanguageRequest request, string updatedBy);
        Task<bool> DeleteAsync(int id, string deletedBy);
        Task<bool> ToggleActiveAsync(int id, string updatedBy);
    }
}
