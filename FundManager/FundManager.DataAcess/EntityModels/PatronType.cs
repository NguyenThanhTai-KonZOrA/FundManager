using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// Patron classification type, e.g. VIP, Standard, Member.
    /// Managed via admin UI so new types can be added without code changes.
    /// </summary>
    public class PatronType : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ColorHex { get; set; }  // optional badge colour, e.g. "#C0922E"

        [StringLength(500)]
        public string? Description { get; set; }

        public int SortOrder { get; set; } = 0;

        // Navigation
        public ICollection<Patron> Patrons { get; set; } = [];
    }
}