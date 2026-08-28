using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;

namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface IFormSubmissionService
    {
        Task<FormSubmissionResponse> SubmitAsync(SubmitFormRequest request, string createdBy);
        Task<FormSubmissionResponse?> GetByIdAsync(int id);
        Task<List<FormSubmissionBriefResponse>> GetByPatronDeviceIdAsync(int patronDeviceId);
        Task<List<FormSubmissionBriefResponse>> GetByTemplateIdAsync(int templateId);
    }
}
