using FundManager.API.WindowHelpers;
using FundManager.Common.Constants;
using FundManager.Common.JwtAuthen;
using FundManager.Common.SystemConfiguration;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Services.Interface;
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FundManager.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Constructor
        private readonly ISystemConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IAuditLogService _auditLogService;
        private readonly IRoleService _roleService;
        public AuthController(
            ISystemConfiguration configuration,
            ILogger<AuthController> logger,
            IEmployeeService employeeService,
            IRefreshTokenService refreshTokenService,
            IAuditLogService auditLogService,
            IRoleService roleService)
        {
            _configuration = configuration;
            _logger = logger;
            _employeeService = employeeService;
            _refreshTokenService = refreshTokenService;
            _auditLogService = auditLogService;
            _roleService = roleService;
        }
        #endregion

        #region Main APIs

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation("Login attempt for user {Username}", loginRequest.Username);

                int result = WindowsAuthHelper.WindowsAccount(loginRequest.Username, loginRequest.Password);

                string adminUserName = _configuration.GetValue("AdminAccount:UserName") ?? "admin";
                string adminPassword = _configuration.GetValue("AdminAccount:Password") ?? "Republic@123456";
                if (loginRequest.Username == adminUserName && loginRequest.Password == adminPassword)
                {
                    result = 1;
                }

                if (result != 1)
                {
                    _logger.LogError("Invalid credentials.");
                    throw new Exception("The user name or password is incorrect.");
                }

                var employee = await _employeeService.GetOrCreateEmployeeFromWindowsAccountAsync(loginRequest.Username);
                _logger.LogInformation("Employee authenticated: {EmployeeCode} (ID: {EmployeeId})", employee.EmployeeCode, employee.Id);

                // Resolve role
                var adminsConfig = "admin;superuser;";
                var adminUsers = adminsConfig.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var role = adminUsers.Contains(loginRequest.Username, StringComparer.OrdinalIgnoreCase)
                    ? CommonConstants.AdminRole
                    : CommonConstants.UserRole;

                // Resolve role
                var roleIds = employee.EmployeeRoles?.Select(er => er.RoleId).ToList() ?? new List<int>();
                var getRolesResult = await _roleService.GetActiveRolesByIdsAsync(roleIds);
                string[] rolesName = Array.Empty<string>();
                if (getRolesResult.Count == 0)
                {
                    rolesName = new string[] { CommonConstants.UserRole };
                }
                else
                {
                    rolesName = getRolesResult.Select(r => r.RoleName).ToArray();
                }

                var tokenResponse = GenerateTokens(loginRequest.Username, rolesName, employee);

                await _auditLogService.LogActionAsync(new CreateAuditLogRequest
                {
                    UserName = loginRequest.Username,
                    Action = "Login",
                    EntityType = "Authentication",
                    EntityId = employee.Id,
                    HttpMethod = "POST",
                    RequestPath = "/api/auth/login",
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IsSuccess = true,
                    StatusCode = 200,
                    ErrorMessage = null,
                    Details = $"User {loginRequest.Username} logged in successfully.",
                    DurationMs = 0 // You can measure actual duration if needed
                });

                _logger.LogInformation("Token generated for {Username} with server_start: {ServerStart}",
                    loginRequest.Username,
                    TokenValidationService.ServerStartTime);

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                await _auditLogService.LogActionAsync(new CreateAuditLogRequest
                {
                    UserName = loginRequest.Username,
                    Action = "Login",
                    EntityType = "Authentication",
                    EntityId = null,
                    HttpMethod = "POST",
                    RequestPath = "/api/auth/login",
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IsSuccess = false,
                    StatusCode = 401,
                    ErrorMessage = ex.Message,
                    Details = $"Failed login attempt for user {loginRequest.Username}.",
                    DurationMs = 0 // You can measure actual duration if needed
                });

                _logger.LogError(ex, "Login error for user {Username}", loginRequest.Username);
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var username = User.Identity?.Name;

                // If username is null, try to extract it from the JWT token in the Authorization header
                if (string.IsNullOrEmpty(username))
                {
                    var authHeader = Request.Headers["Authorization"].ToString();
                    if (authHeader.StartsWith("Bearer "))
                    {
                        var token = authHeader.Substring("Bearer ".Length).Trim();
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(token);
                        username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    }
                }

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (!_refreshTokenService.ValidateRefreshToken(username, request.RefreshToken))
                {
                    _logger.LogWarning("Invalid refresh token for user {Username}", username);
                    return Unauthorized(new { message = "Invalid refresh token" });
                }

                var employee = await _employeeService.GetOrCreateEmployeeFromWindowsAccountAsync(username);

                // Resolve role
                var roleIds = employee.EmployeeRoles?.Select(er => er.RoleId).ToList() ?? new List<int>();
                var getRolesResult = await _roleService.GetActiveRolesByIdsAsync(roleIds);
                string[] rolesName = Array.Empty<string>();
                if (getRolesResult.Count == 0)
                {
                    rolesName = new string[] { CommonConstants.UserRole };
                }
                else
                {
                    rolesName = getRolesResult.Select(r => r.RoleName).ToArray();
                }

                var tokenResponse = GenerateTokens(username, rolesName, employee);

                _logger.LogInformation("Token refreshed for {Username}", username);

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh token error");
                return Unauthorized(new { message = "Token refresh failed" });
            }
        }

        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized();
                }

                _refreshTokenService.RevokeRefreshToken(username);
                _logger.LogInformation("Token revoked for user {Username}", username);

                // Log the revoke action
                await _auditLogService.LogActionAsync(new CreateAuditLogRequest
                {
                    UserName = username,
                    Action = "RevokeToken",
                    EntityType = "Authentication",
                    EntityId = null,
                    HttpMethod = "POST",
                    RequestPath = "/api/auth/revoke-token",
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IsSuccess = true,
                    StatusCode = 200,
                    ErrorMessage = null,
                    Details = $"User {username} revoked their token successfully.",
                    DurationMs = 0 // You can measure actual duration if needed
                });

                return Ok(new { message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                // Log the error
                await _auditLogService.LogActionAsync(new CreateAuditLogRequest
                {
                    UserName = User.Identity?.Name ?? "Unknown",
                    Action = "RevokeToken",
                    EntityType = "Authentication",
                    EntityId = null,
                    HttpMethod = "POST",
                    RequestPath = "/api/auth/revoke-token",
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IsSuccess = false,
                    StatusCode = 500,
                    ErrorMessage = ex.Message,
                    Details = $"Failed to revoke token: {ex.Message}",
                    DurationMs = 0 // You can measure actual duration if needed
                });

                _logger.LogError(ex, "Revoke token error");
                throw new BadHttpRequestException($"Failed to revoke token: {ex.Message}");
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var employeeId = User.FindFirstValue("EmployeeId");
            var employeeCode = User.FindFirstValue("EmployeeCode");
            var result = new
            {
                userName = User.Identity?.Name,
                role = User.FindFirstValue(ClaimTypes.Role),
                employeeId = employeeId != null ? int.Parse(employeeId) : (int?)null,
                employeeCode = employeeCode
            };

            return Ok(result);
        }

        [HttpGet("server-info")]
        [AllowAnonymous]
        public IActionResult GetServerInfo()
        {
            var result = new
            {
                serverStartTime = TokenValidationService.ServerStartTime,
                currentTime = DateTime.Now.ToString("o")
            };

            return Ok(result);
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            _logger.LogInformation("Ping endpoint was called.");
            return Ok("Pong");
        }
        #endregion

        #region Private Methods
        private LoginResponse GenerateTokens(string username, string[] roles, Employee employee)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, string.Join(",", roles)),
                new Claim("EmployeeId", employee.Id.ToString()),
                new Claim("EmployeeCode", employee.EmployeeCode),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim("server_start", TokenValidationService.ServerStartTime)
            };

            var jwt = _configuration.GetSection<JwtOptions>("Jwt") ?? new JwtOptions();
            if (string.IsNullOrWhiteSpace(jwt.Key))
            {
                _logger.LogError("JWT key is missing in configuration.");
                throw new Exception("JWT key missing.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(jwt.Key);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(jwt.ExpireMinutes > 0 ? jwt.ExpireMinutes : 30);

            var token = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Generate refresh token (expires in 7 days)
            var refreshToken = _refreshTokenService.GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.Now.AddDays(7);
            _refreshTokenService.StoreRefreshToken(username, refreshToken, refreshTokenExpiration);

            return new LoginResponse
            {
                UserName = username,
                Token = tokenString,
                RefreshToken = refreshToken,
                Role = roles,
                EmployeeId = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                TokenExpiration = expires
            };
        }
        #endregion
    }
}