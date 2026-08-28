using DigitalDocumentPlatform.Common.Enum;
using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
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