using FundManager.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundManager.DataAccess.EntityModels
{
    /// <summary>
    /// A single question within a FormTemplate.
    /// </summary>
    public class FormQuestion : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(FormTemplate))]
        public int FormTemplateId { get; set; }
        public FormTemplate FormTemplate { get; set; } = null!;

        /// <summary>Display order (1-based). Admin can reorder.</summary>
        public int SortOrder { get; set; }

        /// <summary>The visible question text. Supports inline HTML bold tags.</summary>
        [Required]
        [StringLength(1000)]
        public string QuestionText { get; set; } = string.Empty;

        public QuestionType QuestionType { get; set; } = QuestionType.SingleChoice;

        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// When true, a text input follow-up is shown after a specific option is selected.
        /// </summary>
        public bool HasFollowUpText { get; set; } = false;

        /// <summary>
        /// The label for the follow-up text field, e.g. "If yes, please briefly describe:".
        /// Only relevant when HasFollowUpText = true.
        /// </summary>
        [StringLength(500)]
        public string? FollowUpLabel { get; set; }

        /// <summary>
        /// The option value that triggers the follow-up text field, e.g. "Yes".
        /// Only relevant when HasFollowUpText = true.
        /// </summary>
        [StringLength(500)]
        public string? FollowUpTriggerOption { get; set; }

        // Navigation
        public ICollection<FormQuestionOption> Options { get; set; } = [];
        public ICollection<FormSubmissionAnswer> Answers { get; set; } = [];
    }
}