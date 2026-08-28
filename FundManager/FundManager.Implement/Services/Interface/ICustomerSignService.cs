using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface ICustomerSignService
    {
        /// <summary>Get a form template with all questions/options for patron to fill in.</summary>
        Task<FormTemplateResponse?> GetFormTemplateAsync(int formTemplateId, string language);

        /// <summary>Get a document template HTML for patron to read before signing.</summary>
        Task<DocumentTemplateResponse?> GetDocumentTemplateAsync(int documentTemplateId, string language);

        /// <summary>
        /// Submit full spa session: create Patron, save FormSubmission + answers,
        /// generate PDFs (daily folder), save PatronSignature records.
        /// </summary>
        Task<CustomerSessionSubmitResponse> SubmitSignatureSessionAsync(CustomerSessionSubmitRequest request);

        // ─── Admin: Signed customers list ────────────────────────────────────

        /// <summary>Server-paged list of patrons who have signed, with their documents.</summary>
        Task<SignedCustomerListResponse> GetSignedCustomersAsync(SignedCustomerListRequest request);

        /// <summary>Detail of one patron + all their signed documents.</summary>
        Task<SignedCustomerRow?> GetSignedCustomerDetailAsync(int patronId);

        /// <summary>
        /// Load the last session data of a patron so the iPad can prefill the form
        /// (used after RequestDocumentSignatureAsync sends the SignalR duplicate request).
        /// </summary>
        Task<SessionPrefillResponse?> GetSessionPrefillAsync(int patronId, string language);

    }
}
