using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCacheExample.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SetData(string key, object value)
        {
            _cache.Set(key, value);
        }

        public List<T>? GetData<T>(string key)
        {
            if (_cache.TryGetValue(key, out List<T>? data))
            {
                return data;
            }

            return null;
        }

        public object? GetData(string key)
        {
            if (_cache.TryGetValue(key, out object? data))
            {
                return data;
            }
            return null;
        }
    }
}
