using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// Multilingual translation for a DocumentTemplate.
    /// The parent DocumentTemplate holds the default language HTML content.
    /// Each row here holds the same document's content in a different language.
    /// 
    /// Pattern: Translation Table (locale table).
    /// — One row per (DocumentTemplateId, LanguageCode) pair
    /// — Falls back to parent content when no translation row exists
    /// </summary>
    public class DocumentTemplateTranslation : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(DocumentTemplate))]
        public int DocumentTemplateId { get; set; }
        public DocumentTemplate DocumentTemplate { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>Full HTML content in this language.</summary>
        public string Content { get; set; } = string.Empty;
    }
}
