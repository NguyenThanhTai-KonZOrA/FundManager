using DigitalDocumentPlatform.Common.Enum;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;

namespace DigitalDocumentPlatform.Implement.Services
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

        public async Task<ApplicationImageResponse> CreateAsync(CreateApplicationImageRequest request, string createdBy)
        {
            _logger.LogInformation("[ApplicationImageService.CreateAsync]: name={Name}, type={Type}, createdBy={CreatedBy}", request.Name, request.Type, createdBy);

            ValidateTypeRequirements(request.Type, request.PropertyId, request.OutletId);

            var (filePath, fileUrl, ext, size) = await SaveFileAsync(request.File);

            Property? property = null;
            Outlet? outlet = null;

            if (request.PropertyId.HasValue)
            {
                property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId.Value);
                if (property == null)
                {
                    throw new KeyNotFoundException($"Property {request.PropertyId.Value} not found.");
                }
            }

            if (request.OutletId.HasValue)
            {
                outlet = await _unitOfWork.Outlets.GetByIdAsync(request.OutletId.Value);
                if (outlet == null)
                {
                    throw new KeyNotFoundException($"Outlet {request.OutletId.Value} not found.");
                }
            }

            var entity = new ApplicationImage
            {
                Name = request.Name,
                Description = request.Description,
                FilePath = filePath,
                FileUrl = fileUrl,
                FileExtension = ext,
                FileSize = size,
                Type = request.Type,
                PropertyId = request.PropertyId,
                OutletId = request.OutletId,
                IsActive = true,
                IsDelete = false,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _unitOfWork.ApplicationImages.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return MapToResponse(entity);
        }

        public async Task<ApplicationImageResponse> UpdateAsync(UpdateApplicationImageRequest request, string updatedBy)
        {
            _logger.LogInformation("[ApplicationImageService.UpdateAsync]: id={Id}, updatedBy={UpdatedBy}", request.Id, updatedBy);

            ValidateTypeRequirements(request.Type, request.PropertyId, request.OutletId);

            var entity = await _unitOfWork.ApplicationImages.GetByIdAsync(request.Id)
                         ?? throw new KeyNotFoundException($"ApplicationImage {request.Id} not found.");

            Property? property = null;
            Outlet? outlet = null;

            if (request.PropertyId.HasValue)
            {
                property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId.Value);
                if (property == null)
                {
                    throw new KeyNotFoundException($"Property {request.PropertyId.Value} not found.");
                }
            }

            if (request.OutletId.HasValue)
            {
                outlet = await _unitOfWork.Outlets.GetByIdAsync(request.OutletId.Value);
                if (outlet == null)
                {
                    throw new KeyNotFoundException($"Outlet {request.OutletId.Value} not found.");
                }
            }

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Type = request.Type;
            entity.PropertyId = request.PropertyId;
            entity.OutletId = request.OutletId;
            entity.IsActive = request.IsActive;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;

            // Replace file only if a new one is uploaded
            if (request.File != null)
            {
                // Delete old file
                DeleteFileIfExists(entity.FilePath);

                var (filePath, fileUrl, ext, size) = await SaveFileAsync(request.File);
                entity.FilePath = filePath;
                entity.FileUrl = fileUrl;
                entity.FileExtension = ext;
                entity.FileSize = size;
            }

            _unitOfWork.ApplicationImages.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return MapToResponse(entity);
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
