using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class CreateLanguageRequest
    {
        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string NativeName { get; set; } = string.Empty;

        public string? FlagEmoji { get; set; }

        public int SortOrder { get; set; } = 0;
    }

    public class UpdateLanguageRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string NativeName { get; set; } = string.Empty;

        public string? FlagEmoji { get; set; }

        public int SortOrder { get; set; } = 0;
    }

    public class CreatePatronTypeRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ColorHex { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int SortOrder { get; set; } = 0;
    }

    public class UpdatePatronTypeRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ColorHex { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int SortOrder { get; set; } = 0;
    }
}
