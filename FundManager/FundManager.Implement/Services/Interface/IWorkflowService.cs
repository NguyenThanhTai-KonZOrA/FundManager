using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;

namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface IWorkflowService
    {
        Task<List<WorkflowResponse>> GetAllAsync();
        Task<WorkflowResponse?> GetByIdAsync(int id);
        Task<WorkflowResponse?> GetByOutletIdAsync(int outletId);
        Task<WorkflowResponse> CreateAsync(CreateWorkflowRequest request, string createdBy);
        Task<WorkflowResponse> UpdateAsync(UpdateWorkflowRequest request, string updatedBy);
        Task DeleteAsync(int id, string deletedBy);
    }
}
