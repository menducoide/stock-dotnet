using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using stock_dotnet.utils.env;
namespace stock_dotnet.utils.cache
{
    public class CacheHandler:ICacheHandler
    {
        private IMemoryCache _cache;

        public CacheHandler(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Invalidate(object key)
        {
            if (_cache.TryGetValue(key, out object value))
                _cache.Remove(key);
        }

        public void Add(object key, object value, int duration = 0)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions() // Keep in cache for this time, reset time if accessed.
                     .SetSlidingExpiration(TimeSpan.FromMinutes(duration == 0 ? Const.CACHE_DEFAULT_EXPIRATION_MINUTES : duration)));
        }
        public T GetT<T>(object key, out T value)
        {
            if (!_cache.TryGetValue(key, out value))
            {
                return default(T);
            }
            return value;
        }
    }
}