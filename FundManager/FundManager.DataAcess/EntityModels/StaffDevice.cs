using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    public class StaffDevice : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string MacAddress { get; set; } = string.Empty;

        [StringLength(100)]
        public string? IpAddress { get; set; }

        [StringLength(100)]
        public string? StaffUserName { get; set; }

        // SignalR ConnectionId
        [StringLength(200)]
        public string? ConnectionId { get; set; }

        public bool IsOnline { get; set; }

        public DateTime? LastHeartbeat { get; set; }

        /// <summary>
        /// Optional: the Outlet this staff device is assigned to.
        /// </summary>
        [ForeignKey(nameof(Outlet))]
        public int? OutletId { get; set; }

        public Outlet? Outlet { get; set; }

        /// <summary>Navigation to DeviceMapping records (1-1 with PatronDevice).</summary>
        public ICollection<DeviceMapping> DeviceMappings { get; set; } = [];
    }
}