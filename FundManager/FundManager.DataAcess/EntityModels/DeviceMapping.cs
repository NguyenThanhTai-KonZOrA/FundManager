using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// 1-1 Mapping between Staff PC and iPad devices
    /// </summary>
    public class DeviceMapping : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Staff PC Device (unique constraint per mapping)
        /// </summary>
        public int StaffDeviceId { get; set; }
        public StaffDevice StaffDevice { get; set; } = null!;

        /// <summary>
        /// iPad Device (unique constraint per mapping)
        /// </summary>
        public int PatronDeviceId { get; set; }
        public PatronDevice PatronDevice { get; set; } = null!;

        /// <summary>
        /// Physical location or desk number (e.g., "Desk 1", "Counter A")
        /// </summary>
        [StringLength(100)]
        public string? Location { get; set; }

        /// <summary>
        /// Notes for this pairing
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Last time this mapping was verified/used
        /// </summary>
        public DateTime? LastVerified { get; set; }
    }
}