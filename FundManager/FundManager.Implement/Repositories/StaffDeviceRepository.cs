using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using FundManager.Implement.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class StaffDeviceRepository : GenericRepository<StaffDevice>, IStaffDeviceRepository
    {
        public StaffDeviceRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<StaffDevice?> GetByConnectionIdAsync(string connectionId)
        {
            return await _context.StaffDevices
                .FirstOrDefaultAsync(s => s.ConnectionId == connectionId);
        }

        public async Task<bool> UpdateConnectionIdAsync(string deviceName, string connectionId)
        {
            var device = await _context.StaffDevices
                .FirstOrDefaultAsync(s => s.DeviceName == deviceName);

            if (device == null)
                return false;

            device.ConnectionId = connectionId;
            device.IsOnline = true;
            device.LastHeartbeat = DateTime.Now;
            device.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SetOfflineByConnectionIdAsync(string connectionId)
        {
            var device = await _context.StaffDevices
                .FirstOrDefaultAsync(s => s.ConnectionId == connectionId);

            if (device != null)
            {
                device.IsOnline = false;
                device.ConnectionId = null;
                device.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Get all online staff devices
        public async Task<List<StaffDevice>> GetOnlineDevicesAsync()
        {
            return await _context.StaffDevices
                .Where(d => d.IsOnline && !string.IsNullOrEmpty(d.ConnectionId))
                .OrderByDescending(d => d.LastHeartbeat)
                .ToListAsync();
        }

        public async Task<List<StaffDevice>> GetByOutletIdAsync(int outletId)
        {
            return await _context.StaffDevices
                .Include(s => s.Outlet)
                .Include(s => s.DeviceMappings.Where(m => m.IsActive && !m.IsDelete))
                    .ThenInclude(m => m.PatronDevice)
                .Where(s => s.OutletId == outletId && !s.IsDelete)
                .OrderBy(s => s.DeviceName)
                .ToListAsync();
        }

        public async Task<StaffDevice?> GetByIdWithOutletAsync(int id)
        {
            return await _context.StaffDevices
                .Include(s => s.Outlet)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete);
        }

        public async Task<bool> AssignToOutletAsync(int staffDeviceId, int outletId, string updatedBy)
        {
            var device = await _context.StaffDevices
                .FirstOrDefaultAsync(s => s.Id == staffDeviceId && !s.IsDelete);

            if (device == null) return false;

            device.OutletId = outletId;
            device.UpdatedBy = updatedBy;
            device.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnassignFromOutletAsync(int staffDeviceId, string updatedBy)
        {
            var device = await _context.StaffDevices
                .FirstOrDefaultAsync(s => s.Id == staffDeviceId && !s.IsDelete);

            if (device == null) return false;

            device.OutletId = null;
            device.UpdatedBy = updatedBy;
            device.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OutletResponse?> GetOutletByStaffDeviceIdAsync(int staffDeviceId)
        {
            var device = await _context.StaffDevices
                .Include(s => s.Outlet)
                .FirstOrDefaultAsync(s => s.Id == staffDeviceId && !s.IsDelete);
            if (device?.Outlet == null) return null;

            return new OutletResponse
            {
                Id = device.Outlet.Id,
                Name = device.Outlet.Name,
                Code = device.Outlet.Code,
                Description = device.Outlet.Description,
                MainColor = device.Outlet.MainColor,
                IconImageUrl = device.Outlet.IconImageUrl,
                BackgroundImageUrl = device.Outlet.BackgroundImageUrl,
                IsActive = device.Outlet.IsActive,
                CreatedAt = device.Outlet.CreatedAt,
                UpdatedAt = device.Outlet.UpdatedAt
            };
        }

        public async Task<StaffDevice?> GetStaffDeviceByNameAsync(string deviceName)
        {
            return await _context.StaffDevices
                .Include(s => s.Outlet)
                .FirstOrDefaultAsync(s => s.DeviceName == deviceName && !s.IsDelete);
        }
    }
}