
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface IOutletService
    {
        Task<List<OutletResponse>> GetAllActiveAsync();
        Task<List<OutletResponse>> GetByPropertyIdAsync(int propertyId);
        Task<OutletResponse?> GetByIdAsync(int id);
        Task<OutletResponse> CreateAsync(CreateOutletRequest request, string createdBy);
        Task<OutletResponse> UpdateAsync(UpdateOutletRequest request, string updatedBy);
        Task DeleteAsync(int id, string deletedBy);

        // Outlet ↔ StaffDevice assignment
        Task<List<OutletStaffDeviceResponse>> GetStaffDevicesByOutletAsync(int outletId);
        Task<bool> AssignStaffDeviceAsync(int outletId, int staffDeviceId, string updatedBy);
        Task<bool> UnassignStaffDeviceAsync(int outletId, int staffDeviceId, string updatedBy);
    }
}
