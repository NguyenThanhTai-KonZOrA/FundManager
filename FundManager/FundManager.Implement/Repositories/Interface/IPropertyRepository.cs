using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task<List<Property>> GetActivePropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(int id);
    }
}