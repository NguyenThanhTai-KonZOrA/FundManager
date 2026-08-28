using DigitalDocumentPlatform.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class CustomerSessionSubmitRequest
    {
        // ─── Customer info ────────────────────────────────────────────────────
        /// <summary>InHouse | WalkIn</summary>
        [Required]
        public string CustomerType { get; set; } = string.Empty;

        public string? RoomNumber { get; set; }
        public string? GuestName { get; set; }   // selected sharer name for InHouse

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? IdPassport { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public string? Email { get; set; }

        [Required]
        public string Language { get; set; } = CommonConstants.DefaultLanguage;

        // ─── Workflow context ─────────────────────────────────────────────────
        public int WorkflowId { get; set; }
        public int? PatronDeviceId { get; set; }
        public string? PatronDeviceName { get; set; }

        // ─── Form answers (FillForm step) ─────────────────────────────────────
        public int? FormTemplateId { get; set; }
        public List<FormAnswerItem> Answers { get; set; } = [];

        // ─── Signature ────────────────────────────────────────────────────────
        /// <summary>Base-64 data URL of the patron's drawn signature.</summary>
        [Required]
        public string SignatureDataUrl { get; set; } = string.Empty;

        // ─── Document template acknowledged ───────────────────────────────────
        public int? DocumentTemplateId { get; set; }
        public int OutletId { get; set; }
        public int? PatronId { get; set; }
        public int? PlayerId { get; set; }
        public int? SessionId { get; set; }
    }

    public class FormAnswerItem
    {
        [Required]
        public int FormQuestionId { get; set; }

        /// <summary>JSON array string for MultipleChoice; plain string otherwise.</summary>
        public string AnswerValue { get; set; } = string.Empty;

        public string? FollowUpText { get; set; }
    }
}
