using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class WorkflowRepository : GenericRepository<WorkflowDefinition>, IWorkflowRepository
    {
        public WorkflowRepository(DigitalDocumentPlatformDbContext context) : base(context) { }

        public async Task<WorkflowDefinition?> GetActiveByOutletIdAsync(int outletId)
        {
            return await _dbSet
                .Include(w => w.Steps.Where(s => !s.IsDelete).OrderBy(s => s.StepOrder))
                    .ThenInclude(s => s.FormTemplate)
                .FirstOrDefaultAsync(w => w.OutletId == outletId && w.IsActive && !w.IsDelete);
        }

        public async Task<WorkflowDefinition?> GetDefaultWorkflowAsync()
        {
            return await _dbSet
                .Include(w => w.Steps.Where(s => !s.IsDelete).OrderBy(s => s.StepOrder))
                    .ThenInclude(s => s.FormTemplate)
                .FirstOrDefaultAsync(w => w.Name == CommonConstants.DefaultWorkflowName && w.IsActive && !w.IsDelete);
        }

        public async Task<WorkflowDefinition?> GetByIdWithStepsAsync(int id)
        {
            return await _dbSet
                .Include(w => w.Outlet)
                .Include(w => w.Steps.Where(s => !s.IsDelete).OrderBy(s => s.StepOrder))
                    .ThenInclude(s => s.FormTemplate)
                .AsSplitQuery()
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDelete);
        }

        public async Task<List<WorkflowDefinition>> GetAllWithStepsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(w => w.Outlet)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.FormTemplate)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.DocumentTemplate)
                .AsSplitQuery()
                .Where(w => !w.IsDelete)
                .OrderBy(w => w.Outlet.Name)
                .ThenBy(w => w.Name)
                .ToListAsync();
        }
    }
}
