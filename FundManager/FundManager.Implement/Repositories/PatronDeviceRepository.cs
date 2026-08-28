using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class PatronDeviceRepository : GenericRepository<PatronDevice>, IPatronDeviceRepository
    {
        public PatronDeviceRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<PatronDevice?> GetAvailableDeviceForStaffAsync(int staffDeviceId)
        {
            // Get PatronDevice mapped to this StaffDevice via DeviceMapping
            var mapping = await _context.DeviceMappings
                .Include(m => m.PatronDevice)
                .FirstOrDefaultAsync(m =>
                    m.StaffDeviceId == staffDeviceId &&
                    m.IsActive &&
                    !m.IsDelete);

            if (mapping == null)
            {
                return null;
            }

            var device = mapping.PatronDevice;

            // Check if device is available
            if (!device.IsOnline || !device.IsAvailable || device.IsDelete)
            {
                return null;
            }

            return device;
        }

        public async Task<List<PatronDevice>> GetOnlineDevicesAsync()
        {
            return await _context.PatronDevices
                .Where(d => d.IsOnline)
                .OrderBy(x => x.DeviceName)
                .ToListAsync();
        }
    }
}