using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;
using Microsoft.Extensions.Logging;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WorkflowService> _logger;

        public WorkflowService(IUnitOfWork unitOfWork, ILogger<WorkflowService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<WorkflowResponse>> GetAllAsync()
        {
            _logger.LogInformation("[WorkflowService.GetAllAsync]");
            var workflows = await _unitOfWork.Workflows.GetAllWithStepsAsync();
            return workflows.Select(MapToResponse).ToList();
        }

        public async Task<WorkflowResponse?> GetByIdAsync(int id)
        {
            _logger.LogInformation("[WorkflowService.GetByIdAsync]: id={Id}", id);
            var workflow = await _unitOfWork.Workflows.GetByIdWithStepsAsync(id);
            return workflow == null ? null : MapToResponse(workflow);
        }

        public async Task<WorkflowResponse?> GetByOutletIdAsync(int outletId)
        {
            _logger.LogInformation("[WorkflowService.GetByOutletIdAsync]: outletId={OutletId}", outletId);
            var workflow = await _unitOfWork.Workflows.GetActiveByOutletIdAsync(outletId);
            return workflow == null ? null : MapToResponse(workflow);
        }

        public async Task<WorkflowResponse> CreateAsync(CreateWorkflowRequest request, string createdBy)
        {
            _logger.LogInformation("[WorkflowService.CreateAsync]: name={Name}, outletId={OutletId}", request.Name, request.OutletId);

            var entity = new WorkflowDefinition
            {
                Name = request.Name,
                Description = request.Description,
                OutletId = request.OutletId,
                IsActive = true,
                IsDelete = false,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Steps = request.Steps.Select(s => new WorkflowStep
                {
                    StepOrder = s.StepOrder,
                    StepType = s.StepType,
                    StepLabel = s.StepLabel,
                    FormTemplateId = s.FormTemplateId,
                    DocumentTemplateId = s.DocumentTemplateId,
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = createdBy,
                    UpdatedBy = createdBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList()
            };

            await _unitOfWork.Workflows.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.Workflows.GetByIdWithStepsAsync(entity.Id);
            return MapToResponse(created!);
        }

        public async Task<WorkflowResponse> UpdateAsync(UpdateWorkflowRequest request, string updatedBy)
        {
            _logger.LogInformation("[WorkflowService.UpdateAsync]: id={Id}", request.Id);
            var entity = await _unitOfWork.Workflows.GetByIdWithStepsAsync(request.Id)
                ?? throw new KeyNotFoundException($"WorkflowDefinition {request.Id} not found.");

            entity.Name = request.Name;
            entity.OutletId = request.OutletId;
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;

            // Soft-delete old steps, then add new ones
            foreach (var step in entity.Steps)
            {
                step.IsDelete = true;
                step.UpdatedBy = updatedBy;
                step.UpdatedAt = DateTime.Now;
            }
            foreach (var s in request.Steps)
            {
                entity.Steps.Add(new WorkflowStep
                {
                    StepOrder = s.StepOrder,
                    StepType = s.StepType,
                    StepLabel = s.StepLabel,
                    FormTemplateId = s.FormTemplateId,
                    DocumentTemplateId = s.DocumentTemplateId,
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = updatedBy,
                    UpdatedBy = updatedBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            _unitOfWork.Workflows.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.Workflows.GetByIdWithStepsAsync(entity.Id);
            return MapToResponse(updated!);
        }

        public async Task DeleteAsync(int id, string deletedBy)
        {
            _logger.LogInformation("[WorkflowService.DeleteAsync]: id={Id}", id);
            var entity = await _unitOfWork.Workflows.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"WorkflowDefinition {id} not found.");

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.UpdatedBy = deletedBy;
            entity.UpdatedAt = DateTime.Now;
            _unitOfWork.Workflows.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private static WorkflowResponse MapToResponse(WorkflowDefinition w) => new()
        {
            Id = w.Id,
            Name = w.Name,
            Description = w.Description,
            OutletId = w.OutletId,
            OutletName = w.Outlet?.Name ?? string.Empty,
            IsActive = w.IsActive,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt,
            Steps = w.Steps
                .Where(s => !s.IsDelete)
                .OrderBy(s => s.StepOrder)
                .Select(s => new WorkflowStepResponse
                {
                    Id = s.Id,
                    StepOrder = s.StepOrder,
                    StepType = s.StepType,
                    StepLabel = s.StepLabel,
                    FormTemplateId = s.FormTemplateId,
                    FormTemplateTitle = s.FormTemplate?.Title,
                    DocumentTemplateId = s.DocumentTemplateId,
                    DocumentTemplateTitle = s.DocumentTemplate?.Title
                }).ToList()
        };
    }
}
