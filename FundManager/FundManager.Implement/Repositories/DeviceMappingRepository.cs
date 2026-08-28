using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class DeviceMappingRepository : GenericRepository<DeviceMapping>, IDeviceMappingRepository
    {
        public DeviceMappingRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<DeviceMapping?> GetMappingByStaffDeviceIdAsync(int staffDeviceId)
        {
            return await _context.DeviceMappings
                .Include(m => m.StaffDevice)
                .Include(m => m.PatronDevice)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.StaffDeviceId == staffDeviceId && m.IsActive);
        }

        public async Task<DeviceMapping?> GetMappingByPatronDeviceIdAsync(int patronDeviceId)
        {
            return await _context.DeviceMappings
                .Include(m => m.StaffDevice)
                .Include(m => m.PatronDevice)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.PatronDeviceId == patronDeviceId && m.IsActive);
        }

        public async Task<DeviceMapping?> GetMappingByStaffDeviceNameAsync(string staffDeviceName)
        {
            return await _context.DeviceMappings
                .Include(m => m.StaffDevice)
                .Include(m => m.PatronDevice)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m =>
                    m.StaffDevice.DeviceName == staffDeviceName && m.IsActive);
        }

        public async Task<DeviceMapping?> GetMappingByPatronDeviceNameAsync(string patronDeviceName)
        {
            return await _context.DeviceMappings
                .Include(m => m.StaffDevice)
                    .ThenInclude(sd => sd.Outlet)
                .Include(m => m.PatronDevice)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m =>
                    m.PatronDevice.DeviceName == patronDeviceName && m.IsActive);
        }

        public async Task<List<DeviceMapping>> GetAllActiveMappingsAsync()
        {
            return await _context.DeviceMappings
                .AsNoTracking()
                .Include(m => m.StaffDevice)
                    .ThenInclude(sd => sd.Outlet)
                        .ThenInclude(o => o!.PropertyOutlets)
                            .ThenInclude(po => po.Property)
                .Include(m => m.PatronDevice)
                .AsSplitQuery()
                .Where(m => m.IsActive)
                .OrderBy(m => m.CreatedAt)
                .ThenBy(m => m.StaffDevice.DeviceName)
                .ToListAsync();
        }

        public async Task<bool> IsMappingExistsAsync(int staffDeviceId, int patronDeviceId)
        {
            return await _context.DeviceMappings
                .AnyAsync(m =>
                    m.StaffDeviceId == staffDeviceId &&
                    m.PatronDeviceId == patronDeviceId &&
                    m.IsActive);
        }

        public async Task<DeviceMapping?> GetMappingByStaffAndPatronDeviceIdAsync(int staffDeviceId, int patronDeviceId)
        {
            return await _context.DeviceMappings
                .Include(m => m.StaffDevice)
                .Include(m => m.PatronDevice)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.StaffDeviceId == staffDeviceId && m.PatronDeviceId == patronDeviceId && m.IsActive);
        }
    }
}