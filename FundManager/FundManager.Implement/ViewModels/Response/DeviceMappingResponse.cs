namespace DigitalDocumentPlatform.Implement.ViewModels.Response
{
    public class DeviceMappingResponse
    {
        public int Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public StaffDeviceData StaffDevice { get; set; } = new StaffDeviceData();
        public PatronDeviceData PatronDevice { get; set; } = new PatronDeviceData();
        public OutletResponse Outlet { get; set; } = new OutletResponse();
        public WorkflowResponse Workflow { get; set; } = new WorkflowResponse();
    }

    public class StaffDeviceData
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string StaffUserName { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }

    public class PatronDeviceData
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class DeviceMappingSettingsResponse
    {
        public int Id { get; set; }
        public int StaffDeviceId { get; set; }
        public string StaffDeviceName { get; set; } = string.Empty;
        public int PatronDeviceId { get; set; }
        public string PatronDeviceName { get; set; } = string.Empty;
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public int PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyCode { get; set; }
        public bool PatronIsOnline { get; set; }
        public bool StaffIsOnline { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastVerified { get; set; }
    }
}