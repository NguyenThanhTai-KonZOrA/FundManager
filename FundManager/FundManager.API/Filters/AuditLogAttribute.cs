using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DigitalDocumentPlatform.API.Filters
{
    /// <summary>
    /// Attribute to automatically log audit entries for controller actions.
    /// Usage: [AuditLog("EntityType", "Action")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuditLogAttribute : TypeFilterAttribute
    {
        public AuditLogAttribute(string entityType, string action) : base(typeof(AuditLogActionFilter))
        {
            Arguments = new object[] { entityType, action };
        }
    }

    public class AuditLogActionFilter : IAsyncActionFilter
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLogActionFilter> _logger;
        private readonly string _entityType;
        private readonly string _action;

        private static readonly string[] CommonIdParams =
            ["id", "taskId", "projectId", "commentId", "attachmentId",
             "relatedTaskId", "templateId", "tagId", "employeeId", "performanceId"];

        private static readonly JsonSerializerOptions DetailJsonOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            MaxDepth = 5
        };

        private static readonly string[] SensitiveFields =
            ["password", "token", "secret", "authorization", "creditcard", "ssn"];

        public AuditLogActionFilter(
            IAuditLogService auditLogService,
            ILogger<AuditLogActionFilter> logger,
            string entityType,
            string action)
        {
            _auditLogService = auditLogService;
            _logger = logger;
            _entityType = entityType;
            _action = action;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            var httpContext = context.HttpContext;
            var userName = httpContext.User.Identity?.Name ?? "Anonymous";
            int? entityId = ExtractEntityIdFromRoute(context);

            // Capture request details before execution
            var requestDetails = CaptureRequestDetails(context);

            var executedContext = await next();
            stopwatch.Stop();

            int statusCode;
            bool isSuccess;
            string? errorMessage = null;
            Dictionary<string, object?>? responseDetails = null;

            if (executedContext.Exception != null && !executedContext.ExceptionHandled)
            {
                statusCode = 500;
                isSuccess = false;
                errorMessage = executedContext.Exception.Message;
            }
            else
            {
                statusCode = GetStatusCode(executedContext);
                isSuccess = statusCode >= 200 && statusCode < 300;

                // Try to extract EntityId from response for create operations
                if (executedContext.Result is ObjectResult objResult)
                {
                    if (!entityId.HasValue && isSuccess)
                    {
                        entityId = ExtractEntityIdFromResult(objResult);
                    }
                    responseDetails = CaptureResponseDetails(objResult);
                }
            }

            // Build comprehensive details JSON
            var details = BuildDetailsJson(requestDetails, responseDetails, entityId);

            try
            {
                await _auditLogService.LogActionAsync(new CreateAuditLogRequest
                {
                    UserName = userName,
                    Action = _action,
                    EntityType = _entityType,
                    EntityId = entityId,
                    HttpMethod = httpContext.Request.Method,
                    RequestPath = httpContext.Request.Path,
                    IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
                    IsSuccess = isSuccess,
                    StatusCode = statusCode,
                    ErrorMessage = errorMessage,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    Details = details
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuditLogFilter] Failed to log audit for action {Action}", _action);
            }
        }

        /// <summary>
        /// Capture request arguments: route params, query strings, and request body properties.
        /// Sensitive fields are masked automatically.
        /// </summary>
        private Dictionary<string, object?> CaptureRequestDetails(ActionExecutingContext context)
        {
            var details = new Dictionary<string, object?>();

            // Route parameters
            var routeParams = new Dictionary<string, string>();
            foreach (var rv in context.RouteData.Values)
            {
                if (rv.Key is "controller" or "action") continue;
                routeParams[rv.Key] = rv.Value?.ToString() ?? "";
            }
            if (routeParams.Count > 0)
                details["routeParams"] = routeParams;

            // Query string parameters
            var queryParams = new Dictionary<string, string>();
            foreach (var q in context.HttpContext.Request.Query)
            {
                queryParams[q.Key] = IsSensitive(q.Key) ? "***" : q.Value.ToString();
            }
            if (queryParams.Count > 0)
                details["queryParams"] = queryParams;

            // Action arguments (request body)
            foreach (var arg in context.ActionArguments)
            {
                if (arg.Value == null) continue;

                var argType = arg.Value.GetType();

                // Skip primitives that are already captured via route/query
                if (argType.IsPrimitive || argType == typeof(string) || argType == typeof(decimal))
                    continue;

                // Skip file uploads - just log metadata
                if (arg.Value is IFormFile file)
                {
                    details["file"] = new { file.FileName, file.Length, file.ContentType };
                    continue;
                }
                if (arg.Value is IFormFileCollection files)
                {
                    details["files"] = files.Select(f => new { f.FileName, f.Length, f.ContentType }).ToList();
                    continue;
                }

                // Extract public properties from the request object
                var bodyProps = new Dictionary<string, object?>();
                foreach (var prop in argType.GetProperties())
                {
                    try
                    {
                        var val = prop.GetValue(arg.Value);
                        if (val == null) continue;

                        if (IsSensitive(prop.Name))
                        {
                            bodyProps[prop.Name] = "***";
                        }
                        else if (val is IFormFile bodyFile)
                        {
                            bodyProps[prop.Name] = $"{bodyFile.FileName} ({bodyFile.Length} bytes)";
                        }
                        else if (val is IFormFileCollection bodyFiles)
                        {
                            bodyProps[prop.Name] = $"{bodyFiles.Count} file(s)";
                        }
                        else if (val is string strVal)
                        {
                            bodyProps[prop.Name] = strVal.Length > 200 ? strVal[..200] + "..." : strVal;
                        }
                        else if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(decimal)
                            || prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?)
                            || prop.PropertyType.IsEnum
                            || Nullable.GetUnderlyingType(prop.PropertyType)?.IsEnum == true
                            || Nullable.GetUnderlyingType(prop.PropertyType)?.IsPrimitive == true)
                        {
                            bodyProps[prop.Name] = val;
                        }
                        else
                        {
                            bodyProps[prop.Name] = val.ToString();
                        }
                    }
                    catch
                    {
                        // Skip properties that can't be read
                    }
                }

                if (bodyProps.Count > 0)
                    details["requestBody"] = bodyProps;
            }

            return details;
        }

        /// <summary>
        /// Capture key fields from the response (Id, Name, Code, Status, etc.)
        /// </summary>
        private static Dictionary<string, object?>? CaptureResponseDetails(ObjectResult result)
        {
            if (result.Value == null) return null;

            var details = new Dictionary<string, object?>();
            var type = result.Value.GetType();

            // For bool responses, just log the value
            if (result.Value is bool boolVal)
            {
                details["result"] = boolVal;
                return details;
            }

            // Extract common identifying fields from the response
            string[] summaryProps = ["Id", "id", "TaskId", "ProjectId", "ProjectName",
                "TaskTitle", "TaskCode", "ProjectCode", "Name", "FullName",
                "StatusName", "StatusId", "Action", "EntityType"];

            foreach (var propName in summaryProps)
            {
                var prop = type.GetProperty(propName);
                if (prop != null)
                {
                    var val = prop.GetValue(result.Value);
                    if (val != null)
                        details[propName] = val;
                }
            }

            return details.Count > 0 ? details : null;
        }

        /// <summary>
        /// Build a comprehensive JSON detail string combining request/response info.
        /// </summary>
        private string BuildDetailsJson(
            Dictionary<string, object?> requestDetails,
            Dictionary<string, object?>? responseDetails,
            int? entityId)
        {
            var detail = new Dictionary<string, object?>
            {
                ["entityType"] = _entityType,
                ["action"] = _action
            };

            if (entityId.HasValue)
                detail["entityId"] = entityId.Value;

            if (requestDetails.Count > 0)
                detail["request"] = requestDetails;

            if (responseDetails != null && responseDetails.Count > 0)
                detail["response"] = responseDetails;

            try
            {
                return JsonSerializer.Serialize(detail, DetailJsonOptions);
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new { entityType = _entityType, action = _action, note = "Failed to serialize full details" });
            }
        }

        private static bool IsSensitive(string fieldName)
        {
            var lower = fieldName.ToLowerInvariant();
            return SensitiveFields.Any(s => lower.Contains(s));
        }

        private static int? ExtractEntityIdFromRoute(ActionExecutingContext context)
        {
            foreach (var param in CommonIdParams)
            {
                if (context.RouteData.Values.TryGetValue(param, out var value) &&
                    int.TryParse(value?.ToString(), out var id))
                {
                    return id;
                }
            }

            return null;
        }

        private static int GetStatusCode(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
                return objectResult.StatusCode ?? 200;
            if (context.Result is StatusCodeResult statusCodeResult)
                return statusCodeResult.StatusCode;
            return 200;
        }

        private static int? ExtractEntityIdFromResult(ObjectResult result)
        {
            if (result.Value == null) return null;

            var type = result.Value.GetType();
            var idProp = type.GetProperty("Id") ?? type.GetProperty("id");
            if (idProp != null)
            {
                var value = idProp.GetValue(result.Value);
                if (value is int intValue) return intValue;
                if (int.TryParse(value?.ToString(), out var parsed)) return parsed;
            }

            return null;
        }
    }
}