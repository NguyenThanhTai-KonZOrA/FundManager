using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IPropertyOutletRepository : IGenericRepository<PropertyOutlet>
    {
        Task<List<PropertyOutlet>> GetByPropertyIdAsync(int propertyId);
        Task<List<PropertyOutlet>> GetByOutletIdAsync(int outletId);
        void Remove(PropertyOutlet entity);
    }
}