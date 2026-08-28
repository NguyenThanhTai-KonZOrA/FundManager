namespace FundManager.Implement.ViewModels.Response
{
    public class UpdateConnectionResponse
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
}