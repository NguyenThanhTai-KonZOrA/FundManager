using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// One selectable option belonging to a FormQuestion (e.g. "Yes", "No", "Tired").
    /// </summary>
    public class FormQuestionOption : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(FormQuestion))]
        public int FormQuestionId { get; set; }
        public FormQuestion FormQuestion { get; set; } = null!;

        /// <summary>The label shown to the patron, e.g. "Tired" or "Yes".</summary>
        [Required]
        [StringLength(200)]
        public string OptionText { get; set; } = string.Empty;

        /// <summary>1-based display order within the question.</summary>
        public int SortOrder { get; set; }
    }
}