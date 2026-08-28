using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class FormTemplateService : IFormTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DigitalDocumentPlatformDbContext _dbContext;
        private readonly ILogger<FormTemplateService> _logger;

        public FormTemplateService(
            IUnitOfWork unitOfWork,
            DigitalDocumentPlatformDbContext db,
            ILogger<FormTemplateService> logger)
        {
            _unitOfWork = unitOfWork;
            _dbContext = db;
            _logger = logger;
        }

        public async Task<List<FormTemplateBriefResponse>> GetAllActiveAsync()
        {
            _logger.LogInformation("[FormTemplateService.GetAllActiveAsync]");
            var templates = await _unitOfWork.FormTemplates.GetAllActiveAsync();
            return templates.Select(t => new FormTemplateBriefResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Version = t.Version,
                LogoUrl = t.LogoUrl!,
                IsActive = t.IsActive,
                UpdatedAt = t.UpdatedAt,
                UpdatedBy = t.UpdatedBy,
                Translations = t.Translations.Select(MapFormTranslation).ToList(),
                VersionHistories = t.VersionHistories
                    .OrderByDescending(v => v.Version)
                    .Select(v => new FormTemplateVersionHistoryResponse
                    {
                        Id = v.Id,
                        FormTemplateId = v.FormTemplateId,
                        Version = v.Version,
                        Title = v.Title,
                        Description = v.Description,
                        FooterText = v.FooterText,
                        AgreementText = v.AgreementText,
                        QuestionsSnapshot = v.QuestionsSnapshot,
                        UpdatedAt = v.UpdatedAt,
                        UpdatedBy = v.UpdatedBy,
                        ChangeNote = v.ChangeNote
                    }).ToList()
            }).ToList();
        }

        public async Task<FormTemplateResponse?> GetByIdAsync(int id)
        {
            _logger.LogInformation("[FormTemplateService.GetByIdAsync]: id={Id}", id);
            var template = await _unitOfWork.FormTemplates.GetByIdWithQuestionsAsync(id);
            return template == null ? null : MapToResponse(template);
        }

        public async Task<FormTemplateResponse> CreateAsync(CreateFormTemplateRequest request, string createdBy)
        {
            _logger.LogInformation("[FormTemplateService.CreateAsync]: title={Title}, createdBy={CreatedBy}", request.Title, createdBy);
            var entity = new FormTemplate
            {
                Title = request.Title,
                Description = request.Description,
                LogoUrl = request.LogoUrl,
                FooterText = request.FooterText,
                AgreementText = request.AgreementText,
                Version = 1,
                IsActive = true,
                IsDelete = false,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _unitOfWork.FormTemplates.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.FormTemplates.GetByIdWithQuestionsAsync(entity.Id);
            return MapToResponse(created!);
        }

        public async Task<FormTemplateResponse> UpdateAsync(UpdateFormTemplateRequest request, string updatedBy)
        {
            _logger.LogInformation("[FormTemplateService.UpdateAsync]: id={Id}, updatedBy={UpdatedBy}", request.Id, updatedBy);
            var entity = await _unitOfWork.FormTemplates.GetByIdWithQuestionsAsync(request.Id)
                ?? throw new KeyNotFoundException($"FormTemplate {request.Id} not found.");

            // ── Snapshot the current state BEFORE applying changes ────────
            bool contentChanged = entity.Title != request.Title
                || entity.Description != request.Description
                || entity.FooterText != request.FooterText
                || entity.AgreementText != request.AgreementText;

            if (contentChanged)
            {
                var snapshot = new FormTemplateVersionHistory
                {
                    FormTemplateId = entity.Id,
                    Version = entity.Version,
                    Title = entity.Title,
                    Description = entity.Description,
                    LogoUrl = entity.LogoUrl,
                    FooterText = entity.FooterText,
                    AgreementText = entity.AgreementText,
                    QuestionsSnapshot = JsonSerializer.Serialize(
                        entity.Questions.Where(q => !q.IsDelete).OrderBy(q => q.SortOrder)
                            .Select(q => new
                            {
                                q.Id,
                                q.QuestionText,
                                q.QuestionType,
                                q.IsRequired,
                                q.SortOrder,
                                Options = q.Options.Where(o => !o.IsDelete).OrderBy(o => o.SortOrder)
                                    .Select(o => new { o.Id, o.OptionText }).ToList()
                            })),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = updatedBy,
                    ChangeNote = request.ChangeNote
                };
                await _dbContext.FormTemplateVersionHistories.AddAsync(snapshot);
            }

            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.LogoUrl = request.LogoUrl;
            entity.FooterText = request.FooterText;
            entity.AgreementText = request.AgreementText;
            entity.IsActive = request.IsActive;
            if (contentChanged) entity.Version++;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.FormTemplates.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.FormTemplates.GetByIdWithQuestionsAsync(entity.Id);
            return MapToResponse(updated!);
        }

        public async Task DeleteAsync(int id, string deletedBy)
        {
            _logger.LogInformation("[FormTemplateService.DeleteAsync]: id={Id}, deletedBy={DeletedBy}", id, deletedBy);
            var entity = await _unitOfWork.FormTemplates.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"FormTemplate {id} not found.");

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.UpdatedBy = deletedBy;
            entity.UpdatedAt = DateTime.Now;
            _unitOfWork.FormTemplates.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<FormQuestionResponse> AddQuestionAsync(CreateFormQuestionRequest request, string createdBy)
        {
            _logger.LogInformation("[FormTemplateService.AddQuestionAsync]: templateId={TemplateId}", request.FormTemplateId);

            var existing = await _unitOfWork.FormQuestions.GetByTemplateIdAsync(request.FormTemplateId);
            int nextOrder = existing.Count == 0 ? 1 : existing.Max(q => q.SortOrder) + 1;

            var question = new FormQuestion
            {
                FormTemplateId = request.FormTemplateId,
                SortOrder = nextOrder,
                QuestionText = request.QuestionText,
                QuestionType = request.QuestionType ?? QuestionType.TextInput,
                IsRequired = request.IsRequired,
                HasFollowUpText = request.HasFollowUpText,
                FollowUpLabel = request.FollowUpLabel,
                FollowUpTriggerOption = request.FollowUpTriggerOption,
                IsActive = true,
                IsDelete = false,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Options = request.Options.Select((text, idx) => new FormQuestionOption
                {
                    OptionText = text,
                    SortOrder = idx + 1,
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = createdBy,
                    UpdatedBy = createdBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList()
            };
            await _unitOfWork.FormQuestions.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();
            await BumpTemplateVersionAsync(request.FormTemplateId, createdBy);

            var saved = await _unitOfWork.FormQuestions.GetByIdWithOptionsAsync(question.Id);
            return MapQuestionToResponse(saved!);
        }

        public async Task<FormQuestionResponse> UpdateQuestionAsync(UpdateFormQuestionRequest request, string updatedBy)
        {
            _logger.LogInformation("[FormTemplateService.UpdateQuestionAsync]: id={Id}", request.Id);
            var question = await _unitOfWork.FormQuestions.GetByIdWithOptionsAsync(request.Id)
                ?? throw new KeyNotFoundException($"FormQuestion {request.Id} not found.");

            question.QuestionText = request.QuestionText;
            question.QuestionType = request.QuestionType ?? question.QuestionType;//Enum.TryParse<QuestionType>(request.QuestionTypeName, out var parsedType) ? parsedType : question.QuestionType;
            question.IsRequired = request.IsRequired;
            question.HasFollowUpText = request.HasFollowUpText;
            question.FollowUpLabel = request.FollowUpLabel;
            question.FollowUpTriggerOption = request.FollowUpTriggerOption;
            question.UpdatedBy = updatedBy;
            question.UpdatedAt = DateTime.Now;

            // Replace options: soft-delete old, add new
            foreach (var opt in question.Options)
            {
                opt.IsDelete = true;
                opt.UpdatedBy = updatedBy;
                opt.UpdatedAt = DateTime.Now;
            }
            foreach (var (text, idx) in request.Options.Select((t, i) => (t, i)))
            {
                question.Options.Add(new FormQuestionOption
                {
                    FormQuestionId = question.Id,
                    OptionText = text,
                    SortOrder = idx + 1,
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = updatedBy,
                    UpdatedBy = updatedBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }
            _unitOfWork.FormQuestions.Update(question);
            await _unitOfWork.SaveChangesAsync();
            await BumpTemplateVersionAsync(question.FormTemplateId, updatedBy);

            var updated = await _unitOfWork.FormQuestions.GetByIdWithOptionsAsync(question.Id);
            return MapQuestionToResponse(updated!);
        }

        public async Task DeleteQuestionAsync(int questionId, string deletedBy)
        {
            _logger.LogInformation("[FormTemplateService.DeleteQuestionAsync]: id={Id}", questionId);
            var question = await _unitOfWork.FormQuestions.GetByIdAsync(questionId)
                ?? throw new KeyNotFoundException($"FormQuestion {questionId} not found.");

            question.IsDelete = true;
            question.IsActive = false;
            question.UpdatedBy = deletedBy;
            question.UpdatedAt = DateTime.Now;
            _unitOfWork.FormQuestions.Update(question);
            await _unitOfWork.SaveChangesAsync();
            await BumpTemplateVersionAsync(question.FormTemplateId, deletedBy);
        }

        public async Task ReorderQuestionsAsync(ReorderQuestionsRequest request, string updatedBy)
        {
            _logger.LogInformation("[FormTemplateService.ReorderQuestionsAsync]: templateId={TemplateId}", request.FormTemplateId);
            var orderMap = request.QuestionIds
                .Select((id, index) => (questionId: id, newOrder: index + 1))
                .ToList();
            await _unitOfWork.FormQuestions.ReorderAsync(request.FormTemplateId, orderMap);
            await BumpTemplateVersionAsync(request.FormTemplateId, updatedBy);
        }

        // ─── Version histories ────────────────────────────────────────────────

        public async Task<List<FormTemplateVersionHistoryResponse>> GetFormTemplateVersionHistoryAsync(int formTemplateId)
        {
            var list = await _dbContext.FormTemplateVersionHistories
                .AsNoTracking()
                .Where(v => v.FormTemplateId == formTemplateId)
                .OrderByDescending(v => v.Version)
                .ToListAsync();

            return list.Select(v => new FormTemplateVersionHistoryResponse
            {
                Id = v.Id,
                FormTemplateId = v.FormTemplateId,
                Version = v.Version,
                Title = v.Title,
                Description = v.Description,
                FooterText = v.FooterText,
                AgreementText = v.AgreementText,
                QuestionsSnapshot = v.QuestionsSnapshot,
                UpdatedAt = v.UpdatedAt,
                UpdatedBy = v.UpdatedBy,
                ChangeNote = v.ChangeNote
            }).ToList();
        }

        // ─── Template translations ────────────────────────────────────────────

        public async Task<FormTemplateTranslationResponse> UpsertFormTemplateTranslationAsync(UpsertFormTemplateTranslationRequest request, string updatedBy)
        {
            _logger.LogInformation("[FormTemplateService.UpsertFormTemplateTranslationAsync]: templateId={Id}, lang={Lang}",
                request.FormTemplateId, request.LanguageCode);

            // Load template with questions and options to validate incoming translation data
            var template = await _dbContext.FormTemplates
                .Include(t => t.Questions.Where(q => !q.IsDelete))
                    .ThenInclude(q => q.Options.Where(o => !o.IsDelete))
                .FirstOrDefaultAsync(t => t.Id == request.FormTemplateId && !t.IsDelete)
                ?? throw new KeyNotFoundException($"FormTemplate {request.FormTemplateId} not found.");

            // Build validation lookup: questionId → set of valid optionIds for that question
            var validQuestions = template.Questions.ToDictionary(
                q => q.Id,
                q => q.Options.Select(o => o.Id).ToHashSet());

            // Validate, filter and sanitize QuestionsTranslation JSON before persisting
            string? sanitizedJson = null;
            if (!string.IsNullOrWhiteSpace(request.QuestionsTranslation))
            {
                List<QuestionTranslationJson> inputItems;
                try
                {
                    inputItems = JsonSerializer.Deserialize<List<QuestionTranslationJson>>(
                        request.QuestionsTranslation,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? [];
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "[UpsertFormTemplateTranslationAsync] Invalid JSON in QuestionsTranslation for templateId={Id}", request.FormTemplateId);
                    throw new ArgumentException("QuestionsTranslation contains invalid JSON format.", nameof(request));
                }

                var validated = new List<QuestionTranslationJson>();
                foreach (var qt in inputItems)
                {
                    // Only accept questionIds that actually exist in this template
                    if (!validQuestions.TryGetValue(qt.QuestionId, out var validOptionIds))
                    {
                        _logger.LogWarning("[UpsertFormTemplateTranslationAsync] QuestionId {QId} does not exist in template {TId} — skipping.",
                            qt.QuestionId, request.FormTemplateId);
                        continue;
                    }

                    // Only accept optionIds that actually belong to this question
                    var validatedOptions = new List<OptionTranslationJson>();
                    if (qt.Options != null)
                    {
                        foreach (var ot in qt.Options)
                        {
                            if (!validOptionIds.Contains(ot.OptionId))
                            {
                                _logger.LogWarning("[UpsertFormTemplateTranslationAsync] OptionId {OId} does not exist in question {QId} — skipping.",
                                    ot.OptionId, qt.QuestionId);
                                continue;
                            }
                            validatedOptions.Add(new OptionTranslationJson { OptionId = ot.OptionId, OptionText = ot.OptionText });
                        }
                    }

                    validated.Add(new QuestionTranslationJson
                    {
                        QuestionId = qt.QuestionId,
                        QuestionText = qt.QuestionText,
                        HasFollowUpText = qt.HasFollowUpText,
                        FollowUpLabel = qt.FollowUpLabel,
                        Options = validatedOptions
                    });
                }

                sanitizedJson = JsonSerializer.Serialize(validated,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }

            var existing = await _dbContext.FormTemplateTranslations
                .FirstOrDefaultAsync(t =>
                    t.FormTemplateId == request.FormTemplateId &&
                    t.LanguageCode == request.LanguageCode);

            if (existing == null)
            {
                existing = new FormTemplateTranslation
                {
                    FormTemplateId = request.FormTemplateId,
                    LanguageCode = request.LanguageCode,
                };
                await _dbContext.FormTemplateTranslations.AddAsync(existing);
            }

            existing.Title = request.Title;
            existing.Description = request.Description;
            existing.FooterText = request.FooterText;
            existing.AgreementText = request.AgreementText;
            existing.QuestionsTranslation = sanitizedJson;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = updatedBy;

            await _dbContext.SaveChangesAsync();
            return MapFormTranslation(existing);
        }

        public async Task<List<FormTemplateTranslationResponse>> GetFormTemplateTranslationsAsync(int formTemplateId)
        {
            var list = await _dbContext.FormTemplateTranslations
                .AsNoTracking()
                .Where(t => t.FormTemplateId == formTemplateId)
                .OrderBy(t => t.LanguageCode)
                .ToListAsync();

            return list.Select(MapFormTranslation).ToList();
        }

        // ─── Helpers ────────────────────────────────────────────────────────────

        private async Task BumpTemplateVersionAsync(int templateId, string updatedBy)
        {
            var template = await _unitOfWork.FormTemplates.GetByIdAsync(templateId);
            if (template == null) return;
            template.Version++;
            template.UpdatedBy = updatedBy;
            template.UpdatedAt = DateTime.Now;
            _unitOfWork.FormTemplates.Update(template);
            await _unitOfWork.SaveChangesAsync();
        }

        private static FormTemplateResponse MapToResponse(FormTemplate t) => new()
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            LogoUrl = t.LogoUrl,
            FooterText = t.FooterText,
            AgreementText = t.AgreementText,
            Version = t.Version,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            Questions = t.Questions
                .Where(q => !q.IsDelete)
                .OrderBy(q => q.SortOrder)
                .Select(MapQuestionToResponse)
                .ToList(),
            Translations = t.Translations.Select(MapFormTranslation).ToList(),
            VersionHistories = t.VersionHistories
                .OrderByDescending(v => v.Version)
                .Select(v => new FormTemplateVersionHistoryResponse
                {
                    Id = v.Id,
                    FormTemplateId = v.FormTemplateId,
                    Version = v.Version,
                    Title = v.Title,
                    Description = v.Description,
                    FooterText = v.FooterText,
                    AgreementText = v.AgreementText,
                    QuestionsSnapshot = v.QuestionsSnapshot,
                    UpdatedAt = v.UpdatedAt,
                    UpdatedBy = v.UpdatedBy,
                    ChangeNote = v.ChangeNote
                }).ToList()
        };

        private static FormQuestionResponse MapQuestionToResponse(FormQuestion q) => new()
        {
            Id = q.Id,
            FormTemplateId = q.FormTemplateId,
            SortOrder = q.SortOrder,
            QuestionText = q.QuestionText,
            QuestionType = q.QuestionType,
            QuestionTypeName = q.QuestionType.ToString(),
            IsRequired = q.IsRequired,
            HasFollowUpText = q.HasFollowUpText,
            FollowUpLabel = q.FollowUpLabel,
            FollowUpTriggerOption = q.FollowUpTriggerOption,
            Options = q.Options
                .Where(o => !o.IsDelete)
                .OrderBy(o => o.SortOrder)
                .Select(o => new FormQuestionOptionResponse
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    SortOrder = o.SortOrder
                }).ToList()
        };

        // ─── Private DTOs for QuestionsTranslation JSON parsing ──────────────

        private sealed class QuestionTranslationJson
        {
            public int QuestionId { get; set; }
            public string? QuestionText { get; set; }
            public bool HasFollowUpText { get; set; }
            public string? FollowUpLabel { get; set; }
            public List<OptionTranslationJson>? Options { get; set; }
        }

        private sealed class OptionTranslationJson
        {
            public int OptionId { get; set; }
            public string? OptionText { get; set; }
        }

        private static FormTemplateTranslationResponse MapFormTranslation(FormTemplateTranslation t) => new()
        {
            Id = t.Id,
            FormTemplateId = t.FormTemplateId,
            LanguageCode = t.LanguageCode,
            Title = t.Title,
            Description = t.Description,
            FooterText = t.FooterText,
            AgreementText = t.AgreementText,
            QuestionsTranslation = t.QuestionsTranslation,
            UpdatedAt = t.UpdatedAt,
            UpdatedBy = t.UpdatedBy
        };
    }
}