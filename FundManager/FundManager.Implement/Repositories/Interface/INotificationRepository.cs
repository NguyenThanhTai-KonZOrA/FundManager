using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<List<Notification>> GetPendingByStaffAsync(int staffDeviceId, int maxAttempts = 5);
        Task<Notification?> GetBySessionAndStaffAsync(int sessionId, int staffDeviceId);
        Task<List<Notification>> GetAllPendingAsync(int maxAttempts = 5);
        Task<List<Notification>> GetPendingBatchAsync(int maxAttempts, int batchSize);
        Task<List<Notification>> GetAllPendingOrSentAsync(int maxAttempts = 5);
        Task<List<Notification>> GetAllPendingOrSentBySessionAndStaffAsync(int sessionId, int staffDeviceId, int maxAttempts = 5);
    }
}