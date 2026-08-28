namespace FundManager.Implement.ViewModels.Response
{
    /// <summary>
    /// Represents a StaffDevice assigned to an Outlet, including its paired PatronDevice if any.
    /// </summary>
    public class OutletStaffDeviceResponse
    {
        public int StaffDeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? StaffUserName { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastHeartbeat { get; set; }
        public int? OutletId { get; set; }

        /// <summary>Paired PatronDevice via DeviceMapping, if any.</summary>
        public PairedPatronDeviceResponse? PairedPatronDevice { get; set; }
    }

    public class PairedPatronDeviceResponse
    {
        public int DeviceMappingId { get; set; }
        public int PatronDeviceId { get; set; }
        public string PatronDeviceName { get; set; } = string.Empty;
        public string? PatronIpAddress { get; set; }
        public bool PatronIsOnline { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
}
