using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface ILanguageRepository : IGenericRepository<Language>
    {
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        Task<List<Language>> GetAllLanguagesAsync();
    }
}
