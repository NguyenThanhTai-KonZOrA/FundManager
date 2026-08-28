using FundManager.Common.Enum;
using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IApplicationImageRepository : IGenericRepository<ApplicationImage>
    {
        Task<List<ApplicationImage>> GetAllActiveAsync();
        Task<List<ApplicationImage>> GetByTypeAsync(ImageTypeEnum type);
        Task<ApplicationImage?> GetByIdAsync(int id);
        Task<List<ApplicationImage>> GetByPropertyIdAsync(int propertyId);
        Task<List<ApplicationImage>> GetByTypeAndPropertyAsync(ImageTypeEnum type, int propertyId);
        Task<List<ApplicationImage>> GetByTypeAndOutletAsync(ImageTypeEnum type, int outletId);
    }
}