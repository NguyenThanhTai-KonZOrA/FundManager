using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;
using Microsoft.Extensions.Logging;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LanguageService> _logger;

        public LanguageService(IUnitOfWork unitOfWork, ILogger<LanguageService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<LanguageResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Languages.GetAllLanguagesAsync();
            return list.OrderBy(l => l.SortOrder).ThenBy(l => l.Name)
                       .Select(MapLanguage).ToList();
        }

        public async Task<LanguageResponse?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Languages.GetByIdAsync(id);
            return entity == null ? null : MapLanguage(entity);
        }

        public async Task<LanguageResponse> CreateAsync(CreateLanguageRequest request, string createdBy)
        {
            if (await _unitOfWork.Languages.CodeExistsAsync(request.Code))
                throw new InvalidOperationException($"Language code '{request.Code}' already exists.");

            var entity = new Language
            {
                Code = request.Code.ToLowerInvariant(),
                Name = request.Name,
                NativeName = request.NativeName,
                FlagEmoji = request.FlagEmoji,
                SortOrder = request.SortOrder,
                IsActive = true,
                IsDelete = false,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _unitOfWork.Languages.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("[LanguageService] Created language: {Code}", entity.Code);
            return MapLanguage(entity);
        }

        public async Task<LanguageResponse> UpdateAsync(UpdateLanguageRequest request, string updatedBy)
        {
            var entity = await _unitOfWork.Languages.GetByIdAsync(request.Id)
                ?? throw new KeyNotFoundException($"Language {request.Id} not found.");

            if (await _unitOfWork.Languages.CodeExistsAsync(request.Code, request.Id))
                throw new InvalidOperationException($"Language code '{request.Code}' already exists.");

            entity.Code = request.Code.ToLowerInvariant();
            entity.Name = request.Name;
            entity.NativeName = request.NativeName;
            entity.FlagEmoji = request.FlagEmoji;
            entity.SortOrder = request.SortOrder;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.Languages.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return MapLanguage(entity);
        }

        public async Task<bool> DeleteAsync(int id, string deletedBy)
        {
            var entity = await _unitOfWork.Languages.GetByIdAsync(id);
            if (entity == null) return false;

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.UpdatedBy = deletedBy;
            entity.UpdatedAt = DateTime.Now;
            _unitOfWork.Languages.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActiveAsync(int id, string updatedBy)
        {
            var entity = await _unitOfWork.Languages.GetByIdAsync(id);
            if (entity == null) return false;

            entity.IsActive = !entity.IsActive;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;
            _unitOfWork.Languages.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static LanguageResponse MapLanguage(Language l) => new()
        {
            Id = l.Id,
            Code = l.Code,
            Name = l.Name,
            NativeName = l.NativeName,
            FlagEmoji = l.FlagEmoji,
            SortOrder = l.SortOrder,
            IsActive = l.IsActive,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
            UpdatedBy = l.UpdatedBy,
        };
    }
}