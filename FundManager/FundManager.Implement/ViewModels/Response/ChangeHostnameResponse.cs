namespace FundManager.Implement.ViewModels.Response
{
    public class ChangeHostnameResponse
    {
        public int Id { get; set; }
        public string OldHostname { get; set; } = string.Empty;
        public string NewHostname { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}