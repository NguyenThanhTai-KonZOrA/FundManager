using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IFormTemplateRepository : IGenericRepository<FormTemplate>
    {
        Task<List<FormTemplate>> GetAllActiveAsync();
        Task<FormTemplate?> GetByIdWithQuestionsAsync(int id);
    }
}
