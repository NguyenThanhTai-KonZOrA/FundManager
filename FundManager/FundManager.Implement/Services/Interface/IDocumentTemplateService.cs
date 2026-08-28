using FundManager.DataAccess.EntityModels;
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface IDocumentTemplateService
    {
        // ─── Document templates ─────────────────────────────────────────────────
        Task<List<DocumentTemplateBriefResponse>> GetListAsync();
        Task<DocumentTemplateResponse> GetByIdAsync(int id);
        Task<List<DocumentTemplateBriefResponse>> GetByTypeAsync(DocumentType documentType);
        Task<List<DocumentTemplateBriefResponse>> GetByOutletAsync(int outletId);
        Task<DocumentTemplateResponse> CreateAsync(CreateDocumentTemplateRequest request, string createdBy);
        Task<DocumentTemplateResponse> UpdateAsync(UpdateDocumentTemplateRequest request, string updatedBy);
        Task DeleteAsync(int id, string deletedBy);

        // ─── Version histories ────────────────────────────────────────────────
        Task<List<DocumentTemplateVersionHistoryResponse>> GetDocumentTemplateVersionHistoryAsync(int documentTemplateId);

        // ─── Template translations ────────────────────────────────────────────
        Task<DocumentTemplateTranslationResponse> UpsertDocumentTemplateTranslationAsync(
            UpsertDocumentTemplateTranslationRequest request, string updatedBy);

        Task<List<DocumentTemplateTranslationResponse>> GetDocumentTemplateTranslationsAsync(int documentTemplateId);
    }
}