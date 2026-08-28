using Microsoft.Extensions.FileProviders;
using NLog;

namespace FundManager.API.Configurations
{
    public static class StaticFileRegistration
    {
        public static WebApplication StaticFileRegister(this WebApplication app, Logger logger)
        {
            var contentRoot = app.Services.GetRequiredService<IWebHostEnvironment>().ContentRootPath;
            // Static Files with CORS support
            var staticFileOptions = new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Add CORS headers for static files
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization");

                    // Optional: Set cache control
                    ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=3600");
                }
            };

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(contentRoot, "ApplicationImages")),
                RequestPath = "/ApplicationImages",
                OnPrepareResponse = staticFileOptions.OnPrepareResponse
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(contentRoot, "Documents")),
                RequestPath = "/Documents",
                OnPrepareResponse = staticFileOptions.OnPrepareResponse
            });

            logger.Info("Static file providers for /ApplicationImages and /Documents registered successfully with CORS support");

            return app;
        }
    }
}