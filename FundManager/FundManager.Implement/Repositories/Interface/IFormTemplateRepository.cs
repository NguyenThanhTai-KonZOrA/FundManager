using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IFormTemplateRepository : IGenericRepository<FormTemplate>
    {
        Task<List<FormTemplate>> GetAllActiveAsync();
        Task<FormTemplate?> GetByIdWithQuestionsAsync(int id);
    }
}
