using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IPatronTypeRepository : IGenericRepository<PatronType>
    {
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}
