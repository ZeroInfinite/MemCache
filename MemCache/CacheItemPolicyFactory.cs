using System;
using System.Runtime.Caching;

namespace MemCache
{
    public static class CacheItemPolicyFactory
    {
        public static CacheItemPolicy Factory(bool? persistCache = null, CacheItemPriority priority = CacheItemPriority.Default, int durationInMins = 60, CacheEntryRemovedCallback removedCallback = null, CacheEntryUpdateCallback updateCallback = null)
        {
            var policy = new CacheItemPolicy
            {
                Priority = priority,
                RemovedCallback = removedCallback,
                UpdateCallback = updateCallback
            };

            if (!persistCache.HasValue)
            {
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddDays(1);
                policy.SlidingExpiration = TimeSpan.FromMinutes(durationInMins);
            }
            else
            {
                if (persistCache.Value)
                    policy.SlidingExpiration = TimeSpan.FromMinutes(durationInMins);
                else
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(durationInMins);
            }

            return policy;
        }
    }
}
