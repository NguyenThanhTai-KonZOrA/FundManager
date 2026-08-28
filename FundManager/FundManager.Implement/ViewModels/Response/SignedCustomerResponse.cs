using FundManager.Common.Constants;

namespace FundManager.Implement.ViewModels.Response
{
    // ─── Signed Customers List (Admin page) ───────────────────────────────────

    public class SignedCustomerListResponse
    {
        public int TotalRecords { get; set; }
        public List<SignedCustomerRow> Data { get; set; } = [];
    }

    public class SignedCustomerRow
    {
        public int Id { get; set; }                     // Patron.Id
        public string DisplayId { get; set; } = string.Empty; // e.g. "#G-2481"
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PatronType { get; set; }
        public string? PatronTypeColor { get; set; }
        public string? RoomNumber { get; set; }
        public string? Language { get; set; }
        public string? CustomerType { get; set; }
        public int? OutletId { get; set; }
        public string? OutletName { get; set; }
        public DateTime SignedAt { get; set; }
        public string? SignedBy { get; set; }
        public string? SignedByDevice { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public List<SignedDocumentRow> Documents { get; set; } = [];
    }

    public class SignedDocumentRow
    {
        public int PatronSignatureId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? FileUrl { get; set; }             // relative path → served via static files
        public DateTime SignedAt { get; set; }
        public string Status { get; set; } = SignatureSessionStatus.Signed;
        public string? SignedByDevice { get; set; }
    }

    // ─── Session Prefill (iPad reload after duplicate) ────────────────────────

    public class SessionPrefillResponse
    {
        public int PatronId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RoomNumber { get; set; }
        public string? Language { get; set; }
        public string? CustomerType { get; set; }
        public string? Nationality { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int? PlayerId { get; set; }
        public string? IdPassport { get; set; }         // stored in Patron.Address
        public List<PrefillAnswer> PreviousAnswers { get; set; } = [];
    }

    public class PrefillAnswer
    {
        public int FormQuestionId { get; set; }
        public string? AnswerValue { get; set; }
        public string? FollowUpText { get; set; }
    }

    // ─── Template Translation responses ───────────────────────────────────────

    public class FormTemplateTranslationResponse
    {
        public int Id { get; set; }
        public int FormTemplateId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? FooterText { get; set; }
        public string? AgreementText { get; set; }
        public string? QuestionsTranslation { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    public class DocumentTemplateTranslationResponse
    {
        public int Id { get; set; }
        public int DocumentTemplateId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    // ─── Version history responses ────────────────────────────────────────────

    public class FormTemplateVersionHistoryResponse
    {
        public int Id { get; set; }
        public int FormTemplateId { get; set; }
        public int Version { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? FooterText { get; set; }
        public string? AgreementText { get; set; }
        public string QuestionsSnapshot { get; set; } = "[]";
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public string? ChangeNote { get; set; }
    }

    public class DocumentTemplateVersionHistoryResponse
    {
        public int Id { get; set; }
        public int DocumentTemplateId { get; set; }
        public int Version { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public string? ChangeNote { get; set; }
    }
}
