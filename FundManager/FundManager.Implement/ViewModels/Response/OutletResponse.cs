namespace FundManager.Implement.ViewModels.Response
{
    public class OutletResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MainColor { get; set; } = string.Empty;
        public string IconImageUrl { get; set; } = string.Empty;
        public string BackgroundImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        /// <summary>All properties this outlet belongs to (many-to-many).</summary>
        public List<PropertyBriefResponse> Properties { get; set; } = [];
    }

    /// <summary>Lightweight property info used inside OutletResponse to avoid circular reference.</summary>
    public class PropertyBriefResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsPrimaryOutlet { get; set; }
    }
}