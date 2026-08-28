using DigitalDocumentPlatform.Common.BaseEntity;
using DigitalDocumentPlatform.Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    public class ApplicationImage : BaseEntity
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;
        [StringLength(500)]
        public string FileUrl { get; set; } = string.Empty;
        [StringLength(10)]
        public string FileExtension { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public ImageTypeEnum Type { get; set; }
        public int? PropertyId { get; set; }
        public int? OutletId { get; set; }
    }
}