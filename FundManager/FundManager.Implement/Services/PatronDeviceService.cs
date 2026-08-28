using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class PatronDeviceService : IPatronDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatronRepository _patronRepository;
        private readonly ILogger<PatronDeviceService> _logger;
        private readonly IPatronDeviceRepository _patronDeviceRepository;
        private readonly ISignatureSessionRepository _signatureSessionRepository;
        private readonly IStaffDeviceRepository _staffDeviceRepository;
        private readonly IDeviceMappingRepository _deviceMappingRepository;
        private readonly IWorkflowRepository _workflowRepository;
        // ADD: Semaphore for device registration
        private static readonly SemaphoreSlim _deviceLock = new SemaphoreSlim(1, 1);

        public PatronDeviceService(
            IUnitOfWork unitOfWork,
            IPatronRepository patronRepository,
            ILogger<PatronDeviceService> logger,
            IPatronDeviceRepository patronDeviceRepository,
            ISignatureSessionRepository signatureSessionRepository,
            IStaffDeviceRepository staffDeviceRepository,
            IDeviceMappingRepository deviceMappingRepository,
            IWorkflowRepository workflowRepository
            )
        {
            _unitOfWork = unitOfWork;
            _patronRepository = patronRepository;
            _logger = logger;
            _patronDeviceRepository = patronDeviceRepository;
            _signatureSessionRepository = signatureSessionRepository;
            _staffDeviceRepository = staffDeviceRepository;
            _deviceMappingRepository = deviceMappingRepository;
            _workflowRepository = workflowRepository;
        }

        #region Patron Device Connection Management
        public async Task<PatronDevice> RegisterDeviceAsync(string deviceName, string connectionId, string? macAddress, string? ipAddress)
        {
            var device = await _patronDeviceRepository
                .FirstOrDefaultAsync(d => d.ConnectionId == connectionId);

            if (device == null)
            {
                device = new PatronDevice
                {
                    DeviceName = deviceName,
                    ConnectionId = connectionId,
                    MacAddress = macAddress,
                    IpAddress = ipAddress,
                    IsOnline = true,
                    IsAvailable = true,
                    LastHeartbeat = DateTime.Now
                };
                await _patronDeviceRepository.AddAsync(device);
            }
            else
            {
                device.IsOnline = true;
                device.IsAvailable = true;
                device.LastHeartbeat = DateTime.Now;
                device.DeviceName = deviceName;
                device.MacAddress = macAddress ?? device.MacAddress;
                device.IpAddress = ipAddress ?? device.IpAddress;
            }

            await _unitOfWork.SaveChangesAsync();
            return device;
        }

        public async Task SetDeviceOfflineAsync(string connectionId)
        {
            var device = await _patronDeviceRepository
                .FirstOrDefaultAsync(d => d.ConnectionId == connectionId);

            if (device != null)
            {
                device.IsOnline = false;
                device.IsAvailable = false;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<PatronDevice?> GetAvailableDeviceForStaffAsync(int staffDeviceId)
        {
            return await _patronDeviceRepository.GetAvailableDeviceForStaffAsync(staffDeviceId);
        }

        public async Task<SignatureSession> CreateSignatureSessionAsync(int patronId, int staffDeviceId, int patronDeviceId)
        {
            var session = new SignatureSession
            {
                PatronId = patronId,
                StaffDeviceId = staffDeviceId,
                PatronDeviceId = patronDeviceId,
                Status = SignatureSessionStatus.Pending,
                RequestedAt = DateTime.Now
            };

            await _signatureSessionRepository.AddAsync(session);

            // Mark device as busy
            var device = await _patronDeviceRepository.FirstOrDefaultAsync(x => x.Id == patronDeviceId);
            if (device != null)
            {
                //device.IsAvailable = false;
                device.IsAvailable = true;
            }

            await _unitOfWork.SaveChangesAsync();
            return session;
        }

        public async Task<(SignatureSession, Patron)> CompleteSignatureSessionAsync(int sessionId, string signatureDataUrl)
        {
            var session = await _signatureSessionRepository.FirstOrDefaultAsync(s => s.Id == sessionId, s => s.Patron, s => s.PatronDevice);

            if (session == null) throw new Exception("Session not found");

            // Update patron signature
            var patron = await _patronRepository.FirstOrDefaultAsync(p => p.Id == session.PatronId);
            if (patron == null) throw new Exception(string.Format("Patron not found for session ID {0}", sessionId));

            session.Status = SignatureSessionStatus.Signed;
            session.CompletedAt = DateTime.Now;

            // Mark device as available again
            if (session.PatronDevice != null)
            {
                session.PatronDevice.IsAvailable = true;
            }

            // Clear up session pending status
            var pendingSessions = await _signatureSessionRepository
                .FindAsync(s => s.PatronId == session.PatronId && s.Id != sessionId && !s.CompletedAt.HasValue);

            foreach (var pendingSession in pendingSessions)
            {
                pendingSession.Status = SignatureSessionStatus.Cancelled;
                _signatureSessionRepository.Update(pendingSession);
            }

            await _unitOfWork.SaveChangesAsync();
            return (session!, patron!);
        }

        public async Task UpdateHeartbeatAsync(string connectionId, string deviceType)
        {
            if (deviceType == "staff")
            {
                var staffDevice = await _staffDeviceRepository
                    .FirstOrDefaultAsync(d => d.ConnectionId == connectionId);

                if (staffDevice != null)
                {
                    staffDevice.LastHeartbeat = DateTime.Now;
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                var device = await _patronDeviceRepository
                    .FirstOrDefaultAsync(d => d.ConnectionId == connectionId);

                if (device != null)
                {
                    device.LastHeartbeat = DateTime.Now;
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        public async Task<List<PatronDevice>> GetOnlineDevicesAsync()
        {
            return await _patronDeviceRepository.GetOnlineDevicesAsync();
        }

        public async Task<StaffAndPatronDevicesResponse> GetAllStaffAndPatronDevicesAsync()
        {
            var staffDevices = await _staffDeviceRepository.GetAllNoTrackingAsync();
            var patronDevices = await _patronDeviceRepository.GetAllNoTrackingAsync();
            return new StaffAndPatronDevicesResponse
            {
                TotalPatronDevices = patronDevices.Count(),
                TotalStaffDevices = staffDevices.Count(),
                StaffDevices = staffDevices.Select(d => new StaffDeviceResponse
                {
                    Id = d.Id,
                    DeviceName = d.DeviceName,
                    MacAddress = d.MacAddress ?? string.Empty,
                    IpAddress = d.IpAddress ?? string.Empty,
                    IsOnline = d.IsOnline,
                    IsActive = d.IsActive,
                    ConnectionId = d.ConnectionId ?? string.Empty,
                    StaffUserName = d.StaffUserName ?? string.Empty,
                    LastHeartbeat = d.LastHeartbeat,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                }).ToList(),
                PatronDevices = patronDevices.Select(d => new PatronDeviceResponse
                {
                    Id = d.Id,
                    DeviceName = d.DeviceName,
                    MacAddress = d.MacAddress ?? string.Empty,
                    IpAddress = d.IpAddress ?? string.Empty,
                    IsOnline = d.IsOnline,
                    IsActive = d.IsActive,
                    ConnectionId = d.ConnectionId,
                    StaffUserName = string.Empty,
                    LastHeartbeat = d.LastHeartbeat,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                }).ToList()
            };
        }

        public async Task<StaffAndPatronDevicesSummaryResponse> GetAllStaffAndPatronDevicesSummaryAsync()
        {
            var staffDevices = await _staffDeviceRepository.GetAllNoTrackingAsync();
            var patronDevices = await _patronDeviceRepository.GetAllNoTrackingAsync();
            staffDevices = staffDevices.Where(d => d.IsActive && !d.IsDelete).ToList();
            patronDevices = patronDevices.Where(d => d.IsActive && !d.IsDelete).ToList();
            return new StaffAndPatronDevicesSummaryResponse
            {
                StaffDevices = staffDevices.Select(d => new StaffDeviceSummaryResponse
                {
                    StaffDeviceId = d.Id,
                    StaffDeviceName = d.DeviceName
                }).ToList(),
                PatronDevices = patronDevices.Select(d => new PatronDeviceSummaryResponse
                {
                    PatronDeviceId = d.Id,
                    PatronDeviceName = d.DeviceName
                }).ToList()
            };
        }

        // Manage device operations
        public async Task<bool> ToggleDeviceActiveAsync(int deviceId, string deviceType, bool isActive)
        {
            if (string.Equals(deviceType, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                var device = await _staffDeviceRepository.FirstOrDefaultAsync(d => d.Id == deviceId);
                if (device == null) throw new Exception($"Staff device with ID {deviceId} not found");
                if (device.IsOnline) throw new Exception("Cannot toggle the active status of an online device.");

                device.IsActive = isActive;
                device.UpdatedAt = DateTime.Now;
                _staffDeviceRepository.Update(device);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            else if (string.Equals(deviceType, "Patron", StringComparison.OrdinalIgnoreCase))
            {
                var device = await _patronDeviceRepository.FirstOrDefaultAsync(d => d.Id == deviceId);
                if (device == null) throw new Exception($"Patron device with ID {deviceId} not found");
                if (device.IsOnline) throw new Exception("Cannot toggle the active status of an online device.");

                device.IsActive = isActive;
                device.UpdatedAt = DateTime.Now;
                _patronDeviceRepository.Update(device);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            throw new Exception("DeviceType must be either 'Staff' or 'Patron'");
        }

        public async Task<bool> DeleteDeviceAsync(int deviceId, string deviceType)
        {
            if (string.Equals(deviceType, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                var device = await _staffDeviceRepository.FirstOrDefaultAsync(d => d.Id == deviceId);
                if (device == null) throw new Exception($"Staff device with ID {deviceId} not found");
                if (device.IsOnline) throw new Exception("Cannot delete an online device. Please wait for the device to go offline.");

                _staffDeviceRepository.Remove(device);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            else if (string.Equals(deviceType, "Patron", StringComparison.OrdinalIgnoreCase))
            {
                var device = await _patronDeviceRepository.FirstOrDefaultAsync(d => d.Id == deviceId);
                if (device == null) throw new Exception($"Patron device with ID {deviceId} not found");
                if (device.IsOnline) throw new Exception("Cannot delete an online device. Please wait for the device to go offline.");

                _patronDeviceRepository.Remove(device);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            throw new Exception("DeviceType must be either 'Staff' or 'Patron'");
        }

        public async Task<ChangeHostnameResponse> ChangeDeviceHostnameAsync(int deviceId, string deviceType, string newHostname)
        {
            if (string.Equals(deviceType, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                var existingDevice = await _staffDeviceRepository.FirstOrDefaultAsync(d => d.DeviceName == newHostname && d.Id != deviceId);
                if (existingDevice != null) throw new Exception($"A staff device with hostname '{newHostname}' already exists");

                var device = await _staffDeviceRepository.FirstOrDefaultAsync(d => d.Id == deviceId);
                if (device == null) throw new Exception($"Staff device with ID {deviceId} not found");

                var oldHostname = device.DeviceName;
                device.DeviceName = newHostname;
                device.UpdatedAt = DateTime.Now;
                _staffDeviceRepository.Update(device);
                await _unitOfWork.SaveChangesAsync();

                return new ChangeHostnameResponse
                {
                    Id = device.Id,
                    OldHostname = oldHostname,
                    NewHostname = device.DeviceName,
                    UpdatedAt = device.UpdatedAt
                };
            }
            else if (string.Equals(deviceType, "Patron", StringComparison.OrdinalIgnoreCase))
            {
                var existingDevice = await _patronDeviceRepository.FirstOrDefaultAsync(d => d.DeviceName == newHostname && d.Id != deviceId);
                if (existingDevice != null) throw new Exception($"A patron device with hostname '{newHostname}' already exists");

                var device = await _patronDeviceRepository.FirstOrDefaultAsync(d => d.Id == deviceId);
                if (device == null) throw new Exception($"Patron device with ID {deviceId} not found");

                var oldHostname = device.DeviceName;
                device.DeviceName = newHostname;
                device.UpdatedAt = DateTime.Now;
                _patronDeviceRepository.Update(device);
                await _unitOfWork.SaveChangesAsync();

                return new ChangeHostnameResponse
                {
                    Id = device.Id,
                    OldHostname = oldHostname,
                    NewHostname = device.DeviceName,
                    UpdatedAt = device.UpdatedAt
                };
            }

            throw new Exception("DeviceType must be either 'Staff' or 'Patron'");
        }

        public async Task<CreateOrUpdateMappingResponse> UpdateMappingAsync(UpdateMappingRequest request)
        {
            _logger.LogInformation("[UpdateMappingAsync]: Updating mapping ID: {MappingId}", request.Id);

            // Get existing mapping
            var mapping = await _deviceMappingRepository.FirstOrDefaultAsync(m => m.Id == request.Id);
            if (mapping == null) throw new Exception($"Mapping ID {request.Id} not found");

            if (!mapping.IsActive) throw new Exception($"Mapping ID {request.Id} is inactive and cannot be updated");

            bool hasChanges = false;

            // Update StaffDevice if provided
            if (!string.IsNullOrEmpty(request.NewStaffDeviceName))
            {
                var newStaffDevice = await _staffDeviceRepository.FirstOrDefaultAsync(s => s.DeviceName == request.NewStaffDeviceName);
                if (newStaffDevice == null) throw new Exception($"Staff device '{request.NewStaffDeviceName}' not found");

                // Check if new StaffDevice is already mapped to another device
                var existingStaffMapping = await _deviceMappingRepository.GetMappingByStaffDeviceIdAsync(newStaffDevice.Id);
                if (existingStaffMapping != null && existingStaffMapping.Id != request.Id)
                    throw new Exception($"Staff device '{request.NewStaffDeviceName}' is already mapped to another device");

                if (mapping.StaffDeviceId != newStaffDevice.Id)
                {
                    _logger.LogInformation("[UpdateMappingAsync]: Changing StaffDevice from {OldId} to {NewId}",
                        mapping.StaffDeviceId, newStaffDevice.Id);
                    mapping.StaffDeviceId = newStaffDevice.Id;
                    hasChanges = true;
                }
            }

            // Update PatronDevice if provided
            if (!string.IsNullOrEmpty(request.NewPatronDeviceName))
            {
                var newPatronDevice = await _patronDeviceRepository.FirstOrDefaultAsync(p => p.DeviceName == request.NewPatronDeviceName);
                if (newPatronDevice == null)
                    throw new Exception($"Patron device '{request.NewPatronDeviceName}' not found");

                // Check if new PatronDevice is already mapped to another device
                var existingPatronMapping = await _deviceMappingRepository.GetMappingByPatronDeviceIdAsync(newPatronDevice.Id);
                if (existingPatronMapping != null && existingPatronMapping.Id != request.Id)
                    throw new Exception($"Patron device '{request.NewPatronDeviceName}' is already mapped to another device");

                if (mapping.PatronDeviceId != newPatronDevice.Id)
                {
                    _logger.LogInformation("[UpdateMappingAsync]: Changing PatronDevice from {OldId} to {NewId}",
                        mapping.PatronDeviceId, newPatronDevice.Id);
                    mapping.PatronDeviceId = newPatronDevice.Id;
                    hasChanges = true;
                }
            }

            // Update Location if provided
            if (mapping != null && (mapping.Location != request.OutletName || mapping.Notes != request.Notes))
            {
                mapping.StaffDevice.OutletId = request.OutletId;
                mapping.Location = request.OutletName;
                mapping.Notes = request.Notes;
                hasChanges = true;
            }

            if (hasChanges)
            {
                mapping!.LastVerified = DateTime.Now;
                mapping.UpdatedAt = DateTime.Now;
                _deviceMappingRepository.Update(mapping);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("[UpdateMappingAsync]: Mapping ID {MappingId} updated successfully", mapping.Id);
            }

            return new CreateOrUpdateMappingResponse
            {
                Id = mapping!.Id,
                StaffDeviceId = mapping.StaffDeviceId,
                StaffDeviceName = mapping.StaffDevice?.DeviceName,
                PatronDeviceId = mapping.PatronDeviceId,
                PatronDeviceName = mapping.PatronDevice?.DeviceName,
                OutletId = mapping.StaffDevice?.OutletId ?? 0,
                OutletName = mapping.StaffDevice?.Outlet?.Name,
                IsActive = mapping.IsActive,
                LastVerified = mapping.LastVerified
            };
        }

        public async Task<PatronDevice> AddOrUpdatePatronDeviceAsync(string deviceName, string connectionId, string? macAddress, string? ipAddress)
        {
            await _deviceLock.WaitAsync();
            try
            {
                _logger.LogInformation("[AddOrUpdateDeviceAsync]: Processing DeviceName: {DeviceName}", deviceName);

                var existingDevice = await _patronDeviceRepository.FirstOrDefaultAsync(d => d.DeviceName == deviceName && d.IsActive);

                if (existingDevice != null)
                {
                    _logger.LogInformation("[AddOrUpdateDeviceAsync]: Updating existing device ID: {DeviceId}", existingDevice.Id);

                    existingDevice.ConnectionId = connectionId;
                    existingDevice.IsOnline = true;
                    existingDevice.IsAvailable = true;
                    existingDevice.LastHeartbeat = DateTime.Now;
                    existingDevice.UpdatedAt = DateTime.Now;

                    if (!string.IsNullOrEmpty(macAddress))
                        existingDevice.MacAddress = macAddress;

                    if (!string.IsNullOrEmpty(ipAddress))
                        existingDevice.IpAddress = ipAddress;

                    _patronDeviceRepository.Update(existingDevice);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("[AddOrUpdateDeviceAsync]: Updated device {DeviceName} (ID: {DeviceId})",
                        deviceName, existingDevice.Id);

                    return existingDevice;
                }
                else
                {
                    _logger.LogInformation("[AddOrUpdateDeviceAsync]: Creating new device");

                    var newDevice = new PatronDevice
                    {
                        DeviceName = deviceName,
                        ConnectionId = connectionId,
                        MacAddress = macAddress,
                        IpAddress = ipAddress,
                        IsOnline = true,
                        IsAvailable = true,
                        LastHeartbeat = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    await _patronDeviceRepository.AddAsync(newDevice);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("[AddOrUpdateDeviceAsync]: Created new device {DeviceName} (ID: {DeviceId})",
                        deviceName, newDevice.Id);

                    return newDevice;
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true ||
                                               ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogWarning(ex, "[AddOrUpdateDeviceAsync]: Duplicate key detected for {DeviceName}, retrying...", deviceName);

                var existing = await _patronDeviceRepository.FirstOrDefaultAsync(d => d.DeviceName == deviceName);
                if (existing != null)
                {
                    existing.ConnectionId = connectionId;
                    existing.IsOnline = true;
                    existing.LastHeartbeat = DateTime.Now;

                    if (!string.IsNullOrEmpty(macAddress))
                        existing.MacAddress = macAddress;
                    if (!string.IsNullOrEmpty(ipAddress))
                        existing.IpAddress = ipAddress;

                    _patronDeviceRepository.Update(existing);
                    await _unitOfWork.SaveChangesAsync();

                    return existing;
                }

                throw;
            }
            finally
            {
                _deviceLock.Release();
            }
        }

        public async Task<bool> UpdateConnectionIdAsync(string deviceName, string newConnectionId)
        {
            try
            {
                var device = await GetDeviceByNameAsync(deviceName);

                if (device == null)
                {
                    _logger.LogWarning("Device {DeviceName} not found", deviceName);
                    return false;
                }

                device.ConnectionId = newConnectionId;
                device.IsOnline = true;
                device.LastHeartbeat = DateTime.Now;
                device.UpdatedAt = DateTime.Now;

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated ConnectionId for device {DeviceName}", deviceName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ConnectionId for device {DeviceName}", deviceName);
                return false;
            }
        }
        #endregion

        #region Device Mapping
        public async Task<CreateOrUpdateMappingResponse> CreateOrUpdateMappingAsync(CreateMappingRequest request)
        {
            _logger.LogInformation("[CreateOrUpdateMappingAsync]: Mapping StaffPC '{StaffDevice}' to iPad '{PatronDevice}'",
                request.StaffDeviceName, request.PatronDeviceName);

            // Get staff device
            var staffDevice = await _staffDeviceRepository.GetStaffDeviceByNameAsync(request.StaffDeviceName);
            if (staffDevice == null) throw new Exception($"Staff device '{request.StaffDeviceName}' not found. Please register the PC first.");

            // Get patron device
            var patronDevice = await _patronDeviceRepository.FirstOrDefaultAsync(p => p.DeviceName == request.PatronDeviceName);
            if (patronDevice == null) throw new Exception($"Patron device '{request.PatronDeviceName}' not found. Please register the iPad first.");

            // Check if either device is already mapped
            var existingMappingByStaff = await _deviceMappingRepository.GetMappingByStaffDeviceIdAsync(staffDevice.Id);
            var existingMappingByPatron = await _deviceMappingRepository.GetMappingByPatronDeviceIdAsync(patronDevice.Id);

            var deviceMappingExisted = await _deviceMappingRepository.GetMappingByStaffAndPatronDeviceIdAsync(staffDevice.Id, patronDevice.Id);

            // If both devices are already mapped to each other, just update
            if (deviceMappingExisted != null)
            {
                _logger.LogInformation("[CreateOrUpdateMappingAsync]: Updating existing mapping ID: {MappingId}", deviceMappingExisted.Id);

                deviceMappingExisted.StaffDevice.OutletId = request.OutletId;
                deviceMappingExisted.LastVerified = DateTime.Now;
                deviceMappingExisted.UpdatedAt = DateTime.Now;
                deviceMappingExisted.Location = staffDevice.Outlet?.Name ?? string.Empty;
                deviceMappingExisted.Notes = request.Notes;

                _deviceMappingRepository.Update(deviceMappingExisted);
                await _unitOfWork.SaveChangesAsync();

                return new CreateOrUpdateMappingResponse
                {
                    Id = deviceMappingExisted.Id,
                    StaffDeviceId = deviceMappingExisted.StaffDeviceId,
                    StaffDeviceName = deviceMappingExisted.StaffDevice?.DeviceName,
                    PatronDeviceId = deviceMappingExisted.PatronDeviceId,
                    PatronDeviceName = deviceMappingExisted.PatronDevice?.DeviceName,
                    OutletId = deviceMappingExisted.StaffDevice?.OutletId ?? 0,
                    OutletName = deviceMappingExisted.StaffDevice?.Outlet?.Name,
                    IsActive = deviceMappingExisted.IsActive,
                    LastVerified = deviceMappingExisted.LastVerified
                };
            }

            // Deactivate old mappings
            if (existingMappingByStaff != null)
            {
                _logger.LogInformation("[CreateOrUpdateMappingAsync]: Deactivating old mapping for StaffDevice ID: {StaffDeviceId}", staffDevice.Id);
                existingMappingByStaff.IsActive = false;
                existingMappingByStaff.UpdatedAt = DateTime.Now;
                _deviceMappingRepository.Update(existingMappingByStaff);
            }

            if (existingMappingByPatron != null && existingMappingByPatron.Id != existingMappingByStaff?.Id)
            {
                _logger.LogInformation("[CreateOrUpdateMappingAsync]: Deactivating old mapping for PatronDevice ID: {PatronDeviceId}", patronDevice.Id);
                existingMappingByPatron.IsActive = false;
                existingMappingByPatron.UpdatedAt = DateTime.Now;
                _deviceMappingRepository.Update(existingMappingByPatron);
            }

            // Update the staff device's outlet if it has changed
            if (staffDevice.OutletId != request.OutletId)
            {
                staffDevice.OutletId = request.OutletId;
                staffDevice.UpdatedAt = DateTime.Now;
                _staffDeviceRepository.Update(staffDevice);
            }

            // Create new mapping
            var newMapping = new DeviceMapping
            {
                StaffDeviceId = staffDevice.Id,
                PatronDeviceId = patronDevice.Id,
                Location = staffDevice.Outlet?.Name ?? string.Empty,
                Notes = request.Notes,
                IsActive = true,
                LastVerified = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = CommonConstants.SystemUser,
                UpdatedBy = CommonConstants.SystemUser
            };

            await _deviceMappingRepository.AddAsync(newMapping);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("[CreateOrUpdateMappingAsync]: Mapping created successfully - StaffPC '{StaffDevice}' <-> iPad '{PatronDevice}'",
                request.StaffDeviceName, request.PatronDeviceName);

            return new CreateOrUpdateMappingResponse
            {
                Id = newMapping.Id,
                StaffDeviceId = newMapping.StaffDeviceId,
                StaffDeviceName = request.StaffDeviceName,
                PatronDeviceId = newMapping.PatronDeviceId,
                PatronDeviceName = request.PatronDeviceName,
                OutletId = request.OutletId,
                OutletName = staffDevice?.Outlet?.Name,
                IsActive = newMapping.IsActive,
                LastVerified = newMapping.LastVerified
            };
        }

        public async Task<DeviceMapping?> GetMappingByStaffDeviceNameAsync(string staffDeviceName)
        {
            return await _deviceMappingRepository.GetMappingByStaffDeviceNameAsync(staffDeviceName);
        }

        public async Task<DeviceMapping?> GetMappingByPatronDeviceNameAsync(string patronDeviceName)
        {
            return await _deviceMappingRepository.GetMappingByPatronDeviceNameAsync(patronDeviceName);
        }

        public async Task<DeviceMappingResponse> GetFullInformationMappingByPatronDeviceNameAsync(string patronDeviceName)
        {
            var mapping = await _deviceMappingRepository.GetMappingByPatronDeviceNameAsync(patronDeviceName);
            if (mapping == null) throw new Exception("Mapping not found");

            var outlet = await _staffDeviceRepository.GetOutletByStaffDeviceIdAsync(mapping.StaffDeviceId);
            if (outlet == null) throw new Exception("Outlet not found");

            var workflow = await _workflowRepository.GetActiveByOutletIdAsync(outlet.Id) ??
                await _workflowRepository.GetDefaultWorkflowAsync();

            return new DeviceMappingResponse
            {
                Id = mapping.Id,
                Location = mapping.Location ?? string.Empty,
                StaffDevice = new StaffDeviceData
                {
                    Id = mapping.StaffDeviceId,
                    DeviceName = mapping.StaffDevice?.DeviceName ?? string.Empty,
                    StaffUserName = mapping.StaffDevice?.StaffUserName ?? string.Empty,
                    IsOnline = mapping.StaffDevice?.IsOnline ?? false
                },
                PatronDevice = new PatronDeviceData
                {
                    Id = mapping.PatronDeviceId,
                    DeviceName = mapping.PatronDevice?.DeviceName ?? string.Empty,
                    IsOnline = mapping.PatronDevice?.IsOnline ?? false,
                    IsAvailable = mapping.PatronDevice?.IsAvailable ?? false
                },
                Outlet = outlet,
                Workflow = new WorkflowResponse
                {
                    Id = workflow!.Id,
                    Name = workflow.Name,
                    Description = workflow.Description,
                    OutletId = workflow.OutletId,
                    OutletName = outlet.Name,
                    IsActive = workflow.IsActive,
                    CreatedAt = workflow.CreatedAt,
                    UpdatedAt = workflow.UpdatedAt,
                    Steps = workflow.Steps?.Select(s => new WorkflowStepResponse
                    {
                        Id = s.Id,
                        FormTemplateId = s.FormTemplateId,
                        FormTemplateTitle = s.FormTemplate?.Title,
                        DocumentTemplateId = s.DocumentTemplateId,
                        DocumentTemplateTitle = s.DocumentTemplate?.Title,
                        StepLabel = s.StepLabel,
                        StepOrder = s.StepOrder,
                        StepType = s.StepType
                    }).ToList() ?? []
                }
            };
        }

        public async Task<int?> GetStaffDeviceIdByPatronDeviceNameAsync(string patronDeviceName)
        {
            var mapping = await _deviceMappingRepository.GetMappingByPatronDeviceNameAsync(patronDeviceName);
            return mapping?.StaffDeviceId;
        }

        public async Task<int?> GetPatronDeviceIdByStaffDeviceNameAsync(string staffDeviceName)
        {
            var mapping = await _deviceMappingRepository.GetMappingByStaffDeviceNameAsync(staffDeviceName);
            return mapping?.PatronDeviceId;
        }

        public async Task<List<DeviceMappingSettingsResponse>> GetAllActiveMappingsAsync()
        {
            var mappings = await _deviceMappingRepository.GetAllActiveMappingsAsync();
            return new List<DeviceMappingSettingsResponse>(mappings.Select(m => new DeviceMappingSettingsResponse
            {
                Id = m.Id,
                StaffDeviceId = m.StaffDeviceId,
                StaffDeviceName = m.StaffDevice?.DeviceName ?? string.Empty,
                PatronDeviceId = m.PatronDeviceId,
                PatronDeviceName = m.PatronDevice?.DeviceName ?? string.Empty,
                Location = m.Location,
                Notes = m.Notes,
                IsActive = m.IsActive,
                LastVerified = m.LastVerified,
                OutletId = m.StaffDevice?.OutletId ?? 0,
                OutletName = m.StaffDevice?.Outlet?.Name ?? string.Empty,
                PatronIsOnline = m.PatronDevice?.IsOnline ?? false,
                StaffIsOnline = m.StaffDevice?.IsOnline ?? false,
                PropertyId = m.StaffDevice?.Outlet?.PropertyOutlets?.FirstOrDefault()?.PropertyId ?? 0,
                PropertyName = m.StaffDevice?.Outlet?.PropertyOutlets?.FirstOrDefault()?.Property?.Name ?? string.Empty,
                PropertyCode = m.StaffDevice?.Outlet?.PropertyOutlets?.FirstOrDefault()?.Property?.Code ?? string.Empty
            }));
        }

        public async Task<bool> DeleteMappingAsync(int mappingId)
        {
            var mapping = await _deviceMappingRepository
                .FirstOrDefaultAsync(m => m.Id == mappingId,
                                     m => m.PatronDevice,
                                     m => m.StaffDevice);

            if (mapping == null)
            {
                _logger.LogWarning("[DeleteMappingAsync]: Mapping ID {MappingId} not found", mappingId);
                throw new Exception($"Mapping ID {mappingId} not found");
            }

            if (!mapping.IsActive)
            {
                _logger.LogWarning("[DeleteMappingAsync]: Mapping ID {MappingId} is already inactive", mappingId);
                throw new Exception($"Mapping ID {mappingId} is already inactive");
            }

            if (mapping.StaffDevice.IsOnline || mapping.PatronDevice.IsOnline)
            {
                _logger.LogWarning("[DeleteMappingAsync]: Cannot delete mapping ID {MappingId} because one or both devices are online", mappingId);
                throw new Exception("Cannot delete mapping because one or both devices are online");
            }

            mapping.IsActive = false;
            mapping.IsDelete = true;
            mapping.UpdatedAt = DateTime.Now;
            _deviceMappingRepository.Update(mapping);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("[DeleteMappingAsync]: Mapping ID {MappingId} deactivated", mappingId);
            return true;
        }

        public async Task<PatronDevice?> GetDeviceByNameAsync(string deviceName)
        {
            return await _patronDeviceRepository
                .FirstOrDefaultAsync(d => d.DeviceName == deviceName);
        }

        public async Task<PatronDevice?> GetDeviceByConnectionIdAsync(string connectionId)
        {
            return await _patronDeviceRepository
                .FirstOrDefaultAsync(d => d.ConnectionId == connectionId);
        }
        #endregion

        public async Task<SignatureSession?> GetPendingSessionByPatronIdAsync(int patronId)
        {
            return await _signatureSessionRepository.GetPendingSessionByPatronIdAsync(patronId);
        }

        public async Task<SignatureSession?> GetCompletedSessionByPatronIdAsync(int patronId)
        {
            return await _signatureSessionRepository.GetCompletedSessionByPatronIdAsync(patronId);
        }

        public async Task<SignatureSession> GetSignatureSessionAsync(int patronId, int staffId, int patronDeviceId)
        {
            return await _signatureSessionRepository.GetSignatureSessionAsync(patronId, staffId, patronDeviceId);
        }

        public async Task<bool> IsSignedCompletedAsync(int patronId)
        {
            var session = await _signatureSessionRepository.FirstOrDefaultAsync(s => s.PatronId == patronId && s.CompletedAt.HasValue);

            if (session == null)
            {
                _logger.LogWarning("Signature session with PatronId is {PatronId} not found", patronId);
                return false;
            }

            return session.Status == SignatureSessionStatus.Signed;
        }

        public async Task<int?> FindNearestStaffDeviceByIpAsync(string ipAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    _logger.LogWarning("[FindNearestStaffDeviceByIpAsync]: IP address is null or empty");
                    return null;
                }

                _logger.LogInformation("[FindNearestStaffDeviceByIpAsync]: Finding staff device for IP: {IpAddress}", ipAddress);

                // Get all active staff devices
                var staffDevices = await _staffDeviceRepository
                    .FindAsync(sd => sd.IsActive && !sd.IsDelete && !string.IsNullOrEmpty(sd.IpAddress));

                if (!staffDevices.Any())
                {
                    _logger.LogWarning("[FindNearestStaffDeviceByIpAsync]: No active staff devices found");
                    return null;
                }

                // Try exact IP match first
                var exactMatch = staffDevices.FirstOrDefault(sd => sd.IpAddress == ipAddress);
                if (exactMatch != null)
                {
                    _logger.LogInformation("[FindNearestStaffDeviceByIpAsync]: Found exact IP match with StaffDevice ID: {StaffDeviceId}", exactMatch.Id);
                    return exactMatch.Id;
                }

                // If localhost or loopback, match with any localhost staff device
                if (IsLocalhost(ipAddress))
                {
                    var localhostMatch = staffDevices.FirstOrDefault(sd => IsLocalhost(sd.IpAddress));
                    if (localhostMatch != null)
                    {
                        _logger.LogInformation("[FindNearestStaffDeviceByIpAsync]: Found localhost match with StaffDevice ID: {StaffDeviceId}", localhostMatch.Id);
                        return localhostMatch.Id;
                    }
                }

                // Try subnet matching (same network)
                var subnetMatch = FindBySubnet(ipAddress, staffDevices);
                if (subnetMatch != null)
                {
                    _logger.LogInformation("[FindNearestStaffDeviceByIpAsync]: Found subnet match with StaffDevice ID: {StaffDeviceId}, IP: {StaffIp}",
                        subnetMatch.Id, subnetMatch.IpAddress);
                    return subnetMatch.Id;
                }

                // Fallback: Return the most recently active staff device
                var fallbackDevice = staffDevices
                    .OrderByDescending(sd => sd.LastHeartbeat ?? sd.UpdatedAt)
                    .FirstOrDefault();

                if (fallbackDevice != null)
                {
                    _logger.LogWarning("[FindNearestStaffDeviceByIpAsync]: No IP match found, using most recent active device ID: {StaffDeviceId}",
                        fallbackDevice.Id);
                    return fallbackDevice.Id;
                }

                _logger.LogWarning("[FindNearestStaffDeviceByIpAsync]: No suitable staff device found for IP: {IpAddress}", ipAddress);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FindNearestStaffDeviceByIpAsync]: Error finding staff device for IP: {IpAddress}", ipAddress);
                return null;
            }
        }

        #region Staff Device Connection Management
        public async Task<List<StaffDevice>> GetOnlineStaffDevicesAsync()
        {
            // _logger.LogInformation("[GetOnlineStaffDevicesAsync]: Retrieving online staff devices...");

            var devices = await _staffDeviceRepository.GetOnlineDevicesAsync();

            // _logger.LogInformation("[GetOnlineStaffDevicesAsync]: Found {Count} online staff devices", devices.Count);

            return devices;
        }

        public async Task<bool> UpdateStaffDeviceConnectionIdAsync(string deviceName, string connectionId)
        {
            _logger.LogInformation("[UpdateStaffDeviceConnectionId]: DeviceName={DeviceName}, ConnectionId={ConnectionId}",
                deviceName, connectionId);

            var result = await _staffDeviceRepository.UpdateConnectionIdAsync(deviceName, connectionId);

            if (result)
            {
                _logger.LogInformation("[UpdateStaffDeviceConnectionId]: Updated successfully");
            }
            else
            {
                _logger.LogWarning("[UpdateStaffDeviceConnectionId]: ❌ StaffDevice '{DeviceName}' not found", deviceName);
            }

            return result;
        }

        public async Task SetStaffDeviceOfflineAsync(string connectionId)
        {
            _logger.LogInformation("[SetStaffDeviceOffline]: ConnectionId={ConnectionId}", connectionId);

            await _staffDeviceRepository.SetOfflineByConnectionIdAsync(connectionId);

            _logger.LogInformation("[SetStaffDeviceOffline]: Device set offline");
        }

        public async Task<StaffDevice?> GetStaffDeviceByConnectionIdAsync(string connectionId)
        {
            return await _staffDeviceRepository.GetByConnectionIdAsync(connectionId);
        }

        public async Task<StaffDevice?> GetStaffDeviceByHostNameAsync(string hostName)
        {
            return await _staffDeviceRepository.FirstOrDefaultAsync(x => x.DeviceName == hostName);
        }

        public async Task<StaffDevice> CreateOrUpdateStaffDevice(string hostName, string ipAddress, string employeeUserName)
        {
            StaffDevice? staffDevice = null;

            // PRIORITY 1: Find by IP Address
            if (!string.IsNullOrEmpty(ipAddress))
            {
                staffDevice = await _staffDeviceRepository.FirstOrDefaultAsync(s => s.IpAddress == ipAddress && s.IsActive);

                if (staffDevice != null)
                {
                    _logger.LogInformation("[CreateOrUpdateStaffDevice]: Found device by IP: {IpAddress}, updating hostname from '{OldHostname}' to '{NewHostname}'",
                        ipAddress, staffDevice.DeviceName, hostName);
                }
            }

            // PRIORITY 2: Fallback to hostname
            if (staffDevice == null)
            {
                staffDevice = await _staffDeviceRepository.FirstOrDefaultAsync(s => s.DeviceName == hostName && s.IsActive);

                if (staffDevice != null)
                {
                    _logger.LogInformation("[CreateOrUpdateStaffDevice]: Found device by hostname: {HostName}", hostName);
                }
            }

            // Update existing device
            if (staffDevice != null)
            {
                staffDevice.DeviceName = hostName; // Always update hostname
                staffDevice.IpAddress = ipAddress; // Always update IP
                staffDevice.LastHeartbeat = DateTime.Now;
                staffDevice.UpdatedAt = DateTime.Now;

                if (staffDevice.StaffUserName == "Unknown_Employee" && employeeUserName != "Unknown_Employee")
                {
                    staffDevice.StaffUserName = employeeUserName;
                }

                _staffDeviceRepository.Update(staffDevice);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("[CreateOrUpdateStaffDevice]: Updated device ID: {DeviceId}, Hostname: {HostName}, IP: {IpAddress}",
                    staffDevice.Id, hostName, ipAddress);

                return staffDevice;
            }

            // Create new device
            _logger.LogInformation("[CreateOrUpdateStaffDevice]: Creating new device Hostname: {HostName}, IP: {IpAddress}", hostName, ipAddress);

            var createNewStaffDevice = new StaffDevice
            {
                DeviceName = hostName,
                IpAddress = ipAddress,
                MacAddress = ipAddress, // Use IP as fallback for MacAddress
                IsOnline = true,
                LastHeartbeat = DateTime.Now,
                StaffUserName = employeeUserName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = CommonConstants.SystemUser,
                IsActive = true,
                IsDelete = false,
                UpdatedBy = CommonConstants.SystemUser,
            };

            await _staffDeviceRepository.AddAsync(createNewStaffDevice);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("[CreateOrUpdateStaffDevice]: Created new device ID: {DeviceId}", createNewStaffDevice.Id);

            return createNewStaffDevice;
        }

        public async Task<StaffDevice> GetStaffDeviceByIdAsync(int staffDeviceId)
        {
            return await _staffDeviceRepository.FirstOrDefaultAsync(x => x.Id == staffDeviceId);
        }
        #endregion

        #region Private Function
        private bool IsLocalhost(string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            return ipAddress == "127.0.0.1" ||
                   ipAddress == "::1" ||
                   ipAddress.StartsWith("localhost", StringComparison.OrdinalIgnoreCase) ||
                   ipAddress == "0.0.0.0";
        }

        private StaffDevice? FindBySubnet(string patronIp, IEnumerable<StaffDevice> staffDevices)
        {
            try
            {
                // Parse patron IP
                if (!System.Net.IPAddress.TryParse(patronIp, out var patronIpAddress))
                {
                    _logger.LogWarning("[FindBySubnet]: Invalid IP format: {IpAddress}", patronIp);
                    return null;
                }

                // Try to find device in same /24 subnet (e.g., 192.168.1.x)
                foreach (var device in staffDevices)
                {
                    if (string.IsNullOrWhiteSpace(device.IpAddress))
                        continue;

                    if (!System.Net.IPAddress.TryParse(device.IpAddress, out var staffIpAddress))
                        continue;

                    // Only compare IPv4 addresses
                    if (patronIpAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork ||
                        staffIpAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        continue;

                    // Check if they're in the same /24 subnet
                    if (IsSameSubnet(patronIpAddress, staffIpAddress, 24))
                    {
                        _logger.LogInformation("[FindBySubnet]: Found device in same subnet - Patron: {PatronIp}, Staff: {StaffIp}",
                            patronIp, device.IpAddress);
                        return device;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FindBySubnet]: Error during subnet matching");
                return null;
            }
        }

        private bool IsSameSubnet(System.Net.IPAddress ip1, System.Net.IPAddress ip2, int prefixLength)
        {
            try
            {
                var bytes1 = ip1.GetAddressBytes();
                var bytes2 = ip2.GetAddressBytes();

                if (bytes1.Length != bytes2.Length)
                    return false;

                // Calculate how many full bytes to compare
                int fullBytes = prefixLength / 8;
                int remainingBits = prefixLength % 8;

                // Compare full bytes
                for (int i = 0; i < fullBytes; i++)
                {
                    if (bytes1[i] != bytes2[i])
                        return false;
                }

                // Compare remaining bits if any
                if (remainingBits > 0 && fullBytes < bytes1.Length)
                {
                    int mask = 0xFF << (8 - remainingBits);
                    if ((bytes1[fullBytes] & mask) != (bytes2[fullBytes] & mask))
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}