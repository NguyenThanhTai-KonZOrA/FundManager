using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface IFormTemplateService
    {
        // ─── Form templates ─────────────────────────────────────────────────────
        Task<List<FormTemplateBriefResponse>> GetAllActiveAsync();
        Task<FormTemplateResponse?> GetByIdAsync(int id);
        Task<FormTemplateResponse> CreateAsync(CreateFormTemplateRequest request, string createdBy);
        Task<FormTemplateResponse> UpdateAsync(UpdateFormTemplateRequest request, string updatedBy);
        Task DeleteAsync(int id, string deletedBy);

        // ─── Questions ─────────────────────────────────────────────────────────
        Task<FormQuestionResponse> AddQuestionAsync(CreateFormQuestionRequest request, string createdBy);
        Task<FormQuestionResponse> UpdateQuestionAsync(UpdateFormQuestionRequest request, string updatedBy);
        Task DeleteQuestionAsync(int questionId, string deletedBy);
        Task ReorderQuestionsAsync(ReorderQuestionsRequest request, string updatedBy);

        // ─── Template translations ────────────────────────────────────────────
        Task<FormTemplateTranslationResponse> UpsertFormTemplateTranslationAsync(
            UpsertFormTemplateTranslationRequest request, string updatedBy);

        Task<List<FormTemplateTranslationResponse>> GetFormTemplateTranslationsAsync(int formTemplateId);

        // ─── Version histories ────────────────────────────────────────────────
        Task<List<FormTemplateVersionHistoryResponse>> GetFormTemplateVersionHistoryAsync(int formTemplateId);
    }
}