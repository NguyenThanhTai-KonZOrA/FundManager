using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DigitalDocumentPlatformDbContext _context;
        public CountryRepository(DigitalDocumentPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _context.Countries.AsNoTracking().ToListAsync();
        }
    }
}
