using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.ExternalEntityModels
{
    [Table("BCI_ReservationSnapshot")]
    public class ReservationSnapshot
    {
        [Key]
        public long ReservationSnapshotID { get; set; }

        public int PropertyID { get; set; }

        public DateTime BusinessDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ReservationNo { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        public int? ReservationStatus { get; set; }

        [StringLength(50)]
        public string? RateCode { get; set; }

        [StringLength(50)]
        public string? ProductCode { get; set; }

        public DateTime? ArrivalDate { get; set; }

        public DateTime? DepartureDate { get; set; }

        public int AdultCount { get; set; }

        public int Child1Count { get; set; }

        public int Child2Count { get; set; }

        public int Child3Count { get; set; }

        public int TotalGuest { get; set; }

        [StringLength(250)]
        public string? MainGuestName { get; set; }

        public bool IsMainGuestCheckedIn { get; set; }

        public DateTime? PMSLastUpdated { get; set; }

        public Guid? SyncBatchID { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? PMSCheckedInTime { get; set; }

        public int NoChargePersonCount { get; set; }

        [StringLength(50)]
        public string? Resort { get; set; }

        public int? CheckedIn { get; set; }

        [StringLength(250)]
        public string? GuestName { get; set; }

        public bool MainGuest { get; set; }

        [StringLength(50)]
        public string? MembershipType { get; set; }

        [StringLength(50)]
        public string? IHGType { get; set; }
        [StringLength(50)]
        public string? MembershipNumber { get; set; }
    }
}