namespace FundManager.Implement.ViewModels.Response
{
    public class StaffDeviceRegistrationResponse
    {
        public bool IsRegistered { get; set; }
        public int? StaffDeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? ConnectionId { get; set; }
        public bool IsOnline { get; set; }
        public string? StaffUserName { get; set; }
        public DateTime? LastHeartbeat { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}