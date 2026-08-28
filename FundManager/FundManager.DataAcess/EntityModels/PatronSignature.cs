using DigitalDocumentPlatform.Common.BaseEntity;
using DigitalDocumentPlatform.Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    public class PatronSignature : BaseEntity
    {
        public int Id { get; set; }

        public int PatronId { get; set; }

        public DocumentTypeEnum DocumentType { get; set; }
        [StringLength(200)]
        public string DocumentPath { get; set; }

        [Required]
        public string SignatureData { get; set; }

        public DateTime SignedDate { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(255)]
        public string? DeviceInfo { get; set; }
        [StringLength(255)]
        public string? Location { get; set; }

        // Navigation property
        public virtual Patron Patron { get; set; }
    }
}