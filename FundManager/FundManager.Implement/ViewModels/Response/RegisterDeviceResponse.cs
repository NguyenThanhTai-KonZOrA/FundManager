namespace DigitalDocumentPlatform.Implement.ViewModels.Response
{
    public class RegisterDeviceResponse
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public bool IsAvailable { get; set; }
        public string MacAddress { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}