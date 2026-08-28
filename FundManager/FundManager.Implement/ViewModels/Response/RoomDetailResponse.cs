namespace FundManager.Implement.ViewModels.Response
{
    public class RoomDetailResponse
    {
        public string ResvId { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string RateCode { get; set; } = string.Empty;
        public int TotalGuest { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public int Adults { get; set; }
        public int Child1 { get; set; }
        public int Child2 { get; set; }
        public string SpecialRequests { get; set; } = string.Empty;
        public List<SharerInfo> Sharers { get; set; } = new List<SharerInfo>();
    }

    public class SharerInfo
    {
        public string Name { get; set; } = string.Empty;
        public string GuestLabel { get; set; } = string.Empty;
        public int? PlayerId { get; set; }
    }
}