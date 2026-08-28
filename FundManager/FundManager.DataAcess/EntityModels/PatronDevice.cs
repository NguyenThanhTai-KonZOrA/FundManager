using FundManager.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;

namespace FundManager.DataAccess.EntityModels
{
    public class PatronDevice : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Unique device name (hostname/identifier)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// SignalR ConnectionId (not enforced as unique in DB, managed by app logic)
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ConnectionId { get; set; } = string.Empty;

        /// <summary>
        /// MAC Address (NOT unique - multiple devices may share same MAC in VM environments)
        /// </summary>
        [StringLength(200)]
        public string? MacAddress { get; set; }

        [StringLength(100)]
        public string? IpAddress { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsOnline { get; set; }
        public DateTime? LastHeartbeat { get; set; }
    }
}