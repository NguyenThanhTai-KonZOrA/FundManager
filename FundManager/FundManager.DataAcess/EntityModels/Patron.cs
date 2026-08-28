using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    public class Patron : BaseEntity
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string? FirstName { get; set; }
        [StringLength(255)]
        public string? LastName { get; set; }
        [StringLength(255)]
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(50)]
        public string? PhoneNumber { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(255)]
        public string? Nationality { get; set; }
        [StringLength(255)]
        public string? PostalCode { get; set; }
        public int? PlayerId { get; set; }

        // ─── New fields ───────────────────────────────────────────────
        /// <summary>Which outlet (spa/facility) the patron visited.</summary>
        [ForeignKey(nameof(Outlet))]
        public int? OutletId { get; set; }
        public Outlet? Outlet { get; set; }

        /// <summary>Hotel room number for in-house guests.</summary>
        [StringLength(20)]
        public string? RoomNumber { get; set; }

        /// <summary>
        /// IETF language code chosen by the patron at the kiosk (e.g. "en", "vi", "ko").
        /// </summary>
        [StringLength(10)]
        public string? Language { get; set; }

        /// <summary>Patron classification (VIP, Standard, …).</summary>
        [ForeignKey(nameof(PatronType))]
        public int? PatronTypeId { get; set; }
        public PatronType? PatronType { get; set; }

        /// <summary>InHouse | WalkIn</summary>
        [StringLength(20)]
        public string? CustomerType { get; set; }

        // Navigation
        public ICollection<PatronSignature> Signatures { get; set; } = [];
    }
}