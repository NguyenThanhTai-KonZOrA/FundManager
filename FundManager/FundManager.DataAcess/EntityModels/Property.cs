using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    public class Property : BaseEntity
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        // Navigation for many-to-many with Outlet
        public ICollection<PropertyOutlet> PropertyOutlets { get; set; } = [];
    }
}