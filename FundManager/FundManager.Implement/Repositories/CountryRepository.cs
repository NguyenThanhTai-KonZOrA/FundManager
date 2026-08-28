using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
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
