using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IWorkflowRepository : IGenericRepository<WorkflowDefinition>
    {
        Task<WorkflowDefinition?> GetActiveByOutletIdAsync(int outletId);
        Task<WorkflowDefinition?> GetDefaultWorkflowAsync();
        Task<WorkflowDefinition?> GetByIdWithStepsAsync(int id);
        Task<List<WorkflowDefinition>> GetAllWithStepsAsync();
    }
}
