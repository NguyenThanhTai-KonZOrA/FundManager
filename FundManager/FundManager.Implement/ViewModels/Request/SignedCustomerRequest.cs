using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class SignedCustomerListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;

        public string? SearchTerm { get; set; }   // name / room / ID search
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? OutletId { get; set; }
        public int? PatronTypeId { get; set; }
        public string? CustomerType { get; set; } // InHouse | WalkIn
    }

    // ─── Translation CRUD requests ────────────────────────────────────────────

    public class UpsertFormTemplateTranslationRequest
    {
        [Required]
        public int FormTemplateId { get; set; }

        [Required, StringLength(10)]
        public string LanguageCode { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public string? FooterText { get; set; }
        public string? AgreementText { get; set; }

        /// <summary>
        /// JSON array: [{ "questionId":1, "questionText":"…", "options":[{"optionId":1,"optionText":"…"}] }]
        /// </summary>
        public string? QuestionsTranslation { get; set; }
    }

    public class UpsertDocumentTemplateTranslationRequest
    {
        [Required]
        public int DocumentTemplateId { get; set; }

        [Required, StringLength(10)]
        public string LanguageCode { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}
