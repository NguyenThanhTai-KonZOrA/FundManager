using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class ApplicationSettingsRepository : GenericRepository<ApplicationSettings>, IApplicationSettingsRepository
    {
        private readonly FundManagerDbContext _context;

        public ApplicationSettingsRepository(FundManagerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ApplicationSettings?> GetByKeyAsync(string key)
        {
            return await _context.ApplicationSettings
                .FirstOrDefaultAsync(s => s.Key == key);
        }

        public async Task<List<ApplicationSettings>> GetByCategoryAsync(string category)
        {
            return await _context.ApplicationSettings
                .Where(s => s.Category == category)
                .OrderBy(s => s.Key)
                .ToListAsync();
        }

        public async Task<bool> UpdateSettingAsync(string key, string value, string updatedBy)
        {
            var setting = await GetByKeyAsync(key);
            if (setting == null) return false;

            setting.Value = value;
            setting.UpdatedBy = updatedBy;
            setting.UpdatedAt = DateTime.Now;

            _context.ApplicationSettings.Update(setting);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<ApplicationSettings>> GetAllApplicationSettingsAsync()
        {
            return await _context.ApplicationSettings.AsNoTracking().OrderByDescending(s => s.Category).ToListAsync();
        }
    }
}