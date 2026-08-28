using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IFormSubmissionRepository : IGenericRepository<FormSubmission>
    {
        Task<FormSubmission?> GetByIdWithAnswersAsync(int id);
        Task<List<FormSubmission>> GetByPatronDeviceIdAsync(int patronDeviceId);
        Task<List<FormSubmission>> GetByTemplateIdAsync(int templateId);
    }
}
