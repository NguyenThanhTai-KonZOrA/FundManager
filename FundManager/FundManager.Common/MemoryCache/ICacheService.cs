using Microsoft.Extensions.Caching.Memory;

namespace FundManager.Common.MemoryCache
{
    public interface ICacheService
    {
        bool TryGetValue<T>(string key, out T value);
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null);
        void Remove(string key);
        Task<T?> GetOrCreateCache<T>(string key, Func<ICacheEntry, Task<T>> factory);
    }
}