using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// Stores the patron's answer to a single FormQuestion within a FormSubmission.
    /// For MultipleChoice: AnswerValue is a JSON array of selected option texts.
    /// For SingleChoice / TextInput: AnswerValue is a plain string.
    /// FollowUpText holds the free-text follow-up if applicable.
    /// </summary>
    public class FormSubmissionAnswer : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(FormSubmission))]
        public int FormSubmissionId { get; set; }
        public FormSubmission FormSubmission { get; set; } = null!;

        [ForeignKey(nameof(FormQuestion))]
        public int FormQuestionId { get; set; }
        public FormQuestion FormQuestion { get; set; } = null!;

        /// <summary>
        /// JSON-encoded answer value.
        /// Single string for SingleChoice/TextInput; JSON array for MultipleChoice.
        /// </summary>
        [StringLength(2000)]
        public string AnswerValue { get; set; } = string.Empty;

        /// <summary>Optional follow-up text entered by the patron (e.g. "Washing, masked").</summary>
        [StringLength(1000)]
        public string? FollowUpText { get; set; }
    }
}