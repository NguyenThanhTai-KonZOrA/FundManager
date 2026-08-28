using FundManager.DataAccess.EntityModels;
using FundManager.DataAccess.Initalizer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FundManager.DataAccess.ApplicationDbContext
{
    public class FundManagerDbContext : DbContext
    {
        public FundManagerDbContext(DbContextOptions<FundManagerDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }

        // Role and Permission tables
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        // Audit Logs table
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
        public DbSet<ApplicationImage> ApplicationImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Suppress PendingModelChangesWarning for seed data
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // All IEntityTypeConfiguration<T> implementations in this assembly are applied automatically
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            ApplicationSettingsSeed.Seed(modelBuilder.Entity<ApplicationSettings>());
            ApplicationImageSeed.Seed(modelBuilder.Entity<ApplicationImage>());
            RoleSeed.Seed(modelBuilder.Entity<Role>());
            PermissionSeed.Seed(modelBuilder.Entity<Permission>());
            RolePermissionSeed.Seed(modelBuilder.Entity<RolePermission>());
            EmployeeRoleSeed.Seed(modelBuilder.Entity<EmployeeRole>());
            EmployeeSeed.Seed(modelBuilder.Entity<Employee>());
        }
    }
}