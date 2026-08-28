using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// A configurable document template (e.g. PDP consent, HTP, Terms, Spa Acknowledgement).
    /// Multiple template types are supported via the <see cref="DocumentType"/> enum.
    /// Each edit increments <see cref="Version"/> so history is preserved.
    /// </summary>
    public class DocumentTemplate : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>Human-readable title, e.g. "Personal Data Processing Consent".</summary>
        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        /// <summary>Category of this document.</summary>
        public DocumentType DocumentType { get; set; } = DocumentType.Other;

        /// <summary>
        /// Full HTML content of the document.
        /// Supports placeholders like {{PatronName}}, {{Date}} for runtime substitution.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>Short description shown in the admin list.</summary>
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Monotonically increasing version counter.
        /// Incremented every time an admin publishes changes.
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>Optional: which Outlet this document belongs to (null = all outlets).</summary>
        [ForeignKey(nameof(Outlet))]
        public int? OutletId { get; set; }
        public Outlet? Outlet { get; set; }

        // Navigation – workflow steps that reference this document
        public ICollection<WorkflowStep> WorkflowSteps { get; set; } = [];
        public ICollection<DocumentTemplateVersionHistory> VersionHistories { get; set; } = [];
        public ICollection<DocumentTemplateTranslation> Translations { get; set; } = [];
    }
}