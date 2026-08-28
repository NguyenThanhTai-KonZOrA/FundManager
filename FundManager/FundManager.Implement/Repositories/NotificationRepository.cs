using FundManager.Common.Constants;
using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<List<Notification>> GetPendingByStaffAsync(int staffDeviceId, int maxAttempts = 5)
        {
            return await _context.Set<Notification>()
                .Where(n => n.StaffDeviceId == staffDeviceId && n.Status == NotificationStatus.Pending && n.AttemptCount < maxAttempts)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetBySessionAndStaffAsync(int sessionId, int staffDeviceId)
        {
            return await _context.Set<Notification>()
                .FirstOrDefaultAsync(n => n.SessionId == sessionId && n.StaffDeviceId == staffDeviceId);
        }

        public async Task<List<Notification>> GetAllPendingAsync(int maxAttempts = 5)
        {
            return await _context.Set<Notification>()
                .Where(n => n.Status == NotificationStatus.Pending && n.AttemptCount < maxAttempts)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetPendingBatchAsync(int maxAttempts, int batchSize)
        {
            return await _context.Set<Notification>()
                .Where(n => n.Status == NotificationStatus.Pending && n.AttemptCount < maxAttempts)
                .OrderBy(n => n.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetAllPendingOrSentAsync(int maxAttempts = 5)
        {
            return await _context.Set<Notification>()
                .Where(n =>
                    (n.Status == NotificationStatus.Pending || n.Status == NotificationStatus.Sent) &&
                    n.AttemptCount < maxAttempts)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetAllPendingOrSentBySessionAndStaffAsync(int sessionId, int staffDeviceId, int maxAttempts = 5)
        {
            return await _context.Set<Notification>()
                .Where(n =>
                    (n.Status == NotificationStatus.Pending || n.Status == NotificationStatus.Sent) &&
                    n.AttemptCount < maxAttempts &&
                    n.SessionId == sessionId &&
                    n.StaffDeviceId == staffDeviceId)
                .OrderBy(n => n.SentAt)
                .ToListAsync();
        }
    }
}