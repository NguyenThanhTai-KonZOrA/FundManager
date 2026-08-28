using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface ICountryRepository
    {
        Task<List<Country>> GetAllCountriesAsync();
    }
}