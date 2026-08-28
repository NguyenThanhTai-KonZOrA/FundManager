using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.ViewModels.Response;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IStaffDeviceRepository : IGenericRepository<StaffDevice>
    {
        // Get staff device by ConnectionId
        Task<StaffDevice?> GetByConnectionIdAsync(string connectionId);

        // Update ConnectionId
        Task<bool> UpdateConnectionIdAsync(string deviceName, string connectionId);

        // Set device offline
        Task SetOfflineByConnectionIdAsync(string connectionId);

        // Get all online staff devices
        Task<List<StaffDevice>> GetOnlineDevicesAsync();

        // Get all staff devices (with DeviceMapping) assigned to a specific outlet
        Task<List<StaffDevice>> GetByOutletIdAsync(int outletId);

        // Get a single staff device including its Outlet navigation
        Task<StaffDevice?> GetByIdWithOutletAsync(int id);

        // Assign a staff device to an outlet
        Task<bool> AssignToOutletAsync(int staffDeviceId, int outletId, string updatedBy);

        // Remove the outlet assignment from a staff device
        Task<bool> UnassignFromOutletAsync(int staffDeviceId, string updatedBy);

        Task<OutletResponse?> GetOutletByStaffDeviceIdAsync(int staffDeviceId);
        Task<StaffDevice?> GetStaffDeviceByNameAsync(string deviceName);
    }
}