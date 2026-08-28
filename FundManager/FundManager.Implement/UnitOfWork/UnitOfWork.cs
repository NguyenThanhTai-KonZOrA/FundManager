using FundManager.DataAccess.ApplicationDbContext;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace FundManager.Implement.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FundManagerDbContext _context;
        private IDbContextTransaction? _transaction;

        public IAuditLogRepository AuditLogs { get; }
        public IEmployeeRepository Employees { get; }
        public IRoleRepository Roles { get; }
        public IPermissionRepository Permissions { get; }
        public IEmployeeRoleRepository EmployeeRoles { get; }
        public IRolePermissionRepository RolePermissions { get; }
        public IApplicationSettingsRepository ApplicationSettings { get; }
        public IApplicationImageRepository ApplicationImages { get; }
        public UnitOfWork(FundManagerDbContext context,
            IAuditLogRepository auditLogs,
            IEmployeeRepository employees,
            IRoleRepository roles,
            IPermissionRepository permissions,
            IEmployeeRoleRepository employeeRoles,
            IRolePermissionRepository rolePermissions,
            IApplicationSettingsRepository applicationSettings,
            IApplicationImageRepository applicationImages
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
            ApplicationImages = applicationImages;
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