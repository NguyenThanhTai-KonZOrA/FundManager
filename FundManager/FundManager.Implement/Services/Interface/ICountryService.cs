using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface ICountryService
    {
        Task<List<CountryResponse>> LoadAllCountriesAsync();
        Task<string?> GetCountryNameByIdAsync(int id);
        Task<string> GetCountryIdByContainsNameAsync(string name);
        Task ReloadCacheAsync();
        Task<Dictionary<int, string>> GetCountryNamesByIdsAsync(IEnumerable<int> countryIds);
    }
}