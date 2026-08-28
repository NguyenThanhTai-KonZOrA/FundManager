using FundManager.Implement.Repositories.Interface;

namespace FundManager.Implement.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAuditLogRepository AuditLogs { get; }
        IEmployeeRepository Employees { get; }
        IRoleRepository Roles { get; }
        IPermissionRepository Permissions { get; }
        IEmployeeRoleRepository EmployeeRoles { get; }
        IRolePermissionRepository RolePermissions { get; }
        IApplicationSettingsRepository ApplicationSettings { get; }
        IPropertyRepository Properties { get; }
        IOutletRepository Outlets { get; }
        IPropertyOutletRepository PropertyOutlets { get; }
        IApplicationImageRepository ApplicationImages { get; }
        IStaffDeviceRepository StaffDevices { get; }
        IPatronDeviceRepository PatronDevices { get; }

        // Form template & submission
        IFormTemplateRepository FormTemplates { get; }
        IFormQuestionRepository FormQuestions { get; }
        IFormSubmissionRepository FormSubmissions { get; }

        // Workflow
        IWorkflowRepository Workflows { get; }

        // Document template
        IDocumentTemplateRepository DocumentTemplates { get; }

        // Lookup tables
        ILanguageRepository Languages { get; }
        IPatronTypeRepository PatronTypes { get; }
        /// <summary>
        /// Saves all changes made in this context to the database asynchronously.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();
        /// <summary>
        /// Begins a new transaction asynchronously.
        /// </summary>
        /// <returns></returns>
        Task BeginTransactionAsync();
        /// <summary>
        /// Commits the current transaction asynchronously.
        /// </summary>
        /// <returns></returns>
        Task CommitTransactionAsync();
        /// <summary>
        /// Rolls back the current transaction asynchronously.
        /// </summary>
        /// <returns></returns>
        Task RollbackTransactionAsync();
    }
}
