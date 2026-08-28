namespace FundManager.Implement.ViewModels.Response
{
    public class StaffAndPatronDevicesSummaryResponse
    {
        public List<StaffDeviceSummaryResponse> StaffDevices { get; set; } = new List<StaffDeviceSummaryResponse>();
        public List<PatronDeviceSummaryResponse> PatronDevices { get; set; } = new List<PatronDeviceSummaryResponse>();
    }

    public class StaffDeviceSummaryResponse
    {
        public int StaffDeviceId { get; set; }
        public string StaffDeviceName { get; set; } = string.Empty;

    }

    public class PatronDeviceSummaryResponse
    {
        public int PatronDeviceId { get; set; }
        public string PatronDeviceName { get; set; } = string.Empty;
    }
}