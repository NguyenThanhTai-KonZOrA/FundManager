using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IDeviceMappingRepository : IGenericRepository<DeviceMapping>
    {
        Task<DeviceMapping?> GetMappingByStaffDeviceIdAsync(int staffDeviceId);
        Task<DeviceMapping?> GetMappingByPatronDeviceIdAsync(int patronDeviceId);
        Task<DeviceMapping?> GetMappingByStaffDeviceNameAsync(string staffDeviceName);
        Task<DeviceMapping?> GetMappingByPatronDeviceNameAsync(string patronDeviceName);
        Task<List<DeviceMapping>> GetAllActiveMappingsAsync();
        Task<bool> IsMappingExistsAsync(int staffDeviceId, int patronDeviceId);
        Task<DeviceMapping?> GetMappingByStaffAndPatronDeviceIdAsync(int staffDeviceId, int patronDeviceId);
    }
}