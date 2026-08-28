using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    public class Country : BaseEntity
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        [StringLength(20)]
        public string Abrv2 { get; set; } = string.Empty;
        [StringLength(20)]
        public string Abrv3 { get; set; } = string.Empty;
    }
}