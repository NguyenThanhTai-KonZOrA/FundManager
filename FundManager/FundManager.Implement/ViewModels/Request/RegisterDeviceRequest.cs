namespace FundManager.Implement.Models.Request
{
    public class RegisterDeviceRequest
    {
        public string DeviceName { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public string? MacAddress { get; set; }
        public string? IpAddress { get; set; }
    }

    public class UpdateConnectionRequest
    {
        public string DeviceName { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public string? MacAddress { get; set; }
        public string? IpAddress { get; set; }
    }
}