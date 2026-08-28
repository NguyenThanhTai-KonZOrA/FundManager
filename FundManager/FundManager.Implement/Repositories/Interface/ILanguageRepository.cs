using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface ILanguageRepository : IGenericRepository<Language>
    {
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        Task<List<Language>> GetAllLanguagesAsync();
    }
}
