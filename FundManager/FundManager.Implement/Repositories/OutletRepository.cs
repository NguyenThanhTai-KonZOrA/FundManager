using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class OutletRepository : GenericRepository<Outlet>, IOutletRepository
    {
        public OutletRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<List<Outlet>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.IsActive && !o.IsDelete)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<List<Outlet>> GetAllActiveWithPropertiesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.PropertyOutlets)
                    .ThenInclude(po => po.Property)
                .AsSplitQuery()
                .Where(o => !o.IsDelete)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        /// <summary>Gets all outlets linked to a property via the PropertyOutlet join table.</summary>
        public async Task<List<Outlet>> GetByPropertyIdAsync(int propertyId)
        {
            return await _dbSet
                .Include(o => o.PropertyOutlets)
                .Where(o => !o.IsDelete && o.PropertyOutlets.Any(po => po.PropertyId == propertyId))
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<Outlet?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Where(o => o.Id == id && !o.IsDelete)
                .FirstOrDefaultAsync();
        }

        public async Task<Outlet?> GetByIdWithPropertiesAsync(int id)
        {
            return await _dbSet
                .Include(o => o.PropertyOutlets)
                    .ThenInclude(po => po.Property)
                .Where(o => o.Id == id && !o.IsDelete)
                .FirstOrDefaultAsync();
        }
    }
}