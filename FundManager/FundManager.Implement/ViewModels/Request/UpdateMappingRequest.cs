namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class UpdateMappingRequest
    {
        public int Id { get; set; }
        public string? NewStaffDeviceName { get; set; }
        public string? NewPatronDeviceName { get; set; }
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public string? Notes { get; set; }
    }
}