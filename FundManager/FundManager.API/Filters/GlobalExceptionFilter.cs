//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace ProjectManagement.API.Filters
//{
//    /// <summary>
//    /// Global exception filter for consistent error responses
//    /// </summary>
//    public class GlobalExceptionFilter : IExceptionFilter
//    {
//        private readonly ILogger<GlobalExceptionFilter> _logger;

//        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
//        {
//            _logger = logger;
//        }

//        public void OnException(ExceptionContext context)
//        {
//            var exception = context.Exception;
//            var requestPath = context.HttpContext.Request.Path;
//            var method = context.HttpContext.Request.Method;

//            _logger.LogError(exception, $"Exception occurred: {method} {requestPath}");

//            ApiBaseResponse<object> response;
//            int statusCode;

//            switch (exception)
//            {
//                case KeyNotFoundException:
//                    statusCode = 404;
//                    response = new ApiBaseResponse<object>
//                    {
//                        Status = statusCode,
//                        Success = false,
//                        Message = exception.Message,
//                        Data = null
//                    };
//                    break;

//                case UnauthorizedAccessException:
//                    statusCode = 403;
//                    response = new ApiBaseResponse<object>
//                    {
//                        Status = statusCode,
//                        Success = false,
//                        Message = "You do not have permission to perform this action",
//                        Data = null
//                    };
//                    break;

//                case ArgumentException:
//                case InvalidOperationException:
//                    statusCode = 400;
//                    response = new ApiBaseResponse<object>
//                    {
//                        Status = statusCode,
//                        Success = false,
//                        Message = exception.Message,
//                        Data = null
//                    };
//                    break;

//                case BadHttpRequestException:
//                    statusCode = 400;
//                    response = new ApiBaseResponse<object>
//                    {
//                        Status = statusCode,
//                        Success = false,
//                        Message = exception.Message,
//                        Data = null
//                    };
//                    break;

//                default:
//                    statusCode = 500;
//                    response = new ApiBaseResponse<object>
//                    {
//                        Status = statusCode,
//                        Success = false,
//                        Message = "An unexpected error occurred. Please contact support.",
//                        Data = null
//                    };
//                    break;
//            }

//            context.Result = new ObjectResult(response)
//            {
//                StatusCode = statusCode
//            };

//            context.ExceptionHandled = true;
//        }
//    }
//}