using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.ViewModels.Response
{
    public class WorkflowResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OutletId { get; set; }
        public string OutletName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<WorkflowStepResponse> Steps { get; set; } = [];
    }

    public class WorkflowStepResponse
    {
        public int Id { get; set; }
        public int StepOrder { get; set; }
        public StepType StepType { get; set; }
        public string StepLabel { get; set; } = string.Empty;
        public int? FormTemplateId { get; set; }
        public string? FormTemplateTitle { get; set; }
        public int? DocumentTemplateId { get; set; }
        public string? DocumentTemplateTitle { get; set; }
    }
}
