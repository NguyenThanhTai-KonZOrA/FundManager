using DigitalDocumentPlatform.Common.BaseEntity;
using DigitalDocumentPlatform.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    public class Notification : BaseEntity
    {
        public int Id { get; set; }

        // Foreign key to StaffDevice
        public int StaffDeviceId { get; set; }

        // Business identifier (sessionId)
        public int SessionId { get; set; }

        // JSON payload (SignatureCompleted payload)
        [StringLength(2000)]
        public string PayloadJson { get; set; } = string.Empty;

        // Pending, Sent, Delivered, Failed
        [StringLength(50)]
        public string Status { get; set; } = NotificationStatus.Pending;

        public int AttemptCount { get; set; }

        [StringLength(1000)]
        public string? LastError { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? DeliveredAt { get; set; }
    }
}