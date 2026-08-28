using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace DigitalDocumentPlatform.Implement.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DigitalDocumentPlatformDbContext _context;
        private IDbContextTransaction? _transaction;

        public IAuditLogRepository AuditLogs { get; }
        public IEmployeeRepository Employees { get; }
        public IRoleRepository Roles { get; }
        public IPermissionRepository Permissions { get; }
        public IEmployeeRoleRepository EmployeeRoles { get; }
        public IRolePermissionRepository RolePermissions { get; }
        public IApplicationSettingsRepository ApplicationSettings { get; }
        public IPropertyRepository Properties { get; }
        public IOutletRepository Outlets { get; }
        public IPropertyOutletRepository PropertyOutlets { get; }
        public IApplicationImageRepository ApplicationImages { get; }
        public IStaffDeviceRepository StaffDevices { get; }
        public IPatronDeviceRepository PatronDevices { get; }
        public IFormTemplateRepository FormTemplates { get; }
        public IFormQuestionRepository FormQuestions { get; }
        public IFormSubmissionRepository FormSubmissions { get; }
        public IWorkflowRepository Workflows { get; }
        public IDocumentTemplateRepository DocumentTemplates { get; }
        public ILanguageRepository Languages { get; }
        public IPatronTypeRepository PatronTypes { get; }

        public UnitOfWork(DigitalDocumentPlatformDbContext context,
            IAuditLogRepository auditLogs,
            IEmployeeRepository employees,
            IRoleRepository roles,
            IPermissionRepository permissions,
            IEmployeeRoleRepository employeeRoles,
            IRolePermissionRepository rolePermissions,
            IApplicationSettingsRepository applicationSettings,
            IPropertyRepository properties,
            IOutletRepository outlets,
            IPropertyOutletRepository propertyOutlets,
            IApplicationImageRepository applicationImages,
            IStaffDeviceRepository staffDevices,
            IPatronDeviceRepository patronDevices,
            IFormTemplateRepository formTemplates,
            IFormQuestionRepository formQuestions,
            IFormSubmissionRepository formSubmissions,
            IWorkflowRepository workflows,
            IDocumentTemplateRepository documentTemplates,
            ILanguageRepository languages,
            IPatronTypeRepository patronTypes
            )
        {
            _context = context;
            AuditLogs = auditLogs;
            Employees = employees;
            Roles = roles;
            Permissions = permissions;
            EmployeeRoles = employeeRoles;
            RolePermissions = rolePermissions;
            ApplicationSettings = applicationSettings;
            Properties = properties;
            Outlets = outlets;
            PropertyOutlets = propertyOutlets;
            ApplicationImages = applicationImages;
            StaffDevices = staffDevices;
            PatronDevices = patronDevices;
            FormTemplates = formTemplates;
            FormQuestions = formQuestions;
            FormSubmissions = formSubmissions;
            Workflows = workflows;
            DocumentTemplates = documentTemplates;
            Languages = languages;
            PatronTypes = patronTypes;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}