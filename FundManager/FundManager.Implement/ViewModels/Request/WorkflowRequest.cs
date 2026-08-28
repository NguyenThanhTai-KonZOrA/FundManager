using DigitalDocumentPlatform.DataAccess.EntityModels;
using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class CreateWorkflowRequest
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int OutletId { get; set; }

        /// <summary>Ordered list of steps to create together with the workflow.</summary>
        public List<WorkflowStepItem> Steps { get; set; } = [];
    }

    public class UpdateWorkflowRequest
    {
        [Required]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        public int OutletId { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>Full replacement of steps (delete + re-create).</summary>
        public List<WorkflowStepItem> Steps { get; set; } = [];
    }

    public class WorkflowStepItem
    {
        public int StepOrder { get; set; }
        public StepType StepType { get; set; } = StepType.FillForm;

        [StringLength(200)]
        public string StepLabel { get; set; } = string.Empty;

        public int? FormTemplateId { get; set; }
        public int? DocumentTemplateId { get; set; }
    }
}
