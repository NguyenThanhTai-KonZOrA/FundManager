using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<List<Property>> GetActivePropertiesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => !p.IsDelete)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            return await _dbSet
                .Where(p => p.Id == id && !p.IsDelete)
                .FirstOrDefaultAsync();
        }
    }
}