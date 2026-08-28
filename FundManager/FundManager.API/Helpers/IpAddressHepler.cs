using System.Collections.Concurrent;
using System.Net;

namespace FundManager.API.Helpers
{
    public static class IpAddressHepler
    {
        private static readonly ConcurrentDictionary<string, CacheItem> _cache = new();
        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(15);         // positive cache
        private static readonly TimeSpan NegativeTtl = TimeSpan.FromMinutes(1);         // negative cache
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(300);

        private sealed record CacheItem(string? Name, DateTimeOffset ExpireAt);

        // Get client IP (supports proxy via X-Forwarded-For)
        public static string? GetClientIp(HttpContext httpContext, bool tryUseXForwardedFor = true)
        {
            if (tryUseXForwardedFor)
            {
                var forwarded = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(forwarded))
                {
                    var ip = forwarded.Split(',').FirstOrDefault()?.Trim();
                    if (!string.IsNullOrWhiteSpace(ip)) return ip;
                }
            }
            return httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }

        // Back-compat (no timeout)
        public static Task<string?> GetClientComputerNameAsync(HttpContext httpContext) =>
            GetClientComputerNameAsync(httpContext, timeout: null, CancellationToken.None);

        // With timeout + cancellation
        public static async Task<string?> GetClientComputerNameAsync(HttpContext httpContext, TimeSpan? timeout, CancellationToken ct)
        {
            // Optional fast path: client header as a fallback (trusted networks only)
            var clientNameHeader = httpContext.Request.Headers["X-Client-Name"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(clientNameHeader))
                return clientNameHeader;

            var ip = GetClientIp(httpContext);
            return await DetermineCompNameAsync(ip, timeout, ct);
        }

        public static async Task<string?> DetermineCompNameAsync(string? ip, TimeSpan? timeout, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(ip)) return null;
            if (!IPAddress.TryParse(ip, out var parsed)) return null;

            // Avoid slow public reverse DNS
            if (!IsPrivate(parsed)) return null;

            // Cache lookup
            if (_cache.TryGetValue(ip, out var cached) && cached.ExpireAt > DateTimeOffset.UtcNow)
                return cached.Name;

            try
            {
                var effTimeout = timeout ?? DefaultTimeout;
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                timeoutCts.CancelAfter(effTimeout);

                // DNS reverse lookup with timeout (WaitAsync is .NET 6+)
                var entry = await Dns.GetHostEntryAsync(parsed).WaitAsync(effTimeout, timeoutCts.Token);
                var host = entry.HostName ?? string.Empty;
                var first = host.Split('.').FirstOrDefault();
                var name = string.IsNullOrWhiteSpace(first) ? host : first;

                _cache[ip] = new CacheItem(name, DateTimeOffset.UtcNow.Add(DefaultTtl));
                return name;
            }
            catch
            {
                // Negative-cache failures briefly to avoid repeated slow lookups
                _cache[ip] = new CacheItem(null, DateTimeOffset.UtcNow.Add(NegativeTtl));
                return null;
            }
        }

        private static bool IsPrivate(IPAddress ip)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                var b = ip.GetAddressBytes();
                // 10.0.0.0/8
                if (b[0] == 10) return true;
                // 172.16.0.0/12
                if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;
                // 192.168.0.0/16
                if (b[0] == 192 && b[1] == 168) return true;
            }
            // Treat loopback as private
            if (IPAddress.IsLoopback(ip)) return true;
            return false;
        }
    }
}