namespace DigitalDocumentPlatform.Implement.ViewModels.Response
{
    public class CreateOrUpdateMappingResponse
    {
        public int Id { get; set; }
        public int StaffDeviceId { get; set; }
        public string? StaffDeviceName { get; set; }
        public int PatronDeviceId { get; set; }
        public string? PatronDeviceName { get; set; }
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastVerified { get; set; }

    }
}