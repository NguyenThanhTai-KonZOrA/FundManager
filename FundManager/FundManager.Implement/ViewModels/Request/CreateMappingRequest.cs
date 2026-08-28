namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class CreateMappingRequest
    {
        public string StaffDeviceName { get; set; } = string.Empty;
        public string PatronDeviceName { get; set; } = string.Empty;
        public int OutletId { get; set; }
        public string? Notes { get; set; }
    }
}