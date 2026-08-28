using FundManager.DataAccess.EntityModels;
using System.ComponentModel.DataAnnotations;

namespace FundManager.Implement.ViewModels.Request
{
    // ─── FormTemplate ────────────────────────────────────────────────────────
    public class CreateFormTemplateRequest
    {
        //[Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        public string? FooterText { get; set; }
        
        public string? AgreementText { get; set; }
    }

    public class UpdateFormTemplateRequest
    {
        //[Required]
        public int Id { get; set; }

        //[Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        //[StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        //[StringLength(500)]
        public string? LogoUrl { get; set; }

        public string? FooterText { get; set; }
        public string? AgreementText { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>Optional note explaining what changed in this version.</summary>
        //[StringLength(1000)]
        public string? ChangeNote { get; set; }
    }

    // ─── FormQuestion ─────────────────────────────────────────────────────────
    public class CreateFormQuestionRequest
    {
        [Required]
        public int FormTemplateId { get; set; }

        [Required, StringLength(1000)]
        public string QuestionText { get; set; } = string.Empty;

        public QuestionType? QuestionType { get; set; }

        public bool IsRequired { get; set; } = false;

        public bool HasFollowUpText { get; set; } = false;

        [StringLength(500)]
        public string? FollowUpLabel { get; set; }

        [StringLength(100)]
        public string? FollowUpTriggerOption { get; set; }

        /// <summary>Option texts for this question (in display order).</summary>
        public List<string> Options { get; set; } = [];
    }

    public class UpdateFormQuestionRequest
    {
        [Required]
        public int Id { get; set; }

        //[Required, StringLength(1000)]
        public string QuestionText { get; set; } = string.Empty;

        public QuestionType? QuestionType { get; set; }
        public string? QuestionTypeName { get; set; }

        public bool IsRequired { get; set; } = false;

        public bool HasFollowUpText { get; set; } = false;

        [StringLength(500)]
        public string? FollowUpLabel { get; set; }

        [StringLength(100)]
        public string? FollowUpTriggerOption { get; set; }

        /// <summary>Full replacement list of option texts.</summary>
        public List<string> Options { get; set; } = [];
    }

    public class ReorderQuestionsRequest
    {
        [Required]
        public int FormTemplateId { get; set; }

        /// <summary>Ordered list of question IDs reflecting the new display order.</summary>
        public List<int> QuestionIds { get; set; } = [];
    }
}
