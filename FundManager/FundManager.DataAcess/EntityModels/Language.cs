using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// Lookup table for supported languages.
    /// Code follows IETF BCP 47 (e.g. "en", "vi", "ko", "zh", "ja").
    /// </summary>
    public class Language : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>IETF language tag, e.g. "en", "vi", "ko".</summary>
        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        /// <summary>Human-readable name in English, e.g. "English".</summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Native name, e.g. "Tiếng Việt".</summary>
        [StringLength(100)]
        public string NativeName { get; set; } = string.Empty;

        /// <summary>Unicode flag emoji or image path.</summary>
        [StringLength(100)]
        public string? FlagEmoji { get; set; }

        /// <summary>Display order in language selector.</summary>
        public int SortOrder { get; set; } = 0;
    }
}