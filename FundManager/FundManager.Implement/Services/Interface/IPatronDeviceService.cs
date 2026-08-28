using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;

namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface IPatronDeviceService
    {
        Task<PatronDevice> RegisterDeviceAsync(string deviceName, string connectionId, string? macAddress, string? ipAddress);
        Task SetDeviceOfflineAsync(string connectionId);
        Task<PatronDevice?> GetAvailableDeviceForStaffAsync(int staffDeviceId);
        Task<SignatureSession> CreateSignatureSessionAsync(int patronId, int staffDeviceId, int patronDeviceId);
        Task<(SignatureSession, Patron)> CompleteSignatureSessionAsync(int sessionId, string signatureDataUrl);
        Task UpdateHeartbeatAsync(string connectionId, string deviceType);
        Task<List<PatronDevice>> GetOnlineDevicesAsync();

        // UPDATED: Remove staffDeviceId parameter
        Task<PatronDevice> AddOrUpdatePatronDeviceAsync(string deviceName, string connectionId, string? macAddress, string? ipAddress);
        Task<PatronDevice?> GetDeviceByNameAsync(string deviceName);
        Task<PatronDevice?> GetDeviceByConnectionIdAsync(string connectionId);
        Task<bool> UpdateConnectionIdAsync(string deviceName, string newConnectionId);
        Task<StaffDevice?> GetStaffDeviceByHostNameAsync(string hostName);
        Task<StaffDevice> CreateOrUpdateStaffDevice(string hostName, string ipAdress, string employeeUserName);
        Task<bool> IsSignedCompletedAsync(int patronId);

        // Device Mapping methods
        Task<CreateOrUpdateMappingResponse> CreateOrUpdateMappingAsync(CreateMappingRequest request);
        Task<DeviceMapping?> GetMappingByStaffDeviceNameAsync(string staffDeviceName);
        Task<DeviceMapping?> GetMappingByPatronDeviceNameAsync(string patronDeviceName);
        Task<int?> GetStaffDeviceIdByPatronDeviceNameAsync(string patronDeviceName);
        Task<int?> GetPatronDeviceIdByStaffDeviceNameAsync(string staffDeviceName);
        Task<List<DeviceMappingSettingsResponse>> GetAllActiveMappingsAsync();
        Task<bool> DeleteMappingAsync(int mappingId);
        Task<CreateOrUpdateMappingResponse> UpdateMappingAsync(UpdateMappingRequest request);
        Task<StaffAndPatronDevicesResponse> GetAllStaffAndPatronDevicesAsync();
        Task<SignatureSession> GetSignatureSessionAsync(int patronId, int staffId, int patronDeviceId);
        Task<SignatureSession?> GetPendingSessionByPatronIdAsync(int patronId);
        Task<SignatureSession?> GetCompletedSessionByPatronIdAsync(int patronId);

        // Staff device connection management
        Task<bool> UpdateStaffDeviceConnectionIdAsync(string deviceName, string connectionId);
        Task SetStaffDeviceOfflineAsync(string connectionId);
        Task<StaffDevice?> GetStaffDeviceByConnectionIdAsync(string connectionId);
        Task<List<StaffDevice>> GetOnlineStaffDevicesAsync();
        Task<StaffDevice> GetStaffDeviceByIdAsync(int staffDeviceId);
        Task<DeviceMappingResponse> GetFullInformationMappingByPatronDeviceNameAsync(string patronDeviceName);

        // Manage device operations (previously handled directly in controller)
        Task<bool> ToggleDeviceActiveAsync(int deviceId, string deviceType, bool isActive);
        Task<bool> DeleteDeviceAsync(int deviceId, string deviceType);
        Task<ChangeHostnameResponse> ChangeDeviceHostnameAsync(int deviceId, string deviceType, string newHostname);
        Task<StaffAndPatronDevicesSummaryResponse> GetAllStaffAndPatronDevicesSummaryAsync();
    }
}