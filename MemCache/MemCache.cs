using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Linq;
using System.Threading.Tasks;

namespace Caching
{
    public static class MemCache
    {
        private static ObjectCache myCache = MemoryCache.Default;

        public static void Clear()
        {
            List<string> cacheKeys = myCache.AsParallel().Select(kvp => kvp.Key).ToList();

            Parallel.ForEach(cacheKeys, c => myCache.Remove(c));
        }

        public static void Set(string key, object objectToCache, int cacheExpirationInMins, bool persistCache)
        {
            var cacheItemPolicy = new CacheItemPolicy();

            if (persistCache)
                cacheItemPolicy.SlidingExpiration = TimeSpan.FromMinutes(cacheExpirationInMins);
            else
                cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheExpirationInMins);

            SetToCache(key, objectToCache, cacheItemPolicy);
        }

        public static void Set(string key, object objectToCache, int cacheExpirationInMins)
        {
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheExpirationInMins) };

            SetToCache(key, objectToCache, cacheItemPolicy);
        }

        public static void Set(string key, object objectToCache, CacheItemPolicy cacheItemPolicy)
        {
            SetToCache(key, objectToCache, cacheItemPolicy);
        }

        public static object Get(string key)
        {
            return GetFromCache(key);
        }

        public static object GetOrAdd(string key, object value, int cacheExpirationInMins)
        {
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheExpirationInMins) };

            return GetFromCacheIfExistsOtherwiseAdd(key, value, cacheItemPolicy);
        }

        public static object GetOrAdd(string key, object value, int cacheExpirationInMins, bool persistCache)
        {
            var cacheItemPolicy = new CacheItemPolicy();

            if (persistCache)
                cacheItemPolicy.SlidingExpiration = TimeSpan.FromMinutes(cacheExpirationInMins);
            else
                cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheExpirationInMins);

            return GetFromCacheIfExistsOtherwiseAdd(key, value, cacheItemPolicy);
        }

        private static object GetFromCacheIfExistsOtherwiseAdd(string key, object value, CacheItemPolicy cacheItemPolicy, string regionName = null)
        {
            return myCache.AddOrGetExisting(key, value, cacheItemPolicy, regionName);
        }

        private static CacheItem GetCacheItem(string key)
        {
            return GetCacheItemFromCache(key);
        }

        private static IDictionary<string, object> GetValues(string regionName)
        {
            return GetValuesUsingRegionName(regionName);
        }

        private static object GetFromCache(string key)
        {
            return myCache.Get(key);
        }

        private static CacheItem GetCacheItemFromCache(string key)
        {
            return myCache.GetCacheItem(key);
        }

        private static IDictionary<string, object> GetValuesUsingRegionName(string regionName)
        {
            return myCache.GetValues(regionName);
        }

        private static void SetToCache(string key, object value, CacheItemPolicy cacheItemPolicy, string regionName = null)
        {
            if (myCache.Contains(key))
                myCache.Set(key, value, cacheItemPolicy, regionName);
            else
                myCache.Add(key, value, cacheItemPolicy, regionName);
        }
    }
}
