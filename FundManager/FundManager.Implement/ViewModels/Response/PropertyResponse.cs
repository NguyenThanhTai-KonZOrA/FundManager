namespace DigitalDocumentPlatform.Implement.ViewModels.Response
{
    public class PropertyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsPrimaryOutlet { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OutletResponse> Outlets { get; set; } = [];
    }
}