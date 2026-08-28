using FundManager.Common.Enum;
using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class ApplicationImageRepository : GenericRepository<ApplicationImage>, IApplicationImageRepository
    {
        public ApplicationImageRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<List<ApplicationImage>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(i => !i.IsDelete)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ApplicationImage>> GetByTypeAsync(ImageTypeEnum type)
        {
            return await _dbSet
                .Where(i => i.Type == type && !i.IsDelete && i.IsActive)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<ApplicationImage?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Where(i => i.Id == id && !i.IsDelete)
                .FirstOrDefaultAsync();
        }

        public Task<List<ApplicationImage>> GetByPropertyIdAsync(int propertyId)
        {
            return _dbSet
                .Where(i => i.PropertyId == propertyId && !i.IsDelete && i.IsActive)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public Task<List<ApplicationImage>> GetByTypeAndPropertyAsync(ImageTypeEnum type, int propertyId)
        {
            return _dbSet
                .AsNoTracking()
                .Where(i => i.Type == type && i.PropertyId == propertyId && !i.IsDelete && i.IsActive)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public Task<List<ApplicationImage>> GetByTypeAndOutletAsync(ImageTypeEnum type, int outletId)
        {
            return _dbSet
                .Where(i => i.Type == type && i.OutletId == outletId && !i.IsDelete && i.IsActive)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
    }
}