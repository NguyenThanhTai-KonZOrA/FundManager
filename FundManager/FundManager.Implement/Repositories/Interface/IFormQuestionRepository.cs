using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IFormQuestionRepository : IGenericRepository<FormQuestion>
    {
        Task<List<FormQuestion>> GetByTemplateIdAsync(int templateId);
        Task<FormQuestion?> GetByIdWithOptionsAsync(int id);
        Task ReorderAsync(int templateId, List<(int questionId, int newOrder)> orderMap);
    }
}
