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
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IEmployeeRoleRepository, EmployeeRoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IApplicationSettingsRepository, ApplicationSettingsRepository>();
            services.AddScoped<IApplicationImageRepository, ApplicationImageRepository>();

            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IAuditLogService, AuditLogService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IEmployeeRoleService, EmployeeRoleService>();
            services.AddScoped<IPdfConverterService, LibreOfficePdfConverterService>();
            services.AddScoped<IApplicationSettingsService, ApplicationSettingsService>();
            services.AddScoped<IApplicationImageService, ApplicationImageService>();

            logger.Info("Application services registered successfully");
            return services;
        }
    }
}