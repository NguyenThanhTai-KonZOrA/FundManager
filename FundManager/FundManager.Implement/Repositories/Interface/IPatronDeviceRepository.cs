using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IPatronDeviceRepository : IGenericRepository<PatronDevice>
    {
        Task<PatronDevice?> GetAvailableDeviceForStaffAsync(int staffDeviceId);
        Task<List<PatronDevice>> GetOnlineDevicesAsync();
    }
}