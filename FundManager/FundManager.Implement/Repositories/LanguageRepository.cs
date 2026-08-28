using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.Languages.Where(l => l.Code == code);
            if (excludeId.HasValue)
                query = query.Where(l => l.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<List<Language>> GetAllLanguagesAsync()
        {
            return await _context.Languages.AsNoTracking().Where(l => !l.IsDelete).ToListAsync();
        }
    }
}
