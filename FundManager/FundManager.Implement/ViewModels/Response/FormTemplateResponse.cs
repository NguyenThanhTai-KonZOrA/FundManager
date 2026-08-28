using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.ViewModels.Response
{
    // ─── FormTemplate ─────────────────────────────────────────────────────────
    public class FormTemplateResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? FooterText { get; set; }
        public string? AgreementText { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public List<FormQuestionResponse> Questions { get; set; } = [];
        public List<FormTemplateTranslationResponse> Translations { get; set; } = [];
        public List<FormTemplateVersionHistoryResponse> VersionHistories { get; set; } = [];
    }

    public class FormTemplateBriefResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public List<FormTemplateTranslationResponse> Translations { get; set; } = [];
        public List<FormTemplateVersionHistoryResponse> VersionHistories { get; set; } = [];
    }

    // ─── FormQuestion ─────────────────────────────────────────────────────────
    public class FormQuestionResponse
    {
        public int Id { get; set; }
        public int FormTemplateId { get; set; }
        public int SortOrder { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public string? QuestionTypeName { get; set; }
        public bool IsRequired { get; set; }
        public bool HasFollowUpText { get; set; }
        public string? FollowUpLabel { get; set; }
        public string? FollowUpTriggerOption { get; set; }
        public List<FormQuestionOptionResponse> Options { get; set; } = [];
    }

    public class FormQuestionOptionResponse
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
