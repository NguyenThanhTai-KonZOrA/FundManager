using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;
using Microsoft.Extensions.Logging;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class FormSubmissionService : IFormSubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FormSubmissionService> _logger;

        public FormSubmissionService(IUnitOfWork unitOfWork, ILogger<FormSubmissionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<FormSubmissionResponse> SubmitAsync(SubmitFormRequest request, string createdBy)
        {
            _logger.LogInformation("[FormSubmissionService.SubmitAsync]: templateId={TemplateId}", request.FormTemplateId);

            var template = await _unitOfWork.FormTemplates.GetByIdAsync(request.FormTemplateId)
                ?? throw new KeyNotFoundException($"FormTemplate {request.FormTemplateId} not found.");

            var submission = new FormSubmission
            {
                FormTemplateId = request.FormTemplateId,
                TemplateVersion = template.Version,
                PatronDeviceId = request.PatronDeviceId,
                SignatureSessionId = request.SignatureSessionId,
                SubmittedAt = DateTime.Now,
                IsActive = true,
                IsDelete = false,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Answers = request.Answers.Select(a => new FormSubmissionAnswer
                {
                    FormQuestionId = a.FormQuestionId,
                    AnswerValue = a.AnswerValue,
                    FollowUpText = a.FollowUpText,
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = createdBy,
                    UpdatedBy = createdBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList()
            };

            await _unitOfWork.FormSubmissions.AddAsync(submission);
            await _unitOfWork.SaveChangesAsync();

            var saved = await _unitOfWork.FormSubmissions.GetByIdWithAnswersAsync(submission.Id);
            return MapToResponse(saved!);
        }

        public async Task<FormSubmissionResponse?> GetByIdAsync(int id)
        {
            _logger.LogInformation("[FormSubmissionService.GetByIdAsync]: id={Id}", id);
            var submission = await _unitOfWork.FormSubmissions.GetByIdWithAnswersAsync(id);
            return submission == null ? null : MapToResponse(submission);
        }

        public async Task<List<FormSubmissionBriefResponse>> GetByPatronDeviceIdAsync(int patronDeviceId)
        {
            _logger.LogInformation("[FormSubmissionService.GetByPatronDeviceIdAsync]: patronDeviceId={PatronDeviceId}", patronDeviceId);
            var submissions = await _unitOfWork.FormSubmissions.GetByPatronDeviceIdAsync(patronDeviceId);
            return submissions.Select(MapToBriefResponse).ToList();
        }

        public async Task<List<FormSubmissionBriefResponse>> GetByTemplateIdAsync(int templateId)
        {
            _logger.LogInformation("[FormSubmissionService.GetByTemplateIdAsync]: templateId={TemplateId}", templateId);
            var submissions = await _unitOfWork.FormSubmissions.GetByTemplateIdAsync(templateId);
            return submissions.Select(MapToBriefResponse).ToList();
        }

        private static FormSubmissionResponse MapToResponse(FormSubmission s) => new()
        {
            Id = s.Id,
            FormTemplateId = s.FormTemplateId,
            FormTemplateTitle = s.FormTemplate?.Title ?? string.Empty,
            TemplateVersion = s.TemplateVersion,
            PatronDeviceId = s.PatronDeviceId,
            SignatureSessionId = s.SignatureSessionId,
            SubmittedAt = s.SubmittedAt,
            Answers = s.Answers
                .Where(a => !a.IsDelete)
                .Select(a => new FormSubmissionAnswerResponse
                {
                    Id = a.Id,
                    FormQuestionId = a.FormQuestionId,
                    QuestionText = a.FormQuestion?.QuestionText ?? string.Empty,
                    AnswerValue = a.AnswerValue,
                    FollowUpText = a.FollowUpText
                }).ToList()
        };

        private static FormSubmissionBriefResponse MapToBriefResponse(FormSubmission s) => new()
        {
            Id = s.Id,
            FormTemplateId = s.FormTemplateId,
            FormTemplateTitle = s.FormTemplate?.Title ?? string.Empty,
            TemplateVersion = s.TemplateVersion,
            PatronDeviceId = s.PatronDeviceId,
            SubmittedAt = s.SubmittedAt
        };
    }
}