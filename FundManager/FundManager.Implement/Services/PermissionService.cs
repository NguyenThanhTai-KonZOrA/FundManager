using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.UnitOfWork;
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionService(
            IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PermissionResponse>> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllNoTrackingAsync();
            return permissions.Select(p => new PermissionResponse
            {
                Id = p.Id,
                PermissionName = p.PermissionName,
                PermissionCode = p.PermissionCode,
                Description = p.Description,
                Category = p.Category,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();
        }

        public async Task<PermissionResponse?> GetPermissionByIdAsync(int permissionId)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            if (permission == null) return null;

            return new PermissionResponse
            {
                Id = permission.Id,
                PermissionName = permission.PermissionName,
                PermissionCode = permission.PermissionCode,
                Description = permission.Description,
                Category = permission.Category,
                IsActive = permission.IsActive,
                CreatedAt = permission.CreatedAt
            };
        }

        public async Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request, string createdBy)
        {
            if (await _permissionRepository.PermissionCodeExistsAsync(request.PermissionCode))
            {
                throw new InvalidOperationException($"Permission code '{request.PermissionCode}' already exists.");
            }

            var permission = new Permission
            {
                PermissionName = request.PermissionName,
                PermissionCode = request.PermissionCode,
                Description = request.Description,
                Category = request.Category,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = true,
                IsDelete = false
            };

            await _permissionRepository.AddAsync(permission);
            await _unitOfWork.SaveChangesAsync();

            return (await GetPermissionByIdAsync(permission.Id))!;
        }

        public async Task<PermissionResponse> UpdatePermissionAsync(UpdatePermissionRequest request, string updatedBy)
        {
            var permission = await _permissionRepository.GetByIdAsync(request.Id);
            if (permission == null)
            {
                throw new InvalidOperationException($"Permission with ID {request.Id} not found.");
            }

            if (!string.IsNullOrEmpty(request.PermissionCode) &&
                await _permissionRepository.PermissionCodeExistsAsync(request.PermissionCode, request.Id))
            {
                throw new InvalidOperationException($"Permission code '{request.PermissionCode}' already exists.");
            }

            permission.PermissionName = request.PermissionName;
            permission.PermissionCode = request.PermissionCode;
            permission.Description = request.Description;
            permission.Category = request.Category;
            permission.UpdatedBy = updatedBy;
            permission.UpdatedAt = DateTime.Now;

            _permissionRepository.Update(permission);
            await _unitOfWork.SaveChangesAsync();

            return (await GetPermissionByIdAsync(permission.Id))!;
        }

        public async Task<bool> DeletePermissionAsync(int permissionId, string deletedBy)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            if (permission == null)
            {
                throw new InvalidOperationException($"Permission with ID {permissionId} not found.");
            }

            permission.IsDelete = true;
            permission.UpdatedBy = deletedBy;
            permission.UpdatedAt = DateTime.Now;

            _permissionRepository.Update(permission);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActiveAsync(int permissionId, string updatedBy)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            if (permission == null)
            {
                throw new InvalidOperationException($"Permission with ID {permissionId} not found.");
            }

            permission.IsActive = !permission.IsActive;
            permission.UpdatedBy = updatedBy;
            permission.UpdatedAt = DateTime.Now;

            _permissionRepository.Update(permission);
            await _unitOfWork.SaveChangesAsync();
            return permission.IsActive;
        }

        public async Task<Dictionary<string, List<PermissionResponse>>> GetPermissionsByCategoryAsync()
        {
            var permissions = await GetAllPermissionsAsync();
            return permissions
                .GroupBy(p => p.Category ?? "Other")
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );
        }
    }
}