using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface ICountryRepository
    {
        Task<List<Country>> GetAllCountriesAsync();
    }
}