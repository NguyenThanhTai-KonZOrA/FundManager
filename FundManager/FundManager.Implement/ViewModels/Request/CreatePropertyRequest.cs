using System.ComponentModel.DataAnnotations;

namespace FundManager.Implement.ViewModels.Request
{
    public class CreatePropertyRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        public List<int> OutletIds { get; set; } = [];
    }

    public class UpdatePropertyRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public List<int> OutletIds { get; set; } = [];
    }
}