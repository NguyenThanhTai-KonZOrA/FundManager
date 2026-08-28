using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.UnitOfWork;
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;
using Microsoft.Extensions.Logging;

namespace FundManager.Implement.Services
{
    public class OutletService : IOutletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OutletService> _logger;

        public OutletService(IUnitOfWork unitOfWork, ILogger<OutletService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<OutletResponse>> GetAllActiveAsync()
        {
            _logger.LogInformation("[OutletService.GetAllActiveAsync]: Fetching active outlets");
            var outlets = await _unitOfWork.Outlets.GetAllActiveWithPropertiesAsync();
            return outlets.Select(MapToResponse).ToList();
        }

        public async Task<List<OutletResponse>> GetByPropertyIdAsync(int propertyId)
        {
            _logger.LogInformation("[OutletService.GetByPropertyIdAsync]: propertyId={PropertyId}", propertyId);
            var outlets = await _unitOfWork.Outlets.GetByPropertyIdAsync(propertyId);
            return outlets.Select(MapToResponse).ToList();
        }

        public async Task<OutletResponse?> GetByIdAsync(int id)
        {
            _logger.LogInformation("[OutletService.GetByIdAsync]: id={Id}", id);
            var outlet = await _unitOfWork.Outlets.GetByIdWithPropertiesAsync(id);
            return outlet == null ? null : MapToResponse(outlet);
        }

        public async Task<OutletResponse> CreateAsync(CreateOutletRequest request, string createdBy)
        {
            _logger.LogInformation("[OutletService.CreateAsync]: name={Name}, createdBy={CreatedBy}", request.Name, createdBy);
            var entity = new Outlet
            {
                Name = request.Name,
                Code = request.Code,
                MainColor = request.MainColor,
                Description = request.Description,
                IconImageUrl = request.IconImageUrl,
                BackgroundImageUrl = request.BackgroundImageUrl,
                IsActive = true,
                IsDelete = false,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _unitOfWork.Outlets.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Link to properties via join table
            foreach (var pid in request.PropertyIds.Distinct())
            {
                await _unitOfWork.PropertyOutlets.AddAsync(new PropertyOutlet
                {
                    PropertyId = pid,
                    OutletId = entity.Id,
                    CreatedBy = createdBy,
                    UpdatedBy = createdBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.Outlets.GetByIdWithPropertiesAsync(entity.Id);
            return MapToResponse(created!);
        }

        public async Task<OutletResponse> UpdateAsync(UpdateOutletRequest request, string updatedBy)
        {
            _logger.LogInformation("[OutletService.UpdateAsync]: id={Id}, updatedBy={UpdatedBy}", request.Id, updatedBy);
            var entity = await _unitOfWork.Outlets.GetByIdAsync(request.Id)
                         ?? throw new KeyNotFoundException($"Outlet {request.Id} not found.");

            entity.Name = request.Name;
            entity.Code = request.Code;
            entity.MainColor = request.MainColor;
            entity.Description = request.Description;
            entity.IconImageUrl = request.IconImageUrl;
            entity.BackgroundImageUrl = request.BackgroundImageUrl;
            entity.IsActive = request.IsActive;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.Outlets.Update(entity);

            // Re-sync property links
            var existing = await _unitOfWork.PropertyOutlets.GetByOutletIdAsync(entity.Id);
            foreach (var po in existing)
                _unitOfWork.PropertyOutlets.Remove(po);

            foreach (var pid in request.PropertyIds.Distinct())
                await _unitOfWork.PropertyOutlets.AddAsync(new PropertyOutlet
                {
                    PropertyId = pid,
                    OutletId = entity.Id,
                    CreatedBy = updatedBy,
                    UpdatedBy = updatedBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.Outlets.GetByIdWithPropertiesAsync(entity.Id);
            return MapToResponse(updated!);
        }

        public async Task DeleteAsync(int id, string deletedBy)
        {
            _logger.LogInformation("[OutletService.DeleteAsync]: id={Id}, deletedBy={DeletedBy}", id, deletedBy);
            var entity = await _unitOfWork.Outlets.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Outlet {id} not found.");

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.UpdatedBy = deletedBy;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.Outlets.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        internal static OutletResponse MapToResponse(Outlet o) => new()
        {
            Id = o.Id,
            Name = o.Name,
            Code = o.Code,
            Description = o.Description,
            MainColor = o.MainColor,
            IconImageUrl = o.IconImageUrl,
            BackgroundImageUrl = o.BackgroundImageUrl,
            IsActive = o.IsActive,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            Properties = o.PropertyOutlets
               .Where(po => po.Property != null)
               .Select(po => new PropertyBriefResponse
               {
                   Id = po.Property!.Id,
                   Name = po.Property.Name,
                   Color = po.Property.Color,
                   Code = po.Property.Code
               }).ToList()
        };

        public async Task<List<OutletStaffDeviceResponse>> GetStaffDevicesByOutletAsync(int outletId)
        {
            _logger.LogInformation("[OutletService.GetStaffDevicesByOutletAsync]: outletId={OutletId}", outletId);
            var staffDevices = await _unitOfWork.StaffDevices.GetByOutletIdAsync(outletId);
            return staffDevices.Select(MapToStaffDeviceResponse).ToList();
        }

        public async Task<bool> AssignStaffDeviceAsync(int outletId, int staffDeviceId, string updatedBy)
        {
            _logger.LogInformation("[OutletService.AssignStaffDeviceAsync]: outletId={OutletId}, staffDeviceId={StaffDeviceId}", outletId, staffDeviceId);
            // Verify outlet exists
            var outlet = await _unitOfWork.Outlets.GetByIdAsync(outletId)
                ?? throw new KeyNotFoundException($"Outlet {outletId} not found.");
            return await _unitOfWork.StaffDevices.AssignToOutletAsync(staffDeviceId, outlet.Id, updatedBy);
        }

        public async Task<bool> UnassignStaffDeviceAsync(int outletId, int staffDeviceId, string updatedBy)
        {
            _logger.LogInformation("[OutletService.UnassignStaffDeviceAsync]: outletId={OutletId}, staffDeviceId={StaffDeviceId}", outletId, staffDeviceId);
            return await _unitOfWork.StaffDevices.UnassignFromOutletAsync(staffDeviceId, updatedBy);
        }

        private static OutletStaffDeviceResponse MapToStaffDeviceResponse(DataAccess.EntityModels.StaffDevice s)
        {
            var mapping = s.DeviceMappings?.FirstOrDefault(m => m.IsActive && !m.IsDelete);
            return new OutletStaffDeviceResponse
            {
                StaffDeviceId = s.Id,
                DeviceName = s.DeviceName,
                MacAddress = s.MacAddress,
                IpAddress = s.IpAddress,
                StaffUserName = s.StaffUserName,
                IsOnline = s.IsOnline,
                LastHeartbeat = s.LastHeartbeat,
                OutletId = s.OutletId,
                PairedPatronDevice = mapping == null ? null : new PairedPatronDeviceResponse
                {
                    DeviceMappingId = mapping.Id,
                    PatronDeviceId = mapping.PatronDeviceId,
                    PatronDeviceName = mapping.PatronDevice?.DeviceName ?? string.Empty,
                    PatronIpAddress = mapping.PatronDevice?.IpAddress,
                    PatronIsOnline = mapping.PatronDevice?.IsOnline ?? false,
                    Location = mapping.Location,
                    Notes = mapping.Notes
                }
            };
        }
    }
}
