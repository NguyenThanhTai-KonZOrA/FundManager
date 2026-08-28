using System.ComponentModel.DataAnnotations;

namespace FundManager.Implement.ViewModels.Request
{
    public class SubmitFormRequest
    {
        [Required]
        public int FormTemplateId { get; set; }

        public int? PatronDeviceId { get; set; }
        public int? SignatureSessionId { get; set; }

        [Required]
        public List<SubmitAnswerItem> Answers { get; set; } = [];
    }

    public class SubmitAnswerItem
    {
        [Required]
        public int FormQuestionId { get; set; }

        /// <summary>
        /// JSON string for MultipleChoice (e.g. ["Tired","Stressed"]),
        /// plain text for SingleChoice/TextInput.
        /// </summary>
        public string AnswerValue { get; set; } = string.Empty;

        public string? FollowUpText { get; set; }
    }
}
