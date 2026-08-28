using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;
using Microsoft.Extensions.Logging;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PropertyService> _logger;

        public PropertyService(IUnitOfWork unitOfWork, ILogger<PropertyService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<PropertyResponse>> GetActivePropertiesAsync()
        {
            _logger.LogInformation("[PropertyService.GetActivePropertiesAsync]: Fetching active properties");
            var properties = await _unitOfWork.Properties.GetActivePropertiesAsync();
            var result = new List<PropertyResponse>();
            foreach (var p in properties)
            {
                var outlets = await _unitOfWork.Outlets.GetByPropertyIdAsync(p.Id);
                result.Add(MapToResponse(p, outlets));
            }
            return result;
        }

        public async Task<PropertyResponse?> GetPropertyByIdAsync(int id)
        {
            _logger.LogInformation("[PropertyService.GetPropertyByIdAsync]: id={Id}", id);
            var property = await _unitOfWork.Properties.GetPropertyByIdAsync(id);
            if (property == null) return null;
            var outlets = await _unitOfWork.Outlets.GetByPropertyIdAsync(id);
            return MapToResponse(property, outlets);
        }

        public async Task<PropertyResponse> CreatePropertyAsync(CreatePropertyRequest request, string createdBy)
        {
            _logger.LogInformation("[PropertyService.CreatePropertyAsync]: name={Name}, createdBy={CreatedBy}", request.Name, createdBy);
            var entity = new Property
            {
                Name = request.Name,
                Description = request.Description,
                Color = request.Color,
                IsActive = true,
                IsDelete = false,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _unitOfWork.Properties.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var outlets = await _unitOfWork.Outlets.GetByPropertyIdAsync(entity.Id);
            return MapToResponse(entity, outlets);
        }

        public async Task<PropertyResponse> UpdatePropertyAsync(UpdatePropertyRequest request, string updatedBy)
        {
            _logger.LogInformation("[PropertyService.UpdatePropertyAsync]: id={Id}, updatedBy={UpdatedBy}", request.Id, updatedBy);
            var entity = await _unitOfWork.Properties.GetPropertyByIdAsync(request.Id)
                         ?? throw new KeyNotFoundException($"Property {request.Id} not found.");

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Color = request.Color;
            entity.IsActive = request.IsActive;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.Properties.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var outlets = await _unitOfWork.Outlets.GetByPropertyIdAsync(entity.Id);
            return MapToResponse(entity, outlets);
        }

        public async Task DeletePropertyAsync(int id, string deletedBy)
        {
            _logger.LogInformation("[PropertyService.DeletePropertyAsync]: id={Id}, deletedBy={DeletedBy}", id, deletedBy);
            var entity = await _unitOfWork.Properties.GetPropertyByIdAsync(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Property {id} not found.");
            }

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.UpdatedBy = deletedBy;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.Properties.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private static PropertyResponse MapToResponse(Property p, List<Outlet> outlets) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            Description = p.Description,
            Color = p.Color,
            IsActive = p.IsActive,
            IsPrimaryOutlet = outlets.Any(o => o.PropertyOutlets.Any(po => po.PropertyId == p.Id)),
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Outlets = outlets.Select(OutletService.MapToResponse).ToList()
        };
    }
}
