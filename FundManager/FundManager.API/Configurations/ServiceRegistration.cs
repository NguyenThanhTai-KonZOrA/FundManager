using BreakFastCheckIn.Implement.Services;
using BreakFastCheckIn.Implement.Services.Interface;
using FundManager.API.WindowHelpers;
using FundManager.BackgroundQueue;
using FundManager.Common.ApiClient;
using FundManager.Common.MemoryCache;
using FundManager.Common.SystemConfiguration;
using FundManager.Implement.BackgroundQueue;
using FundManager.Implement.Repositories;
using FundManager.Implement.Repositories.Interface;
using FundManager.Implement.Services;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.UnitOfWork;
using FundManager.Implement.Workers;
using NLog;

namespace FundManager.API.Configurations
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, Logger logger)
        {
            logger.Info("🟢  Add services...");
            services.AddSingleton<TokenValidationService>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<ISystemConfiguration, SystemConfiguration>();
            services.AddHttpClient<IApiClient, ApiClient>();
            services.AddHostedService<NotificationRetryWorker>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IPatronRepository, PatronRepository>();
            services.AddTransient<IPatronDeviceRepository, PatronDeviceRepository>();
            services.AddTransient<ISignatureSessionRepository, SignatureSessionRepository>();
            services.AddTransient<IStaffDeviceRepository, StaffDeviceRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDeviceMappingRepository, DeviceMappingRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IEmployeeRoleRepository, EmployeeRoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IApplicationSettingsRepository, ApplicationSettingsRepository>();
            services.AddScoped<IApplicationImageRepository, ApplicationImageRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IOutletRepository, OutletRepository>();
            services.AddScoped<IPropertyOutletRepository, PropertyOutletRepository>();
            services.AddScoped<IReservationSnapshotRepository, ReservationSnapshotRepository>();
            // Form template, submission, workflow
            services.AddScoped<IFormTemplateRepository, FormTemplateRepository>();
            services.AddScoped<IFormQuestionRepository, FormQuestionRepository>();
            services.AddScoped<IFormSubmissionRepository, FormSubmissionRepository>();
            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddScoped<IDocumentTemplateRepository, DocumentTemplateRepository>();
            services.AddScoped<IPatronSignatureRepository, PatronSignatureRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IPatronTypeRepository, PatronTypeRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();

            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IPatronDeviceService, PatronDeviceService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IAuditLogService, AuditLogService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IEmployeeRoleService, EmployeeRoleService>();
            services.AddScoped<IPdfConverterService, LibreOfficePdfConverterService>();
            services.AddScoped<IApplicationSettingsService, ApplicationSettingsService>();
            services.AddScoped<IApplicationImageService, ApplicationImageService>();
            services.AddScoped<IPropertyOutletService, PropertyOutletService>();
            services.AddScoped<IOutletService, OutletService>();
            services.AddScoped<ISignalRService, SignalRService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<ICountryService, CountryService>();

            // Form template, submission, workflow services
            services.AddScoped<IFormTemplateService, FormTemplateService>();
            services.AddScoped<IFormSubmissionService, FormSubmissionService>();
            services.AddScoped<IWorkflowService, WorkflowService>();
            services.AddScoped<IDocumentTemplateService, DocumentTemplateService>();
            services.AddScoped<ICustomerSignService, CustomerSignService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IPatronTypeService, PatronTypeService>();

            logger.Info("Application services registered successfully");
            return services;
        }
    }
}