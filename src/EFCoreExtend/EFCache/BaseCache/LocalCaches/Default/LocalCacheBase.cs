using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.BaseCache.LocalCaches.Default
{
    public abstract class LocalCacheBase : ILocalCache, IDisposable
    {
        public LocalCacheBase()
        {
            LocalCacheClearManager.StartClear();
        }

        public abstract IReadOnlyDictionary<string, ILocalCacheInfo> Caches { get; }

        public abstract void SetCache(string cacheKey, object val, DateTime? expiry = null);
        public abstract bool TryGetCache(string cacheKey, out object val, DateTime? expiry = null);

        public abstract void Remove(string key);

        public abstract void RemoveRange(IEnumerable<string> keys);

        public abstract void Clear();

        public abstract void Dispose();
    }
}
