using FundManager.Implement.SignalRHubs;
using NLog;

namespace FundManager.API.Configurations
{
    public static class SignalRConfiguration
    {
        public static WebApplication MapSignalRHubs(this WebApplication application, Logger logger)
        {
            logger.Info("Mapping PatronSignatureHub hubs with CORS support and transport options");
            application.MapHub<PatronSignatureHub>("/patronSignatureHub", options =>
            {
                options.Transports =
                   Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
                   Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents |
                   Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                options.LongPolling.PollTimeout = TimeSpan.FromSeconds(90);
                options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(5);
            }).RequireCors("AllowAll"); ;

            return application;
        }
    }
}