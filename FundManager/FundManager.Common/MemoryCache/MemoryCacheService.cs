using Microsoft.Extensions.Caching.Memory;

namespace FundManager.Common.MemoryCache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T? value) ? value : default;
        }

        public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (absoluteExpiration.HasValue)
                options.SetAbsoluteExpiration(absoluteExpiration.Value);

            _cache.Set(key, value, options);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value!);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public async Task<T?> GetOrCreateCache<T>(string key, Func<ICacheEntry, Task<T>> factory)
        {
            if (_cache.TryGetValue(key, out T? value))
            {
                return value;
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            var cacheEntry = _cache.CreateEntry(key);
            cacheEntry.SetOptions(cacheEntryOptions);
            value = await factory(cacheEntry);
            cacheEntry.Value = value;
            return value;
        }
    }
}