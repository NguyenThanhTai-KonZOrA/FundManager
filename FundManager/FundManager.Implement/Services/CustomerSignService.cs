using DigitalDocumentPlatform.BackgroundQueue;
using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.Common.Enum;
using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using DigitalDocumentPlatform.Implement.ViewModels;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class CustomerSignService : ICustomerSignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatronRepository _patronRepository;
        private readonly IPatronSignatureRepository _patronSignatureRepository;
        private readonly IPdfConverterService _pdfConverter;
        private readonly DigitalDocumentPlatformDbContext _dbContext;
        private readonly IBackgroundTaskQueue _backgroundQueue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CustomerSignService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationSettingsService _appSettingsService;
        // Relative to IWebHostEnvironment.ContentRootPath  →  App/Documents/yyyyMMdd/
        private const string DocumentsFolder = "Documents";

        public CustomerSignService(
            IUnitOfWork unitOfWork,
            IPatronRepository patronRepository,
            IPatronSignatureRepository patronSignatureRepository,
            IPdfConverterService pdfConverter,
            DigitalDocumentPlatformDbContext db,
            IBackgroundTaskQueue backgroundQueue,
            IServiceScopeFactory scopeFactory,
            ILogger<CustomerSignService> logger,
            IWebHostEnvironment env,
            IApplicationSettingsService appSettingsService)
        {
            _unitOfWork = unitOfWork;
            _patronRepository = patronRepository;
            _patronSignatureRepository = patronSignatureRepository;
            _pdfConverter = pdfConverter;
            _appSettingsService = appSettingsService;
            _dbContext = db;
            _backgroundQueue = backgroundQueue;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _env = env;
        }

        // ─── GET form template ────────────────────────────────────────────────

        public async Task<FormTemplateResponse?> GetFormTemplateAsync(int formTemplateId, string language)
        {
            _logger.LogInformation("[CustomerSignService.GetFormTemplateAsync]: id={Id}, language={Language}", formTemplateId, language);
            var template = await _unitOfWork.FormTemplates.GetByIdWithQuestionsAsync(formTemplateId);
            if (template == null) return null;

            // Try to find a translation for the requested language; fall back to default (en) content if not found
            var templateTranslation = template.Translations
                .FirstOrDefault(t => t.LanguageCode.Equals(language, StringComparison.OrdinalIgnoreCase));

            // Deserialize the questions translation JSON once (null-safe)
            List<QuestionTranslationItem>? questionTranslations = null;
            if (templateTranslation?.QuestionsTranslation is { } json)
            {
                try
                {
                    questionTranslations = System.Text.Json.JsonSerializer.Deserialize<List<QuestionTranslationItem>>(
                        json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[GetFormTemplateAsync] Failed to parse QuestionsTranslation JSON for template {Id} lang {Lang}", formTemplateId, language);
                }
            }

            var translatedQuestions = template.Questions
                .Where(q => q.IsActive && !q.IsDelete)
                .OrderBy(q => q.SortOrder)
                .Select(q =>
                {
                    var qTrans = questionTranslations?.FirstOrDefault(qt => qt.QuestionId == q.Id);
                    return new FormQuestionResponse
                    {
                        Id = q.Id,
                        FormTemplateId = q.FormTemplateId,
                        SortOrder = q.SortOrder,
                        QuestionText = qTrans?.QuestionText ?? q.QuestionText,
                        QuestionType = q.QuestionType,
                        IsRequired = q.IsRequired,
                        HasFollowUpText = q.HasFollowUpText,
                        // Use translated follow-up label if provided, else fall back to default
                        FollowUpLabel = qTrans?.FollowUpLabel ?? q.FollowUpLabel,
                        FollowUpTriggerOption = q.FollowUpTriggerOption,
                        Options = q.Options
                            .Where(o => o.IsActive && !o.IsDelete)
                            .OrderBy(o => o.SortOrder)
                            .Select(o =>
                            {
                                var oTrans = qTrans?.Options?.FirstOrDefault(ot => ot.OptionId == o.Id);
                                return new FormQuestionOptionResponse
                                {
                                    Id = o.Id,
                                    OptionText = oTrans?.OptionText ?? o.OptionText,
                                    SortOrder = o.SortOrder,
                                };
                            }).ToList(),
                    };
                }).ToList();

            return new FormTemplateResponse
            {
                Id = template.Id,
                Title = templateTranslation?.Title ?? template.Title,
                Description = templateTranslation?.Description ?? template.Description ?? string.Empty,
                LogoUrl = template.LogoUrl,
                FooterText = templateTranslation?.FooterText ?? template.FooterText,
                AgreementText = templateTranslation?.AgreementText ?? template.AgreementText,
                Version = template.Version,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                Questions = translatedQuestions,
            };
        }

        // ─── GET document template ────────────────────────────────────────────

        public async Task<DocumentTemplateResponse?> GetDocumentTemplateAsync(int documentTemplateId, string language)
        {
            _logger.LogInformation("[CustomerSignService.GetDocumentTemplateAsync]: id={Id}, language={Language}", documentTemplateId, language);
            var dt = await _dbContext.DocumentTemplateTranslations.Include(x => x.DocumentTemplate).FirstOrDefaultAsync(t => t.DocumentTemplateId == documentTemplateId && t.LanguageCode == language);
            if (dt == null) return null;

            return new DocumentTemplateResponse
            {
                Id = dt.Id,
                Title = dt.Title,
                DocumentType = dt.DocumentTemplate.DocumentType,
                Description = dt.Description!,
                Content = dt.Content,
                UpdatedAt = dt.UpdatedAt,
            };
        }

        // ─── SUBMIT Signature session ───────────────────────────────────────────────

        public async Task<CustomerSessionSubmitResponse> SubmitSignatureSessionAsync(CustomerSessionSubmitRequest request)
        {
            _logger.LogInformation("[CustomerSignService.SubmitSignatureSessionAsync]: customerType={Type}", request.CustomerType);

            // Validate OutletId
            var outlet = await _unitOfWork.Outlets.GetByIdAsync(request.OutletId);
            if (outlet == null)
                throw new Exception($"Outlet with ID {request.OutletId} not found.");

            if (request.PatronId.HasValue)
            {
                var patron = await _patronRepository.GetByIdAsync(request.PatronId.Value);
                if (patron != null)
                {
                    request.FirstName = patron.FirstName;
                    request.LastName = patron.LastName;
                }
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Resolve patron name parts

                if (!string.IsNullOrWhiteSpace(request.GuestName))
                {
                    // GuestName from sharer may be "Last, First" or "Full Name"
                    var parts = request.GuestName.Split(' ', 2, StringSplitOptions.TrimEntries);
                    if (parts.Length >= 2)
                    {
                        request.FirstName = parts[0];
                        request.LastName = parts[1];
                    }
                    else
                    {
                        request.FirstName = request.GuestName;
                    }
                }

                Patron patron;
                #region Skip logics
                // Skip logic update for existing patron if PatronId is provided, always create a new Patron record
                // 2. Create Patron record or update existing if PatronId is provided
                //if (request.PatronId.HasValue)
                //{
                //    patron = await _patronRepository.GetByIdAsync(request.PatronId.Value);
                //    if (patron == null)
                //        throw new KeyNotFoundException($"Patron with ID {request.PatronId.Value} not found.");
                //    patron.FirstName = firstName;
                //    patron.LastName = lastName;
                //    patron.PhoneNumber = request.PhoneNumber;
                //    patron.Nationality = request.Nationality;
                //    _patronRepository.Update(patron);
                //}
                //else
                //{
                //}
                #endregion

                patron = new Patron
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Nationality = request.Nationality,
                    Email = request.Email,
                    Address = request.IdPassport,   // store ID/Passport in Address field
                    RoomNumber = request.RoomNumber,
                    Language = request.Language,
                    CustomerType = request.CustomerType,
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    OutletId = request.OutletId,
                    PlayerId = request.PlayerId,
                };
                await _patronRepository.AddAsync(patron);

                // Flush to get the patronId before adding signatures
                await _unitOfWork.SaveChangesAsync();

                // 3. Save FormSubmission (if there are answers)
                int? submissionId = null;
                if (request.FormTemplateId.HasValue && request.Answers.Any())
                {
                    var template = await _unitOfWork.FormTemplates.GetByIdAsync(request.FormTemplateId.Value)
                        ?? throw new KeyNotFoundException($"FormTemplate {request.FormTemplateId} not found.");

                    var submission = new FormSubmission
                    {
                        FormTemplateId = request.FormTemplateId.Value,
                        TemplateVersion = template.Version,
                        LanguageCode = request.Language,
                        PatronDeviceId = request.PatronDeviceId,
                        SubmittedAt = DateTime.Now,
                        IsActive = true,
                        IsDelete = false,
                        CreatedBy = CommonConstants.SystemUser,
                        UpdatedBy = CommonConstants.SystemUser,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Answers = request.Answers.Select(a => new FormSubmissionAnswer
                        {
                            FormQuestionId = a.FormQuestionId,
                            AnswerValue = a.AnswerValue,
                            FollowUpText = a.FollowUpText,
                            IsActive = true,
                            IsDelete = false,
                            CreatedBy = CommonConstants.SystemUser,
                            UpdatedBy = CommonConstants.SystemUser,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        }).ToList()
                    };

                    await _unitOfWork.FormSubmissions.AddAsync(submission);
                    await _unitOfWork.SaveChangesAsync();
                    submissionId = submission.Id;
                }

                var sigDate = DateTime.Now;

                // 4. Insert placeholder PatronSignature rows immediately so we have IDs.
                //    PDF paths will be updated by the background job after generation.
                PatronSignature? consultationSig = null;
                if (submissionId.HasValue && request.FormTemplateId.HasValue)
                {
                    consultationSig = new PatronSignature
                    {
                        PatronId = patron.Id,
                        DocumentType = DocumentTypeEnum.ConsultationForm,
                        DocumentPath = string.Empty,   // updated by background job
                        SignatureData = request.SignatureDataUrl,
                        SignedDate = sigDate,
                        DeviceInfo = request.PatronDeviceName,
                        Location = outlet.Name,
                        IsActive = true,
                        IsDelete = false,
                        CreatedBy = CommonConstants.SystemUser,
                        UpdatedBy = CommonConstants.SystemUser,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };
                    await _patronSignatureRepository.AddAsync(consultationSig);
                }

                PatronSignature? pdpSig = null;
                if (request.DocumentTemplateId.HasValue)
                {
                    pdpSig = new PatronSignature
                    {
                        PatronId = patron.Id,
                        DocumentType = DocumentTypeEnum.PdpForm,
                        DocumentPath = string.Empty,   // updated by background job
                        SignatureData = request.SignatureDataUrl,
                        SignedDate = sigDate,
                        IsActive = true,
                        IsDelete = false,
                        CreatedBy = CommonConstants.SystemUser,
                        UpdatedBy = CommonConstants.SystemUser,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };
                    await _patronSignatureRepository.AddAsync(pdpSig);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // 5. Enqueue background PDF generation (non-blocking)
                var capturedRequest = request;
                var capturedFirstName = request.FirstName;
                var capturedLastName = request.LastName;
                var capturedPatronId = patron.Id;
                var capturedSubmissionId = submissionId;
                var capturedSigDate = sigDate;
                var capturedConsultationSigId = consultationSig?.Id;
                var capturedPdpSigId = pdpSig?.Id;

                // Load language content for PDF generation
                var langContent = await LoadLanguageContentByCode(request.Language, _appSettingsService);

                await _backgroundQueue.QueueBackgroundWorkItemAsync(async ct =>
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var db = scope.ServiceProvider.GetRequiredService<DigitalDocumentPlatformDbContext>();
                    var pdfConverter = scope.ServiceProvider.GetRequiredService<IPdfConverterService>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomerSignService>>();

                    // Daily output folder: application/Documents/yyyyMMdd/
                    var today = capturedSigDate.ToString("yyyyMMdd");
                    var pdfOutputDir = Path.Combine(AppContext.BaseDirectory, DocumentsFolder, today);
                    Directory.CreateDirectory(pdfOutputDir);

                    try
                    {
                        // 5a. Consultation form PDF
                        if (capturedConsultationSigId.HasValue && capturedSubmissionId.HasValue && capturedRequest.FormTemplateId.HasValue)
                        {
                            var formHtml = await BuildFormSubmissionHtmlAsync(capturedRequest, langContent, db);
                            var path = await GeneratePdfFromHtmlWithConverterAsync(formHtml, pdfOutputDir,
                                $"consultationForm_{capturedPatronId}_{capturedSigDate:HHmmss}", pdfConverter, logger);

                            var sig = await db.PatronSignature.FindAsync(new object[] { capturedConsultationSigId.Value }, ct);
                            if (sig != null) { sig.DocumentPath = ToWebPath(path); sig.UpdatedAt = DateTime.Now; }
                        }

                        // 5b. PDP document PDF
                        if (capturedPdpSigId.HasValue && capturedRequest.DocumentTemplateId.HasValue)
                        {
                            var docTemplate = await db.DocumentTemplates.FindAsync(new object[] { capturedRequest.DocumentTemplateId.Value }, ct);
                            var docTranslation = await db.DocumentTemplateTranslations
                            .FirstOrDefaultAsync(t => t.DocumentTemplateId == capturedRequest.DocumentTemplateId.Value && t.LanguageCode == capturedRequest.Language, ct);
                            if (docTranslation != null && docTemplate != null)
                            {
                                var pdpHtml = BuildSignedDocumentHtml(docTranslation.Content, string.Empty, capturedRequest, langContent);
                                var path = await GeneratePdfFromHtmlWithConverterAsync(pdpHtml, pdfOutputDir,
                                    $"pdpForm_{capturedPatronId}_{capturedSigDate:HHmmss}", pdfConverter, logger);

                                var sig = await db.PatronSignature.FindAsync(new object[] { capturedPdpSigId.Value }, ct);
                                if (sig != null) { sig.DocumentPath = ToWebPath(path); sig.UpdatedAt = DateTime.Now; }
                            }
                            else
                            {
                                _logger.LogWarning("[CustomerSignService BG] No document translation found for DocumentTemplateId={DocumentTemplateId}, language={Language}", capturedRequest.DocumentTemplateId, capturedRequest.Language);
                            }
                        }

                        await db.SaveChangesAsync(ct);
                        logger.LogInformation("[CustomerSignService BG] PDF generation completed for patronId={PatronId}", capturedPatronId);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "[CustomerSignService BG] PDF generation failed for patronId={PatronId}", capturedPatronId);
                    }
                });

                _logger.LogInformation("[CustomerSignService.SubmitSignatureSessionAsync]: completed, patronId={PatronId}", patron.Id);

                return new CustomerSessionSubmitResponse
                {
                    Success = true,
                    Message = "Session submitted successfully.",
                    PatronId = patron.Id,
                    SubmittedAt = sigDate,
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "[CustomerSignService.SubmitSignatureSessionAsync]: failed — {Message}", ex.Message);
                throw;
            }
        }

        // ─── Admin: Signed customers list ─────────────────────────────────────

        public async Task<SignedCustomerListResponse> GetSignedCustomersAsync(SignedCustomerListRequest request)
        {
            _logger.LogInformation("[CustomerSignService.GetSignedCustomersAsync]: page={Page}, size={Size}", request.Page, request.PageSize);

            var query = _dbContext.Patron
                .AsNoTracking()
                .Where(p => p.IsActive && !p.IsDelete)
                .Include(p => p.Outlet)
                .Include(p => p.PatronType)
                .Include(p => p.Signatures)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    (p.FirstName != null && p.FirstName.ToLower().Contains(term)) ||
                    (p.LastName != null && p.LastName.ToLower().Contains(term)) ||
                    (p.RoomNumber != null && p.RoomNumber.ToLower().Contains(term)) ||
                    (p.PhoneNumber != null && p.PhoneNumber.Contains(term)));
            }

            if (request.FromDate.HasValue)
                query = query.Where(p => p.CreatedAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(p => p.CreatedAt <= request.ToDate.Value.AddDays(1));

            if (request.OutletId.HasValue)
                query = query.Where(p => p.OutletId == request.OutletId.Value);

            if (request.PatronTypeId.HasValue)
                query = query.Where(p => p.PatronTypeId == request.PatronTypeId.Value);

            if (!string.IsNullOrWhiteSpace(request.CustomerType))
                query = query.Where(p => p.CustomerType == request.CustomerType);

            var total = await query.CountAsync();

            var patrons = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new SignedCustomerListResponse
            {
                TotalRecords = total,
                Data = patrons.Select(MapToSignedCustomerRow).ToList()
            };
        }

        public async Task<SignedCustomerRow?> GetSignedCustomerDetailAsync(int patronId)
        {
            _logger.LogInformation("[CustomerSignService.GetSignedCustomerDetailAsync]: patronId={Id}", patronId);

            var patron = await _dbContext.Patron
                .AsNoTracking()
                .Include(p => p.Outlet)
                .Include(p => p.PatronType)
                .Include(p => p.Signatures)
                .FirstOrDefaultAsync(p => p.Id == patronId && p.IsActive && !p.IsDelete);

            return patron == null ? null : MapToSignedCustomerRow(patron);
        }

        public async Task<SessionPrefillResponse?> GetSessionPrefillAsync(int patronId, string language)
        {
            _logger.LogInformation("[CustomerSignService.GetSessionPrefillAsync]: patronId={Id}", patronId);

            var patron = await _dbContext.Patron
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == patronId && p.IsActive && !p.IsDelete);

            if (patron == null) return null;

            // Load the most recent FormSubmission linked to this patron's device session
            var latestSubmission = await _dbContext.FormSubmissions
                .AsNoTracking()
                .Include(s => s.Answers)
                .Where(s => s.IsActive && !s.IsDelete &&
                            s.SubmittedAt >= patron.CreatedAt.AddSeconds(-5) &&
                            s.SubmittedAt <= patron.CreatedAt.AddHours(2) &&
                            s.LanguageCode == language)
                .OrderByDescending(s => s.SubmittedAt)
                .FirstOrDefaultAsync();

            return new SessionPrefillResponse
            {
                PatronId = patron.Id,
                FirstName = patron.FirstName,
                LastName = patron.LastName,
                RoomNumber = patron.RoomNumber,
                Language = patron.Language,
                CustomerType = patron.CustomerType,
                Nationality = patron.Nationality,
                PhoneNumber = patron.PhoneNumber,
                IdPassport = patron.Address,
                PlayerId = patron.PlayerId,
                Email = patron.Email,
                PreviousAnswers = latestSubmission?.Answers
                    .Select(a => new PrefillAnswer
                    {
                        FormQuestionId = a.FormQuestionId,
                        AnswerValue = a.AnswerValue,
                        FollowUpText = a.FollowUpText
                    }).ToList() ?? []
            };
        }

        // ─── Private helpers ──────────────────────────────────────────────────

        private static SignedCustomerRow MapToSignedCustomerRow(Patron p) => new()
        {
            Id = p.Id,
            DisplayId = $"#G-{p.Id}",
            FirstName = p.FirstName,
            LastName = p.LastName,
            CustomerName = $"{p.FirstName} {p.LastName}".Trim(),
            Email = p.Email,
            PatronType = p.PatronType?.Name,
            PatronTypeColor = p.PatronType?.ColorHex,
            RoomNumber = p.RoomNumber,
            Language = p.Language?.ToUpper(),
            CustomerType = p.CustomerType,
            OutletId = p.OutletId,
            OutletName = p.Outlet?.Name,
            Nationality = p.Nationality,
            PhoneNumber = p.PhoneNumber,
            SignedAt = p.Signatures
                .Where(s => s.IsActive && !s.IsDelete)
                .OrderByDescending(s => s.SignedDate)
                .Select(s => s.SignedDate)
                .FirstOrDefault(),
            SignedBy = p.Signatures
                .Where(s => s.IsActive && !s.IsDelete)
                .OrderByDescending(s => s.SignedDate)
                .Select(s => $"{p.FirstName} {p.LastName}".Trim())
                .FirstOrDefault(),
            SignedByDevice = p.Signatures
                .Where(s => s.IsActive && !s.IsDelete)
                .OrderByDescending(s => s.SignedDate)
                .Select(s => s.DeviceInfo)
                .FirstOrDefault(),
            Documents = p.Signatures
                .Where(s => s.IsActive && !s.IsDelete)
                .OrderByDescending(s => s.SignedDate)
                .Select(s => new SignedDocumentRow
                {
                    PatronSignatureId = s.Id,
                    DocumentTypeName = s.DocumentType.ToString(),
                    FileName = Path.GetFileName(s.DocumentPath ?? $"{s.DocumentType}.pdf"),
                    FileUrl = s.DocumentPath,
                    SignedAt = s.SignedDate,
                    Status = SignatureSessionStatus.Signed,
                    SignedByDevice = s.DeviceInfo
                }).ToList()
        };

        private async Task<string> BuildFormSubmissionHtmlAsync(CustomerSessionSubmitRequest request, LanguageContent langContent)
        {
            return await BuildFormSubmissionHtmlAsync(request, langContent, _dbContext);
        }

        private static async Task<string> BuildFormSubmissionHtmlAsync(CustomerSessionSubmitRequest request, LanguageContent langContent, DigitalDocumentPlatformDbContext db)
        {
            var template = await db.FormTemplates
                .Include(t => t.Questions.Where(q => !q.IsDelete).OrderBy(q => q.SortOrder))
                    .ThenInclude(q => q.Options.Where(o => !o.IsDelete).OrderBy(o => o.SortOrder))
                .Include(t => t.Translations)
                .FirstOrDefaultAsync(t => t.Id == request.FormTemplateId!.Value);

            if (template == null) return string.Empty;

            var templateTranslation = template.Translations
                .FirstOrDefault(t => t.LanguageCode.Equals(request.Language, StringComparison.OrdinalIgnoreCase));

            if (templateTranslation == null) return string.Empty;

            // --- Resolve language overrides ---
            // Try to find translation for the requested language
            var translation = template.Translations
                .FirstOrDefault(t => t.LanguageCode.Equals(request.Language, StringComparison.OrdinalIgnoreCase));

            // Deserialize question translations once (null-safe)
            List<QuestionTranslationItem>? questionTranslations = null;
            if (translation?.QuestionsTranslation is { } json)
            {
                try
                {
                    questionTranslations = System.Text.Json.JsonSerializer.Deserialize<List<QuestionTranslationItem>>(
                        json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch
                {
                    // Fall back to default texts on parse error
                }
            }

            // Apply template-level translations with fallback to defaults
            string headerDescription = translation?.Description ?? template.Description;
            string footerText = translation?.FooterText ?? template.FooterText ?? string.Empty;
            string agreementText = translation?.AgreementText ?? template.AgreementText ?? string.Empty;

            // Build question and option text mappings with translation priority
            // Map: questionId -> questionText
            var questionTexts = new Dictionary<int, string>();
            // Map: questionId -> (optionId -> (originalText, translatedText))
            var optionTexts = new Dictionary<int, Dictionary<int, (string Original, string Translated)>>();

            foreach (var question in template.Questions)
            {
                var qTrans = questionTranslations?.FirstOrDefault(qt => qt.QuestionId == question.Id);

                // Store question text (translated if available, otherwise original)
                questionTexts[question.Id] = qTrans?.QuestionText ?? question.QuestionText;

                // Build option text dictionary for this question
                var optDict = new Dictionary<int, (string Original, string Translated)>();

                if (qTrans?.Options != null && qTrans.Options.Any())
                {
                    // If translation has options, use them but keep original as backup
                    foreach (var opt in question.Options)
                    {
                        var optTrans = qTrans.Options.FirstOrDefault(ot => ot.OptionId == opt.Id);
                        optDict[opt.Id] = (opt.OptionText, optTrans?.OptionText ?? opt.OptionText);
                    }
                }
                else
                {
                    // No translation available, use original for both
                    foreach (var opt in question.Options)
                    {
                        optDict[opt.Id] = (opt.OptionText, opt.OptionText);
                    }
                }

                optionTexts[question.Id] = optDict;
            }

            // --- Build answer lookup ---
            // answerMap: questionId -> answer item. GroupBy to be safe against accidental duplicate FormQuestionIds from FE.
            var answerMap = request.Answers
                .GroupBy(a => a.FormQuestionId)
                .ToDictionary(g => g.Key, g => g.Last());

            // --- Render questions ---
            const string checkboxChecked = "&#9746;"; // ☑
            const string checkboxEmpty = "&#9744;";   // □

            var questionsHtml = new System.Text.StringBuilder();
            int qNumber = 1;

            foreach (var question in template.Questions.OrderBy(q => q.SortOrder))
            {
                var qText = questionTexts.TryGetValue(question.Id, out var qt) ? qt : question.QuestionText;
                answerMap.TryGetValue(question.Id, out var answerItem);

                questionsHtml.AppendLine($"<div class=\"question-block\">");
                questionsHtml.AppendLine($"<p class=\"question-text\"><strong>{qNumber}.</strong> {System.Net.WebUtility.HtmlEncode(qText)}</p>");

                if (question.Options.Any())
                {
                    var opts = question.Options.OrderBy(o => o.SortOrder).ToList();
                    var optDict = optionTexts.TryGetValue(question.Id, out var od) ? od : new Dictionary<int, (string Original, string Translated)>();

                    // Determine answered values (case-insensitive to handle minor FE/BE text discrepancies)
                    var answeredValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    if (answerItem != null && !string.IsNullOrEmpty(answerItem.AnswerValue))
                    {
                        if (question.QuestionType == QuestionType.MultipleChoice)
                        {
                            try
                            {
                                var vals = System.Text.Json.JsonSerializer.Deserialize<List<string>>(answerItem.AnswerValue);
                                if (vals != null)
                                    foreach (var v in vals) answeredValues.Add(v);
                            }
                            catch { answeredValues.Add(answerItem.AnswerValue); }
                        }
                        else
                        {
                            answeredValues.Add(answerItem.AnswerValue);
                        }
                    }

                    // Choose column count based on option count
                    int colCount = opts.Count <= 2 ? 2 : opts.Count <= 4 ? 4 : 3;
                    // For inline questions (body/face focus), detect by question type
                    if (question.QuestionType == QuestionType.YesNo)
                        colCount = 2;

                    int colWidth = 100 / colCount;

                    questionsHtml.AppendLine("<table class=\"options-table\"><tr>");
                    for (int i = 0; i < opts.Count; i++)
                    {
                        var opt = opts[i];

                        // Get both original and translated text for this option
                        string displayText;
                        bool isChecked = false;

                        if (optDict.TryGetValue(opt.Id, out var textTuple))
                        {
                            // Use the translated text for display
                            displayText = textTuple.Translated;
                            // Check if answer matches either original OR translated text
                            isChecked = answeredValues.Contains(textTuple.Original) ||
                                       answeredValues.Contains(textTuple.Translated);
                        }
                        else
                        {
                            // Fallback to original option text
                            displayText = opt.OptionText;
                            isChecked = answeredValues.Contains(opt.OptionText);
                        }

                        string checkbox = isChecked ? checkboxChecked : checkboxEmpty;

                        questionsHtml.AppendLine($"<td class=\"option-cell\" style=\"width:{colWidth}%;\">{checkbox} {System.Net.WebUtility.HtmlEncode(displayText)}</td>");

                        if ((i + 1) % colCount == 0 && i < opts.Count - 1)
                            questionsHtml.AppendLine("</tr><tr>");
                    }
                    // Fill remaining cells if last row is incomplete
                    int remaining = opts.Count % colCount;
                    if (remaining != 0)
                    {
                        for (int i = 0; i < colCount - remaining; i++)
                            questionsHtml.AppendLine("<td class=\"option-cell\"></td>");
                    }
                    questionsHtml.AppendLine("</tr></table>");
                }
                else if (question.QuestionType == QuestionType.TextInput)
                {
                    // Free-text answer: show underline area
                    var textVal = answerItem?.AnswerValue ?? string.Empty;
                    questionsHtml.AppendLine($"<div class=\"free-text\">{System.Net.WebUtility.HtmlEncode(textVal)}</div>");
                    questionsHtml.AppendLine("<div class=\"underline\"></div>");
                }

                // Follow-up text
                if (question.HasFollowUpText)
                {
                    var qTrans = questionTranslations?.FirstOrDefault(qt => qt.QuestionId == question.Id);
                    var followUpLabel = qTrans?.FollowUpLabel ?? "If yes, please briefly describe";
                    var followUpVal = answerItem?.FollowUpText ?? string.Empty;
                    questionsHtml.AppendLine($"<p class=\"followup-label\">{System.Net.WebUtility.HtmlEncode(followUpLabel)}</p>");
                    questionsHtml.AppendLine($"<div class=\"free-text\">{System.Net.WebUtility.HtmlEncode(followUpVal)}</div>");
                    questionsHtml.AppendLine("<div class=\"underline\"></div>");
                }

                questionsHtml.AppendLine("</div>");
                qNumber++;
            }

            var consultTitle = template.Title;
            var consultDate = DateTime.Now.ToString("dd MMM yyyy HH:mm");
            var guestName = $"{request.FirstName} {request.LastName}".Trim();
            var signedDateStr = DateTime.Now.ToString("dd MMM yyyy HH:mm");
            return $$"""
                <!DOCTYPE html>
                <html>
                <head><meta charset="utf-8"/>
                <style>
                  /* Base / print-stable styles to reduce PDF renderer scaling issues */
                  html,body { box-sizing: border-box; font-family: Arial, sans-serif; font-size: 12pt; line-height: 1.35; color: #4a3728; margin: 0; }
                  @page { size: A4 portrait; margin: 20mm; }
                  body { padding: 16px; }
                  .container { max-width: 800px; margin: 0 auto; }
                  h2 { color:#274549; margin: 0 0 8px 0; }
                  .title-content { margin-left: 0; text-align: left; }
                  .header-desc { margin-bottom: 12px; font-style: italic; color: #6b4f3a; }
                  .question-block { margin-bottom: 12px; }
                  .question-text { margin: 0 0 6px 0; font-size: 11pt; }
                  .options-table { width: 100%; border-collapse: collapse; table-layout: fixed; word-wrap: break-word; margin-bottom: 6px; }
                  .option-cell { padding: 6px 8px; vertical-align: middle; font-size: 11pt; }
                  .free-text { min-height: 18px; padding: 4px 0; font-size: 11pt; }
                  .underline { margin: 6px 0 10px; }
                  .followup-label { margin: 4px 0 6px 0; font-style: italic; }
                  .footer { margin-top: 18px; padding-top: 8px; font-size: 12pt; }
                  .guest-info { margin-bottom: 12px; }
                  .page-title { margin-bottom: 12px; }
                  .sig-block { margin-top: 20px; padding-top: 12px; }
                  img.signature {  }
                  @media print { body { padding: 0; } .container { max-width: none; margin: 0; } }
                </style>
                </head>
                <body>

                <div class="container">
                <div class="page-title">
                  <h2><strong> {{templateTranslation.Title}} </strong></h2>
                </div>
                
                <div class="guest-info">
                  <strong>{{langContent.FirstName}}:</strong> {{request.FirstName}} &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; <strong>{{langContent.LastName}}:</strong> {{request.LastName}}
                </div>

                <div class="guest-info">
                  <strong>{{langContent.Nationality}}:</strong> {{request.Nationality}} &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; <strong>{{langContent.RoomNumber}}:</strong> {{request.RoomNumber}}
                </div>

                <div class="guest-info">
                  <strong>{{langContent.Email}}:</strong> {{request.Email}} &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; <strong>{{langContent.Mobile}}:</strong> {{request.PhoneNumber}}
                </div>

                {{(string.IsNullOrEmpty(headerDescription) ? "" : $"<p class=\"header-desc\">{System.Net.WebUtility.HtmlEncode(headerDescription)}</p>")}}
                <hr style="border-color:#c8a882;margin-bottom:16px;"/>
                {{questionsHtml}}
                {{(string.IsNullOrEmpty(footerText) ? "" : $"<div class=\"footer\">{(footerText)}</div>")}}
                <div class="sig-block">
                  {{(string.IsNullOrEmpty(agreementText) ? "" : $"<p> <input type=\"checkbox\" checked disabled>{System.Net.WebUtility.HtmlEncode(agreementText)}</p>")}}
                  <p><strong>{{langContent.FullName}}:</strong> {{request.FirstName}} {{request.LastName}}</p>
                  <p><strong>{{langContent.Signed}}:</strong> {{signedDateStr}}</p>
                  <p><strong>{{langContent.Signature}}:</strong></p>
                  <img class="signature" src="{{request.SignatureDataUrl}}" alt="{{langContent.Signature}}" style="width: 240px; height: auto;" />
                </div>
                </div>
                </body></html>
                """;
        }

        private static async Task<LanguageContent> LoadLanguageContentByCode(string languageCode, IApplicationSettingsService appSettingsService)
        {
            var jsonContent = await appSettingsService.GetSettingByKeyAsync("MultiLanguageContent");

            var languageContent = new LanguageContent();

            if (!string.IsNullOrEmpty(jsonContent?.Value))
            {
                try
                {
                    var translations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, LanguageContent>>(jsonContent.Value);
                    if (translations != null && translations.TryGetValue(languageCode, out var content))
                    {
                        languageContent = content;
                    }
                }
                catch
                {
                    // Log or handle JSON deserialization error if needed
                }
            }

            return languageContent;
        }

        // DTO for deserializing QuestionsTranslation JSON
        private sealed class QuestionTranslationItem
        {
            public int QuestionId { get; set; }
            public string? QuestionText { get; set; }
            /// <summary>Translated follow-up label (optional, falls back to entity value).</summary>
            public string? FollowUpLabel { get; set; }
            public List<OptionTranslationItem>? Options { get; set; }
        }

        private sealed class OptionTranslationItem
        {
            public int OptionId { get; set; }
            public string? OptionText { get; set; }
        }

        private static string BuildSignedDocumentHtml(
            string documentContent, string documentTitle, CustomerSessionSubmitRequest capturedRequest, LanguageContent langContent)
        {
            var signedDateStr = DateTime.Now.ToString("dd MMM yyyy HH:mm");
            return $$"""
                <!DOCTYPE html>
                <html>
                <head><meta charset="utf-8"/>
                <style>
                  /* Print-stable base styles */
                  html,body { box-sizing: border-box; font-family: Arial, sans-serif; font-size: 12pt; line-height: 1.35; color: #4a3728; margin: 0; }
                  @page { size: A4 portrait; margin: 20mm; }
                  body { padding: 16px; }
                  .container { max-width: 800px; margin: 0 auto; }
                  h2 { color: #274549; margin: 0 0 8px 0; }
                  .title-content { margin-left: 0; text-align: left; }
                  .sig-block { margin-top: 20px; padding-top: 12px; }
                  img.signature {  }
                  @media print { body { padding: 0; } .container { max-width: none; margin: 0; } }
                </style>
                </head>
                <body>
                <div class="container">
                <div class ="title-content">
                <h2>{{documentTitle}}</h2>
                </div>
                {{documentContent}}
                <div class="sig-block">
                  <p><strong>{{langContent.FullName}}:</strong> {{capturedRequest.FirstName}} {{capturedRequest.LastName}}</p>
                  <p><strong>{{langContent.Signed}}:</strong> {{signedDateStr}}</p>
                  <p><strong>{{langContent.Signature}}:</strong></p>
                  <img class="signature" src="{{capturedRequest.SignatureDataUrl}}" alt="Signature" style="width: 240px; height: auto;"/>
                </div>
                </div>
                </body></html>
                """;
        }

        private async Task<string> GeneratePdfFromHtmlAsync(string htmlContent, string outputDir, string fileBaseName)
            => await GeneratePdfFromHtmlWithConverterAsync(htmlContent, outputDir, fileBaseName, _pdfConverter, _logger);

        private static async Task<string> GeneratePdfFromHtmlWithConverterAsync(
            string htmlContent,
            string outputDir,
            string fileBaseName,
            IPdfConverterService pdfConverter,
            ILogger logger)
        {
            var htmlPath = Path.Combine(outputDir, $"{fileBaseName}.html");
            await File.WriteAllTextAsync(htmlPath, htmlContent, System.Text.Encoding.UTF8);

            try
            {
                var pdfPath = await pdfConverter.ConvertToPdfAsync(htmlPath, outputDir);
                return pdfPath;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[CustomerSignService] PDF generation failed for {File}; returning HTML path as fallback.", htmlPath);
                return htmlPath;
            }
            finally
            {
                if (File.Exists(htmlPath))
                {
                    // keep the HTML file for debugging if PDF generation fails, but delete it if PDF was successfully generated
                    //try { File.Delete(htmlPath); } catch { /* best-effort */ }
                }
            }
        }

        private string ToWebPath(string absolutePath)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath);
            var rel = Path.GetRelativePath(webRoot, absolutePath).Replace('\\', '/');
            if (!rel.StartsWith("/")) rel = "/" + rel;
            return rel;
        }
    }
}