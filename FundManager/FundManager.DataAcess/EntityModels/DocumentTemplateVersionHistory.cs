using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// Immutable snapshot of a DocumentTemplate taken every time its content changes.
    /// </summary>
    public class DocumentTemplateVersionHistory : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(DocumentTemplate))]
        public int DocumentTemplateId { get; set; }
        public DocumentTemplate DocumentTemplate { get; set; } = null!;

        /// <summary>The version number that was in effect before this change.</summary>
        public int Version { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>Full HTML content at this version.</summary>
        public string Content { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ChangeNote { get; set; }
    }
}