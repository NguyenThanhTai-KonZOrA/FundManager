using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class DocumentTemplateService : IDocumentTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DigitalDocumentPlatformDbContext _dbContext;
        private readonly ILogger<DocumentTemplateService> _logger;

        public DocumentTemplateService(
            IUnitOfWork unitOfWork,
            DigitalDocumentPlatformDbContext dbContext,
            ILogger<DocumentTemplateService> logger)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _logger = logger;
        }

        // ── Document templates ─────────────────────────────────────────────────────
        #region Document templates
        public async Task<List<DocumentTemplateBriefResponse>> GetListAsync()
        {
            var list = await _unitOfWork.DocumentTemplates.GetAllActiveAsync();
            return list.Select(MapToBrief).ToList();
        }

        public async Task<DocumentTemplateResponse> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.DocumentTemplates.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"DocumentTemplate {id} not found.");
            return MapToResponse(entity);
        }

        public async Task<List<DocumentTemplateBriefResponse>> GetByTypeAsync(DocumentType documentType)
        {
            var list = await _unitOfWork.DocumentTemplates.GetByTypeAsync(documentType);
            return list.Select(MapToBrief).ToList();
        }

        public async Task<List<DocumentTemplateBriefResponse>> GetByOutletAsync(int outletId)
        {
            var list = await _unitOfWork.DocumentTemplates.GetByOutletAsync(outletId);
            return list.Select(MapToBrief).ToList();
        }

        public async Task<DocumentTemplateResponse> CreateAsync(CreateDocumentTemplateRequest request, string createdBy)
        {
            var entity = new DocumentTemplate
            {
                Title = request.Title,
                DocumentType = request.DocumentType,
                Description = request.Description,
                Content = request.Content,
                OutletId = request.OutletId,
                Version = 1,
                IsActive = true,
                CreatedBy = createdBy,
                UpdatedBy = createdBy
            };

            await _unitOfWork.DocumentTemplates.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("[DocumentTemplateService.Create] Id={Id}, Title={Title}, By={By}",
                entity.Id, entity.Title, createdBy);

            return MapToResponse(entity);
        }

        public async Task<DocumentTemplateResponse> UpdateAsync(UpdateDocumentTemplateRequest request, string updatedBy)
        {
            var entity = await _unitOfWork.DocumentTemplates.GetByIdAsync(request.Id)
                ?? throw new KeyNotFoundException($"DocumentTemplate {request.Id} not found.");

            // Snapshot BEFORE applying changes, whenever content or title changes
            bool contentChanged = entity.Content != request.Content
                || entity.Title != request.Title
                || entity.Description != request.Description;

            if (contentChanged)
            {
                var snapshot = new DocumentTemplateVersionHistory
                {
                    DocumentTemplateId = entity.Id,
                    Version = entity.Version,
                    Title = entity.Title,
                    Description = entity.Description,
                    Content = entity.Content,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = updatedBy,
                    ChangeNote = request.ChangeNote
                };
                await _dbContext.DocumentTemplateVersionHistories.AddAsync(snapshot);
            }

            entity.Title = request.Title;
            entity.DocumentType = request.DocumentType;
            entity.Description = request.Description;
            entity.Content = request.Content;
            entity.OutletId = request.OutletId;
            entity.IsActive = request.IsActive;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;

            if (contentChanged)
            {
                entity.Version++;
                _logger.LogInformation("[DocumentTemplateService.Update] Version bumped to {Version} for Id={Id}", entity.Version, entity.Id);
            }

            _unitOfWork.DocumentTemplates.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(entity);
        }

        public async Task DeleteAsync(int id, string deletedBy)
        {
            var entity = await _unitOfWork.DocumentTemplates.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"DocumentTemplate {id} not found.");

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.UpdatedBy = deletedBy;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.DocumentTemplates.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
        #endregion

        // ── Template translations ────────────────────────────────────────────────
        public async Task<DocumentTemplateTranslationResponse> UpsertDocumentTemplateTranslationAsync(
           UpsertDocumentTemplateTranslationRequest request, string updatedBy)
        {
            var existing = await _dbContext.DocumentTemplateTranslations
                .FirstOrDefaultAsync(t =>
                    t.DocumentTemplateId == request.DocumentTemplateId &&
                    t.LanguageCode == request.LanguageCode);

            if (existing == null)
            {
                existing = new DocumentTemplateTranslation
                {
                    DocumentTemplateId = request.DocumentTemplateId,
                    LanguageCode = request.LanguageCode,
                };
                await _dbContext.DocumentTemplateTranslations.AddAsync(existing);
            }

            existing.Title = request.Title;
            existing.Description = request.Description;
            existing.Content = request.Content;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = updatedBy;

            await _dbContext.SaveChangesAsync();
            return MapDocTranslation(existing);
        }

        public async Task<List<DocumentTemplateTranslationResponse>> GetDocumentTemplateTranslationsAsync(int documentTemplateId)
        {
            var list = await _dbContext.DocumentTemplateTranslations
                .AsNoTracking()
                .Where(t => t.DocumentTemplateId == documentTemplateId)
                .OrderBy(t => t.LanguageCode)
                .ToListAsync();

            return list.Select(MapDocTranslation).ToList();
        }

        // ── Version histories ─────────────────────────────────────────────────────
        public async Task<List<DocumentTemplateVersionHistoryResponse>> GetDocumentTemplateVersionHistoryAsync(int documentTemplateId)
        {
            var list = await _dbContext.DocumentTemplateVersionHistories
                .AsNoTracking()
                .Where(v => v.DocumentTemplateId == documentTemplateId)
                .OrderByDescending(v => v.Version)
                .ToListAsync();

            return list.Select(v => new DocumentTemplateVersionHistoryResponse
            {
                Id = v.Id,
                DocumentTemplateId = v.DocumentTemplateId,
                Version = v.Version,
                Title = v.Title,
                Description = v.Description,
                Content = v.Content,
                UpdatedAt = v.UpdatedAt,
                UpdatedBy = v.UpdatedBy,
                ChangeNote = v.ChangeNote
            }).ToList();
        }

        // ── Mapping helpers ───────────────────────────────────────────────────
        private static DocumentTemplateTranslationResponse MapDocTranslation(DocumentTemplateTranslation t) => new()
        {
            Id = t.Id,
            DocumentTemplateId = t.DocumentTemplateId,
            LanguageCode = t.LanguageCode,
            Title = t.Title,
            Description = t.Description,
            Content = t.Content,
            UpdatedAt = t.UpdatedAt,
            UpdatedBy = t.UpdatedBy
        };

        private static DocumentTemplateResponse MapToResponse(DocumentTemplate d) => new()
        {
            Id = d.Id,
            Title = d.Title,
            DocumentType = d.DocumentType,
            Description = d.Description,
            Content = d.Content,
            Version = d.Version,
            IsActive = d.IsActive,
            OutletId = d.OutletId,
            OutletName = d.Outlet?.Name,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            Translations = d.Translations.Select(MapDocTranslation).ToList(),
            VersionHistories = d.VersionHistories
                .OrderByDescending(v => v.Version)
                .Select(v => new DocumentTemplateVersionHistoryResponse
                {
                    Id = v.Id,
                    DocumentTemplateId = v.DocumentTemplateId,
                    Version = v.Version,
                    Title = v.Title,
                    Description = v.Description,
                    Content = v.Content,
                    UpdatedAt = v.UpdatedAt,
                    UpdatedBy = v.UpdatedBy,
                    ChangeNote = v.ChangeNote
                }).ToList()
        };

        private static DocumentTemplateBriefResponse MapToBrief(DocumentTemplate d) => new()
        {
            Id = d.Id,
            Title = d.Title,
            DocumentType = d.DocumentType,
            Description = d.Description,
            Version = d.Version,
            IsActive = d.IsActive,
            OutletId = d.OutletId,
            OutletName = d.Outlet?.Name,
            UpdatedAt = d.UpdatedAt,
            UpdatedBy = d.UpdatedBy,
            Translations = d.Translations.Select(MapDocTranslation).ToList(),
            VersionHistories = d.VersionHistories
                .OrderByDescending(v => v.Version)
                .Select(v => new DocumentTemplateVersionHistoryResponse
                {
                    Id = v.Id,
                    DocumentTemplateId = v.DocumentTemplateId,
                    Version = v.Version,
                    Title = v.Title,
                    Description = v.Description,
                    Content = v.Content,
                    UpdatedAt = v.UpdatedAt,
                    UpdatedBy = v.UpdatedBy,
                    ChangeNote = v.ChangeNote
                }).ToList()
        };
    }
}