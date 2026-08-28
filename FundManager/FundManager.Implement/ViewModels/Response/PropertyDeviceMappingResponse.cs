namespace DigitalDocumentPlatform.Implement.ViewModels.Response
{
    public class PropertyDeviceMappingResponse
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyCode { get; set; } = string.Empty;
        public string PropertyColor { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastVerified { get; set; }
        public List<OutletResponse> Outlets { get; set; } = new List<OutletResponse>();
    }
}