using FundManager.Common.BaseEntity;
using FundManager.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundManager.DataAccess.EntityModels
{
    /// <summary>
    /// One patron's completed response to a FormTemplate.
    /// Each submit creates a new record — preserving full history per patron.
    /// </summary>
    public class FormSubmission : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(FormTemplate))]
        public int FormTemplateId { get; set; }
        public FormTemplate FormTemplate { get; set; } = null!;

        /// <summary>The Version value of the template at the time of submission.</summary>
        public int TemplateVersion { get; set; }

        /// <summary>
        /// IETF language code used by the patron when filling the form (e.g. "en", "vi", "ko").
        /// Used to look up the correct QuestionsTranslation when rendering the submission HTML.
        /// </summary>
        [StringLength(10)]
        public string LanguageCode { get; set; } = CommonConstants.DefaultLanguage;

        /// <summary>Optional: which PatronDevice submitted the form.</summary>
        public int? PatronDeviceId { get; set; }
        public PatronDevice? PatronDevice { get; set; }

        /// <summary>Optional: link to the SignatureSession this form belongs to.</summary>
        public int? SignatureSessionId { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<FormSubmissionAnswer> Answers { get; set; } = [];
    }
}
