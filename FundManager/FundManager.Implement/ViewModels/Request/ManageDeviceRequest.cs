namespace FundManager.Implement.ViewModels.Request
{
    public class ToggleDeviceRequest
    {
        public int DeviceId { get; set; }
        public string DeviceType { get; set; } = string.Empty; // "Staff" or "Patron"
        public bool IsActive { get; set; }
    }

    public class DeleteDeviceRequest
    {
        public int DeviceId { get; set; }
        public string DeviceType { get; set; } = string.Empty; // "Staff" or "Patron"
    }

    public class ChangeHostnameRequest
    {
        public int DeviceId { get; set; }
        public string DeviceType { get; set; } = string.Empty; // "Staff" or "Patron"
        public string NewHostname { get; set; } = string.Empty;
    }
}
