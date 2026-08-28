using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface ISignalRService
    {
        Task SendSignatureRequestToDeviceAsync(int patronId, int staffDeviceId);
        Task NotifyStaffSignatureCompletedAsync(int staffDeviceId, int sessionId, Patron patron);
        Task NotifyNewRegistrationAsync(Patron patron);
        // Notification resend for pending notifications
        Task ResendPendingNotificationsForStaffAsync(int staffDeviceId, string connectionId);
        Task AcknowledgeNotificationAsync(int staffDeviceId, int sessionId);
    }
}