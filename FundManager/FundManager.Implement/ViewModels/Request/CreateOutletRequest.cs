using System.ComponentModel.DataAnnotations;

namespace FundManager.Implement.ViewModels.Request
{
    public class CreateOutletRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Code { get; set; } = string.Empty;
        public string MainColor { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(1000)]
        public string IconImageUrl { get; set; } = string.Empty;

        [StringLength(1000)]
        public string BackgroundImageUrl { get; set; } = string.Empty;

        /// <summary>IDs of properties this outlet belongs to (many-to-many).</summary>
        public List<int> PropertyIds { get; set; } = [];
    }

    public class UpdateOutletRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Code { get; set; } = string.Empty;
        public string MainColor { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(1000)]
        public string IconImageUrl { get; set; } = string.Empty;

        [StringLength(1000)]
        public string BackgroundImageUrl { get; set; } = string.Empty;

        /// <summary>IDs of properties this outlet belongs to (many-to-many).</summary>
        public List<int> PropertyIds { get; set; } = [];

        public bool IsActive { get; set; }
    }
}