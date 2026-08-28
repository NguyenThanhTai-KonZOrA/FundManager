using FundManager.DataAccess.EntityModels;
using FundManager.DataAccess.Initalizer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FundManager.DataAccess.ApplicationDbContext
{
    public class DigitalDocumentPlatformDbContext : DbContext
    {
        public DigitalDocumentPlatformDbContext(DbContextOptions<DigitalDocumentPlatformDbContext> options) : base(options)
        {
        }

        public DbSet<Patron> Patron { get; set; }
        public DbSet<PatronSignature> PatronSignature { get; set; }

        // SignalR-related tables
        public DbSet<StaffDevice> StaffDevices { get; set; }
        public DbSet<PatronDevice> PatronDevices { get; set; }
        public DbSet<SignatureSession> SignatureSessions { get; set; }

        // Device Mapping table
        public DbSet<DeviceMapping> DeviceMappings { get; set; }
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

        // Property and Outlet tables
        public DbSet<Property> Properties { get; set; }
        public DbSet<Outlet> Outlets { get; set; }
        public DbSet<PropertyOutlet> PropertyOutlets { get; set; }
        public DbSet<ApplicationImage> ApplicationImages { get; set; }

        // Dynamic Form tables
        public DbSet<FormTemplate> FormTemplates { get; set; }
        public DbSet<FormQuestion> FormQuestions { get; set; }
        public DbSet<FormQuestionOption> FormQuestionOptions { get; set; }
        public DbSet<FormSubmission> FormSubmissions { get; set; }
        public DbSet<FormSubmissionAnswer> FormSubmissionAnswers { get; set; }

        // Document Template table
        public DbSet<DocumentTemplate> DocumentTemplates { get; set; }

        // Workflow tables
        public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }

        // ─── New managed lookup tables ───────────────────────────────────
        public DbSet<Language> Languages { get; set; }
        public DbSet<PatronType> PatronTypes { get; set; }

        // ─── Version history tables ───────────────────────────────────────
        public DbSet<FormTemplateVersionHistory> FormTemplateVersionHistories { get; set; }
        public DbSet<DocumentTemplateVersionHistory> DocumentTemplateVersionHistories { get; set; }

        // ─── Multilingual translation tables ──────────────────────────────
        public DbSet<FormTemplateTranslation> FormTemplateTranslations { get; set; }
        public DbSet<DocumentTemplateTranslation> DocumentTemplateTranslations { get; set; }

        public DbSet<Country> Countries { get; set; }

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

            // ApplicationImage nullable SetNull FKs (must stay here; nullable FKs + SetNull not easily expressed in a generic config)
            modelBuilder.Entity<ApplicationImage>().HasQueryFilter(x => !x.IsDelete);
            modelBuilder.Entity<ApplicationImage>()
                .HasOne<Property>()
                .WithMany()
                .HasForeignKey(ai => ai.PropertyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ApplicationImage>()
                .HasOne<Outlet>()
                .WithMany()
                .HasForeignKey(ai => ai.OutletId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Country>()
               .HasQueryFilter(x => !x.IsDelete);

            // ── Seed Data ─────────────────────────────────────────────────────
            LanguageSeed.Seed(modelBuilder.Entity<Language>());
            CountrySeed.Seed(modelBuilder.Entity<Country>());
            PropertySeed.Seed(modelBuilder.Entity<Property>());
            OutletSeed.Seed(modelBuilder.Entity<Outlet>());
            PropertyOutletSeed.Seed(modelBuilder.Entity<PropertyOutlet>());
            ApplicationSettingsSeed.Seed(modelBuilder.Entity<ApplicationSettings>());
            ApplicationImageSeed.Seed(modelBuilder.Entity<ApplicationImage>());
            RoleSeed.Seed(modelBuilder.Entity<Role>());
            PermissionSeed.Seed(modelBuilder.Entity<Permission>());
            RolePermissionSeed.Seed(modelBuilder.Entity<RolePermission>());
            EmployeeRoleSeed.Seed(modelBuilder.Entity<EmployeeRole>());
            EmployeeSeed.Seed(modelBuilder.Entity<Employee>());
            FormTemplateSeed.Seed(modelBuilder);
            FormTemplateTranslationsSeed.Seed(modelBuilder);
            DocumentTemplateSeed.Seed(modelBuilder.Entity<DocumentTemplate>());
            DocumentTemplateTranslationSeed.Seed(modelBuilder);
            WorkflowSeed.Seed(modelBuilder);
        }
    }
}
