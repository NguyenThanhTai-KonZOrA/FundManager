using FundManager.Common.Enum;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.UnitOfWork;
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;

namespace FundManager.Implement.Services
{
    public class ApplicationImageService : IApplicationImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ApplicationImageService> _logger;
        private readonly string _applicationImageStoragePath;

        public ApplicationImageService(IUnitOfWork unitOfWork, ILogger<ApplicationImageService> logger, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            _applicationImageStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "ApplicationImages");
            if (!Directory.Exists(_applicationImageStoragePath))
            {
                Directory.CreateDirectory(_applicationImageStoragePath);
            }
        }

        public async Task<List<ApplicationImageResponse>> GetAllActiveAsync()
        {
            _logger.LogInformation("[ApplicationImageService.GetAllActiveAsync]: Fetching active images");
            var images = await _unitOfWork.ApplicationImages.GetAllActiveAsync();
            return images.Select(MapToResponse).ToList();
        }

        public async Task<List<ApplicationImageResponse>> GetByTypeAsync(ImageTypeEnum type)
        {
            _logger.LogInformation("[ApplicationImageService.GetByTypeAsync]: type={Type}", type);
            var images = await _unitOfWork.ApplicationImages.GetByTypeAsync(type);
            return images.Select(MapToResponse).ToList();
        }

        public async Task<List<ApplicationImageResponse>> GetByTypeAsync(ImageTypeEnum type, int? propertyId, int? outletId = null)
        {
            _logger.LogInformation("[ApplicationImageService.GetByTypeAsync]: type={Type}, propertyId={PropertyId}, outletId={OutletId}", type, propertyId, outletId);

            // SliderHotel: filter by property
            if (type == ImageTypeEnum.SliderHotel && propertyId.HasValue)
            {
                var images = await _unitOfWork.ApplicationImages.GetByTypeAndPropertyAsync(type, propertyId.Value);
                return images.Select(MapToResponse).ToList();
            }

            // SliderOutlet: filter by outlet
            if (type == ImageTypeEnum.SliderOutlet && outletId.HasValue)
            {
                var images = await _unitOfWork.ApplicationImages.GetByTypeAndOutletAsync(type, outletId.Value);
                return images.Select(MapToResponse).ToList();
            }
            else if (type == ImageTypeEnum.SliderOutlet && propertyId.HasValue)
            {
                var images = await _unitOfWork.ApplicationImages.GetByTypeAndPropertyAsync(type, propertyId.Value);
                return images.Select(MapToResponse).ToList();
            }

            // Legacy Slider (type=1): filter by property when provided
            //if (type == ImageTypeEnum.Slider && propertyId.HasValue)
            //{
            //    var images = await _unitOfWork.ApplicationImages.GetByTypeAndPropertyAsync(type, propertyId.Value);
            //    return images.Select(MapToResponse).ToList();
            //}

            return await GetByTypeAsync(type);
        }

        public async Task<ApplicationImageResponse?> GetByIdAsync(int id)
        {
            _logger.LogInformation("[ApplicationImageService.GetByIdAsync]: id={Id}", id);
            var image = await _unitOfWork.ApplicationImages.GetByIdAsync(id);
            return image == null ? null : MapToResponse(image);
        }

        public async Task DeleteAsync(int id, string deletedBy)
        {
            _logger.LogInformation("[ApplicationImageService.DeleteAsync]: id={Id}, deletedBy={DeletedBy}", id, deletedBy);
            var entity = await _unitOfWork.ApplicationImages.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"ApplicationImage {id} not found.");

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.UpdatedBy = deletedBy;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.ApplicationImages.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── File helpers ─────────────────────────────────────────────────────────

        private async Task<(string filePath, string fileUrl, string ext, long size)> SaveFileAsync(IFormFile file)
        {
            var uploadsDir = _applicationImageStoragePath;
            Directory.CreateDirectory(uploadsDir);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsDir, uniqueName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relPath = Path.Combine("ApplicationImages", Path.GetFileName(fullPath)).Replace('\\', '/');
            var fileUrl = $"/{relPath}";

            return (fullPath, fileUrl, ext, file.Length);
        }

        private void DeleteFileIfExists(string? filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                try { File.Delete(filePath); }
                catch (Exception ex) { _logger.LogWarning(ex, "Could not delete old file: {Path}", filePath); }
            }
        }

        private static void ValidateTypeRequirements(ImageTypeEnum type, int? propertyId, int? outletId)
        {
            // Legacy Slider and new SliderHotel require PropertyId
            if ((type == ImageTypeEnum.Slider || type == ImageTypeEnum.SliderHotel) && !propertyId.HasValue)
                throw new ArgumentException("PropertyId is required when ImageType is Slider or SliderHotel.");

            // Outlet image and SliderOutlet require OutletId
            if ((type == ImageTypeEnum.Outlet || type == ImageTypeEnum.SliderOutlet) && !outletId.HasValue)
                throw new ArgumentException("OutletId is required when ImageType is Outlet or SliderOutlet.");
        }

        private static ApplicationImageResponse MapToResponse(ApplicationImage i) => new()
        {
            Id = i.Id,
            Name = i.Name,
            Description = i.Description,
            FilePath = i.FilePath,
            FileUrl = i.FileUrl,
            FileExtension = i.FileExtension,
            FileSize = i.FileSize,
            Type = i.Type,
            TypeName = GetDescription(i.Type),
            PropertyId = i.PropertyId,
            OutletId = i.OutletId,
            IsActive = i.IsActive,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };

        private static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }
    }
}
