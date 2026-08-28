using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// Immutable snapshot of a FormTemplate taken every time its content changes.
    /// Never soft-deleted — history must be preserved indefinitely.
    /// </summary>
    public class FormTemplateVersionHistory : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(FormTemplate))]
        public int FormTemplateId { get; set; }
        public FormTemplate FormTemplate { get; set; } = null!;

        /// <summary>The version number that was in effect before this change.</summary>
        public int Version { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LogoUrl { get; set; }
        [StringLength(2000)]
        public string? FooterText { get; set; }
        [StringLength(1000)]
        public string? AgreementText { get; set; }

        /// <summary>Serialised JSON snapshot of all questions at this version.</summary>
        public string QuestionsSnapshot { get; set; } = "[]";

        /// <summary>Free-text note explaining why this version was published.</summary>
        [StringLength(1000)]
        public string? ChangeNote { get; set; }
    }
}