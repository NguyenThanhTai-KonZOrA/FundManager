using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
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