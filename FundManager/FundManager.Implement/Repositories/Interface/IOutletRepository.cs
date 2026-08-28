using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IOutletRepository : IGenericRepository<Outlet>
    {
        Task<List<Outlet>> GetAllActiveAsync();
        /// <summary>Gets all outlets linked to a property via the PropertyOutlet join table.</summary>
        Task<List<Outlet>> GetByPropertyIdAsync(int propertyId);
        /// <summary>Gets outlets with their PropertyOutlets navigation loaded.</summary>
        Task<List<Outlet>> GetAllActiveWithPropertiesAsync();
        Task<Outlet?> GetByIdWithPropertiesAsync(int id);
        Task<Outlet?> GetByIdAsync(int id);
    }
}