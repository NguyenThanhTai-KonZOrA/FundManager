namespace FundManager.Implement.ViewModels.Response
{
    public class CustomerSessionSubmitResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PatronId { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
