using FundManager.Common.Enum;

namespace FundManager.Implement.ViewModels.Response
{
    public class ApplicationImageResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public ImageTypeEnum Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public int? PropertyId { get; set; }
        public int? OutletId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}