using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// One ordered step within a WorkflowDefinition.
    /// </summary>
    public class WorkflowStep : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(WorkflowDefinition))]
        public int WorkflowDefinitionId { get; set; }
        public WorkflowDefinition WorkflowDefinition { get; set; } = null!;

        /// <summary>1-based execution order within the workflow.</summary>
        public int StepOrder { get; set; }

        public StepType StepType { get; set; } = StepType.FillForm;

        /// <summary>Human-readable label for this step, e.g. "Spa Consultation Form".</summary>
        [StringLength(200)]
        public string StepLabel { get; set; } = string.Empty;

        /// <summary>
        /// The FormTemplate the patron must complete at this step.
        /// Required when StepType = FillForm; optional otherwise.
        /// </summary>
        [ForeignKey(nameof(FormTemplate))]
        public int? FormTemplateId { get; set; }
        public FormTemplate? FormTemplate { get; set; }

        /// <summary>
        /// The DocumentTemplate the patron must sign / acknowledge at this step.
        /// Required when StepType = Signature or Acknowledgement; optional otherwise.
        /// </summary>
        [ForeignKey(nameof(DocumentTemplate))]
        public int? DocumentTemplateId { get; set; }
        public DocumentTemplate? DocumentTemplate { get; set; }
    }
}