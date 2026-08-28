using FundManager.Implement.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FundManager.API.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryService countryService, ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all countries with full data (ID, Description, Abrv2, Abrv3)
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCountriesFull()
        {
            try
            {
                _logger.LogInformation("[GetAllCountriesFull] START - Fetching all countries with full data");
                var countries = await _countryService.LoadAllCountriesAsync();
                _logger.LogInformation("[GetAllCountriesFull] END - Retrieved {Count} countries", countries.Count);
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetAllCountriesFull] FAILED - Error: {Message}", ex.Message);
                throw new BadHttpRequestException($"Error fetching countries: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountryById(int id)
        {
            try
            {
                _logger.LogInformation("[GetCountryById] START - CountryId: {Id}", id);
                var country = await _countryService.GetCountryNameByIdAsync(id);

                if (country == null)
                {
                    _logger.LogWarning("[GetCountryById] NOT FOUND - CountryId: {Id}", id);
                    return NotFound(new { success = false, message = "Country not found" });
                }

                _logger.LogInformation("[GetCountryById] END - CountryId: {Id}, Name: {Name}", id, country);
                return Ok(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetCountryById] FAILED - CountryId: {Id}", id);
                throw new BadHttpRequestException($"Error fetching country by ID: {ex.Message}");
            }
        }

        [HttpPost("reload-cache")]
        public async Task<IActionResult> ReloadCache()
        {
            try
            {
                _logger.LogInformation("[ReloadCache] START - Reloading country cache");
                await _countryService.ReloadCacheAsync();
                _logger.LogInformation("[ReloadCache] END - Cache reloaded successfully");
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReloadCache] FAILED");
                throw new BadHttpRequestException($"Error reloading country cache: {ex.Message}");
            }
        }
    }
}