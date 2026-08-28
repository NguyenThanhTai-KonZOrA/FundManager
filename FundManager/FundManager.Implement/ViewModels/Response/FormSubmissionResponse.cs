namespace DigitalDocumentPlatform.Implement.ViewModels.Response
{
    public class FormSubmissionResponse
    {
        public int Id { get; set; }
        public int FormTemplateId { get; set; }
        public string FormTemplateTitle { get; set; } = string.Empty;
        public int TemplateVersion { get; set; }
        public int? PatronDeviceId { get; set; }
        public int? SignatureSessionId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<FormSubmissionAnswerResponse> Answers { get; set; } = [];
    }

    public class FormSubmissionAnswerResponse
    {
        public int Id { get; set; }
        public int FormQuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string AnswerValue { get; set; } = string.Empty;
        public string? FollowUpText { get; set; }
    }

    public class FormSubmissionBriefResponse
    {
        public int Id { get; set; }
        public int FormTemplateId { get; set; }
        public string FormTemplateTitle { get; set; } = string.Empty;
        public int TemplateVersion { get; set; }
        public int? PatronDeviceId { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
