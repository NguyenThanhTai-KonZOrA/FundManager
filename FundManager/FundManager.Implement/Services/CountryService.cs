using DigitalDocumentPlatform.Common.MemoryCache;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.ViewModels.Response;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class CountryService : ICountryService
    {
        #region Contructor
        private readonly ICacheService _cache;
        private const string CacheKey = "CountryList";
        private const string CacheFullKey = "CountryFullList";
        private const string ReverseCacheKey = "CountryReverseList";
        private readonly ICountryRepository _countryRepository;
        public CountryService(ICacheService cache, ICountryRepository countryRepository)
        {
            _cache = cache;
            _countryRepository = countryRepository;
        }
        #endregion

        #region Main functions using on the application

        public async Task<List<CountryResponse>> LoadAllCountriesAsync()
        {
            var countriesResult = await _cache.GetOrCreateCache(CacheFullKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30);
                var countries = await _countryRepository.GetAllCountriesAsync();
                return countries.Select(c => new CountryResponse
                {
                    Id = c.Id,
                    Description = c.Description,
                    Abrv2 = c.Abrv2,
                    Abrv3 = c.Abrv3
                }).ToList();
            });
            Console.WriteLine($"[CountryCacheService] Loaded {countriesResult!.Count} countries full data into cache.");

            return countriesResult;
        }

        public async Task<string?> GetCountryNameByIdAsync(int id)
        {
            var countries = await LoadAllCountriesAsync();
            var country = countries.FirstOrDefault(c => c.Id == id);
            return country?.Description;
        }

        public async Task<int?> GetCountryIdByNameAsync(string name)
        {
            if (_cache.TryGetValue(ReverseCacheKey, out IDictionary<string, int> reverse))
            {
                var key = name.ToLowerInvariant();
                return reverse.TryGetValue(key, out var id) ? id : null;
            }

            await LoadAllCountriesAsync();
            return await GetCountryIdByNameAsync(name);
        }

        public async Task<int> GetCountryIdAsync(string countryName)
        {
            var countryId = 704; // Default: Viet Nam
            var countries = await LoadAllCountriesAsync();
            var country = countries.FirstOrDefault(c => c.Description.Equals(countryName, StringComparison.OrdinalIgnoreCase));
            if (country != null)
            {
                countryId = country.Id;
            }

            return countryId;
        }

        public async Task<string> GetCountryIdByContainsNameAsync(string name)
        {
            var countryMappingName = "Viet Nam";
            var countries = await LoadAllCountriesAsync();
            var country = countries.FirstOrDefault(c => c.Description.Contains(name, StringComparison.OrdinalIgnoreCase));

            // Fixed: Return the found country name
            if (country != null)
            {
                countryMappingName = country.Description;
            }

            return countryMappingName;
        }

        // Add to ICountryService interface and implementation
        public async Task<Dictionary<int, string>> GetCountryNamesByIdsAsync(IEnumerable<int> countryIds)
        {
            if (countryIds == null || !countryIds.Any())
                return new Dictionary<int, string>();

            var countries = await LoadAllCountriesAsync();

            return countries.ToDictionary(c => c.Id, c => c.Description);
        }

        public async Task ReloadCacheAsync()
        {
            _cache.Remove(CacheKey);
            _cache.Remove(ReverseCacheKey);
            _cache.Remove(CacheFullKey);
            await LoadAllCountriesAsync();
        }

        // Fixed: Removed incorrect explicit interface implementations
        #endregion
    }
}