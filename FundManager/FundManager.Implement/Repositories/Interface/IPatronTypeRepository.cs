using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IPatronTypeRepository : IGenericRepository<PatronType>
    {
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}
