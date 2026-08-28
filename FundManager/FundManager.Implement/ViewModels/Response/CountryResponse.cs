namespace FundManager.Implement.ViewModels.Response
{
    public class CountryResponse
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Abrv2 { get; set; } = string.Empty;
        public string Abrv3 { get; set; } = string.Empty;
    }
}