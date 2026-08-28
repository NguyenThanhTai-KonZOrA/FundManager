using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.UnitOfWork;
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;
using Microsoft.Extensions.Logging;

namespace FundManager.Implement.Services
{
    public class PatronTypeService : IPatronTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PatronTypeService> _logger;

        public PatronTypeService(IUnitOfWork unitOfWork, ILogger<PatronTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<PatronTypeResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.PatronTypes.GetAllAsync();
            return list.OrderBy(p => p.SortOrder).ThenBy(p => p.Name)
                       .Select(MapPatronType).ToList();
        }

        public async Task<PatronTypeResponse?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.PatronTypes.GetByIdAsync(id);
            return entity == null ? null : MapPatronType(entity);
        }

        public async Task<PatronTypeResponse> CreateAsync(CreatePatronTypeRequest request, string createdBy)
        {
            if (await _unitOfWork.PatronTypes.NameExistsAsync(request.Name))
                throw new InvalidOperationException($"PatronType name '{request.Name}' already exists.");

            var entity = new PatronType
            {
                Name = request.Name,
                ColorHex = request.ColorHex,
                Description = request.Description,
                SortOrder = request.SortOrder,
                IsActive = true,
                IsDelete = false,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _unitOfWork.PatronTypes.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("[PatronTypeService] Created PatronType: {Name}", entity.Name);
            return MapPatronType(entity);
        }

        public async Task<PatronTypeResponse> UpdateAsync(UpdatePatronTypeRequest request, string updatedBy)
        {
            var entity = await _unitOfWork.PatronTypes.GetByIdAsync(request.Id)
                ?? throw new KeyNotFoundException($"PatronType {request.Id} not found.");

            if (await _unitOfWork.PatronTypes.NameExistsAsync(request.Name, request.Id))
                throw new InvalidOperationException($"PatronType name '{request.Name}' already exists.");

            entity.Name = request.Name;
            entity.ColorHex = request.ColorHex;
            entity.Description = request.Description;
            entity.SortOrder = request.SortOrder;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.PatronTypes.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return MapPatronType(entity);
        }

        public async Task<bool> DeleteAsync(int id, string deletedBy)
        {
            var entity = await _unitOfWork.PatronTypes.GetByIdAsync(id);
            if (entity == null) return false;

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.UpdatedBy = deletedBy;
            entity.UpdatedAt = DateTime.Now;
            _unitOfWork.PatronTypes.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActiveAsync(int id, string updatedBy)
        {
            var entity = await _unitOfWork.PatronTypes.GetByIdAsync(id);
            if (entity == null) return false;

            entity.IsActive = !entity.IsActive;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;
            _unitOfWork.PatronTypes.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static PatronTypeResponse MapPatronType(PatronType p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            ColorHex = p.ColorHex,
            Description = p.Description,
            SortOrder = p.SortOrder,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
        };
    }
}
