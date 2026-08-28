using FundManager.Common.Enum;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FundManager.Implement.ViewModels.Request
{
    public class CreateApplicationImageRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>The image file to upload.</summary>
        [Required]
        public IFormFile File { get; set; } = null!;

        [Required]
        public ImageTypeEnum Type { get; set; }

        public int? PropertyId { get; set; }
        public int? OutletId { get; set; }
    }

    /// <summary>Update: form-data; File is optional (keep existing if not supplied).</summary>
    public class UpdateApplicationImageRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>New file to replace the existing one. Leave empty to keep the current file.</summary>
        public IFormFile? File { get; set; }

        [Required]
        public ImageTypeEnum Type { get; set; }

        public int? PropertyId { get; set; }
        public int? OutletId { get; set; }

        public bool IsActive { get; set; }
    }
}