using FundManager.API.Configurations;
using FundManager.Common.ApiClient;
using FundManager.Common.Constants;
using FundManager.DataAccess.ApplicationDbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NLog;
using NLog.Web;
using System.Security.Claims;
using System.Text;

// NLog initialization
var logger = LogManager.Setup()
    .LoadConfigurationFromFile("NLog.config")
    .GetCurrentClassLogger();

logger.Info("============> Digital Document Platform API initializing... <============");

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region Configure NLog
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    #endregion

    #region JWT Authentication
    var jwtSection = builder.Configuration.GetSection("Jwt");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
            // SignalR: JWT qua query string cho WebSocket connection
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var path = context.HttpContext.Request.Path;
                    if (!path.StartsWithSegments("/patronSignatureHub")) return Task.CompletedTask;

                    // WebSocket & LongPolling: token qua query string
                    var tokenFromQuery = context.Request.Query["access_token"].ToString();
                    if (!string.IsNullOrEmpty(tokenFromQuery))
                    {
                        context.Token = tokenFromQuery;
                        return Task.CompletedTask;
                    }

                    // Negotiate (HTTP POST): token qua Authorization header
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        context.Token = authHeader["Bearer ".Length..].Trim();

                    return Task.CompletedTask;
                }
            };
        });
    #endregion

    #region Register Database
    builder.Services.AddDbContext<FundManagerDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString(CommonConstants.DefaultConnection))
        .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));
    #endregion

    #region Cache & HTTP Client
    builder.Services.AddMemoryCache();
    builder.Services.AddHttpClient<IApiClient, ApiClient>();
    #endregion

    // Application Services & Repositories
    builder.Services.AddApplicationServices(logger);

    // Add services to the container.
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddControllers();

    // CORS — SignalR need to AllowCredentials so cannot use AllowAnyOrigin
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    #region Register SignalR and Swagger
    builder.Services.AddSignalR()
        .AddJsonProtocol(options =>
        {
            // Ensure all properties sent via SignalR are camelCase
            // (including anonymous objects like { ConversationId, EmployeeId })
            options.PayloadSerializerOptions.PropertyNamingPolicy =
                System.Text.Json.JsonNamingPolicy.CamelCase;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        options.SwaggerDoc("v1", new() { Title = "Digital Document Platform API", Version = "v1" });
    });
    #endregion

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
    }

    // Static Files with CORS support.
    logger.Info("============> Static Files with CORS support initializing... <============");
    StaticFileRegistration.StaticFileRegister(app, logger);
    logger.Info("============> Static Files with CORS support end! <============");

    app.UseCors("AllowAll");
    // Swagger & Hangfire Dashboard
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMiddleware<ApiMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    logger.Info("============> Map SignalR Hubs initializing... <============");
    //SignalRConfiguration.MapSignalRHubs(app, logger);
    logger.Info("============> Map SignalR Hubs end! <============");

    // Database Migration
    logger.Info("============> Check and run migration initializing... <============");
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<FundManagerDbContext>();
        await context.Database.MigrateAsync();
    }
    logger.Info("============> Check and run migration end! <============");

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "API startup terminated unexpectedly.");
    throw;
}
finally
{
    LogManager.Shutdown();
}