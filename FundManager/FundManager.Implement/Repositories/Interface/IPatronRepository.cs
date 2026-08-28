using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IPatronRepository : IGenericRepository<Patron>
    {
        Task<Patron> GetPatronByIdAsync(int patronId);
        Task<Patron?> GetRandomPatronAsync();
        Task<bool> IsExistPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<Patron>> GetAllPatronsByIdsAsync(List<int> patronIds);
    }
}