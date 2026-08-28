using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class PropertyOutletRepository : GenericRepository<PropertyOutlet>, IPropertyOutletRepository
    {
        public PropertyOutletRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<List<PropertyOutlet>> GetByPropertyIdAsync(int propertyId)
        {
            return await _dbSet
                .Where(po => po.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<List<PropertyOutlet>> GetByOutletIdAsync(int outletId)
        {
            return await _dbSet
                .Where(po => po.OutletId == outletId)
                .ToListAsync();
        }

        public void Remove(PropertyOutlet entity)
        {
            _dbSet.Remove(entity);
        }
    }
}