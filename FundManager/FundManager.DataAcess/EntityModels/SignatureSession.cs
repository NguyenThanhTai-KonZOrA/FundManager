using DigitalDocumentPlatform.Common.BaseEntity;
using DigitalDocumentPlatform.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    public class SignatureSession : BaseEntity
    {
        public long Id { get; set; }

        public int PatronId { get; set; }
        public Patron Patron { get; set; } = null!;

        public int StaffDeviceId { get; set; }
        public StaffDevice StaffDevice { get; set; } = null!;

        public int PatronDeviceId { get; set; }
        public PatronDevice PatronDevice { get; set; } = null!;

        [StringLength(50)]
        public string Status { get; set; } = SignatureSessionStatus.Pending; // Pending, Viewed, Signed, Rejected, Expired

        public DateTime RequestedAt { get; set; }

        public DateTime? ViewedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}