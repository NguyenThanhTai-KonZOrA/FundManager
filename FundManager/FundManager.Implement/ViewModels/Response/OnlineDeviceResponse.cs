namespace FundManager.Implement.ViewModels.Response
{
    public class BaseOnlineDeviceResponse
    {
        public int Id { get; set; }
        public string? DeviceName { get; set; }
        public string? ConnectionId { get; set; }
        public string? IpAddress { get; set; }
        public bool IsOnline { get; set; }
    }

    public class OnlineStaffDeviceResponse : BaseOnlineDeviceResponse
    {
        public string? StaffUserName { get; set; }
    }

    public class OnlinePatronDeviceResponse : BaseOnlineDeviceResponse
    {

    }
}