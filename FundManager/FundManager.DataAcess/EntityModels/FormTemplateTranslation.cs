using FundManager.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundManager.DataAccess.EntityModels
{
    /// <summary>
    /// Multilingual translation for a FormTemplate.
    /// Best practice: the parent FormTemplate stores the default (fallback) language content;
    /// this table stores per-language overrides for Title, Description, FooterText, and a JSON
    /// snapshot of translated question texts.
    /// 
    /// Pattern used: "Translation Table" (also called "locale table").
    /// — Lightweight: no duplication of questions/options rows
    /// — Fallback: if a translation row is missing, the consumer falls back to the parent record
    /// — Version-safe: translations reference the master FormTemplate (not a specific version)
    ///   and are expected to be updated whenever question text changes
    /// </summary>
    public class FormTemplateTranslation : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(FormTemplate))]
        public int FormTemplateId { get; set; }
        public FormTemplate FormTemplate { get; set; } = null!;

        /// <summary>IETF language code, e.g. "vi", "ko", "zh", "ja".</summary>
        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; } = string.Empty;
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(2000)]
        public string? FooterText { get; set; }
        [StringLength(1000)]
        public string? AgreementText { get; set; }

        /// <summary>
        /// JSON array matching the question order of the parent template.
        /// Each element: { "questionId": 1, "questionText": "…", "options": [ { "optionId": 1, "optionText": "…" } ] }
        /// </summary>
        public string? QuestionsTranslation { get; set; }
    }
}