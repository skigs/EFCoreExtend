using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.BaseCache.LocalCaches
{
    public static class LocalCacheExtensions
    {
        public static bool TryGetCache<T>(this ILocalCache cache, string cacheKey, out T val, DateTime? expiry = null)
        {
            object obj;
            if (cache.TryGetCache(cacheKey, out obj, expiry))
            {
                val = (T)obj;
                return true;
            }
            val = default(T);
            return false;
        }
    }
}
