using FundManager.Implement.ViewModels;
using System.Security.Claims;

namespace FundManager.API.Helpers
{
    public static class EmployeeHelper
    {
        /// <summary>
        /// Get EmployeeId from current authenticated user's JWT token
        /// </summary>
        public static int? GetCurrentEmployeeId(HttpContext httpContext)
        {
            // Add debug logging
            var allClaims = httpContext.User?.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
            Console.WriteLine($"🔍 All claims: {string.Join(", ", allClaims ?? new List<string>())}");

            var employeeIdClaim = httpContext.User?.FindFirstValue("EmployeeId");
            Console.WriteLine($"🔍 EmployeeId claim value: {employeeIdClaim ?? "NULL"}");

            if (string.IsNullOrEmpty(employeeIdClaim))
                return null;

            return int.TryParse(employeeIdClaim, out var employeeId) ? employeeId : null;
        }

        /// <summary>
        /// Get EmployeeCode from current authenticated user's JWT token
        /// </summary>
        public static string? GetCurrentEmployeeCode(HttpContext httpContext)
        {
            return httpContext.User?.FindFirstValue("EmployeeCode");
        }

        /// <summary>
        /// Get username from current authenticated user
        /// </summary>
        public static string? GetCurrentUsername(HttpContext httpContext)
        {
            return httpContext.User?.Identity?.Name;
        }

        /// <summary>
        /// Get current employee information from JWT claims including Id, Code, and Name
        /// </summary>
        /// <param name="httpContext">Current HTTP context</param>
        /// <returns>CurrentEmployeeInfo object containing Id, Code, and Name. Returns null if user is not authenticated.</returns>
        public static CurrentEmployeeInfo CurrentEmployee(HttpContext httpContext)
        {
            var result = new CurrentEmployeeInfo
            {
                Id = Common.Constants.CommonConstants.UnknowUserId,
                Code = Common.Constants.CommonConstants.UnknowUser,
                Name = Common.Constants.CommonConstants.UnknowUser,
                WindowAccount = Common.Constants.CommonConstants.UnknowUser,
                IsQualityControl = false
            };

            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return result;

            var employeeIdClaim = httpContext.User?.FindFirstValue("EmployeeId");
            var employeeCode = httpContext.User?.FindFirstValue("EmployeeCode");
            var employeeName = httpContext.User?.FindFirstValue("EmployeeName")
                             ?? httpContext.User?.FindFirstValue(ClaimTypes.Name)
                             ?? httpContext.User?.Identity?.Name;
            var windowAccount = httpContext.User?.FindFirstValue("WindowAccount");
            bool isQualityControl = bool.TryParse(httpContext.User?.FindFirstValue("IsQualityControl"), out var qc) && qc;

            if (string.IsNullOrEmpty(employeeIdClaim))
                return result;

            if (!int.TryParse(employeeIdClaim, out var employeeId))
                return result;

            return new CurrentEmployeeInfo
            {
                Id = employeeId,
                Code = employeeCode ?? Common.Constants.CommonConstants.UnknowUser,
                Name = employeeName ?? Common.Constants.CommonConstants.UnknowUser,
                WindowAccount = windowAccount ?? Common.Constants.CommonConstants.UnknowUser,
                IsQualityControl = isQualityControl
            };
        }
    }
}