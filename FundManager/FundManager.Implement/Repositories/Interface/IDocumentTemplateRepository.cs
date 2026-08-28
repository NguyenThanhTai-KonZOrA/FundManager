using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IDocumentTemplateRepository : IGenericRepository<DocumentTemplate>
    {
        Task<List<DocumentTemplate>> GetAllActiveAsync();
        Task<DocumentTemplate?> GetByIdAsync(int id);
        Task<List<DocumentTemplate>> GetByTypeAsync(DocumentType documentType);
        Task<List<DocumentTemplate>> GetByOutletAsync(int outletId);
    }
}