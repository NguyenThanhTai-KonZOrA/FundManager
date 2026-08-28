namespace FundManager.Implement.ViewModels.Response
{
    public class StaffAndPatronDevicesResponse
    {
        public int TotalStaffDevices { get; set; }
        public int TotalPatronDevices { get; set; }
        public List<StaffDeviceResponse> StaffDevices { get; set; } = new List<StaffDeviceResponse>();
        public List<PatronDeviceResponse> PatronDevices { get; set; } = new List<PatronDeviceResponse>();
    }

    public class BaseDeviceResponse
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public bool IsActive { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public string StaffUserName { get; set; } = string.Empty;
        public DateTime? LastHeartbeat { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class StaffDeviceResponse : BaseDeviceResponse
    {
        public string DeviceType { get; set; } = "Staff";

    }

    public class PatronDeviceResponse : BaseDeviceResponse
    {
        public string DeviceType { get; set; } = "Patron";
    }
}