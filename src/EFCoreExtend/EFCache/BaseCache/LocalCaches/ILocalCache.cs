using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.BaseCache.LocalCaches
{
    public interface ILocalCache : IDisposable
    {
        IReadOnlyDictionary<string, ILocalCacheInfo> Caches { get; }

        void SetCache(string cacheKey, object val, DateTime? expiry = null);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        /// <param name="expiry">更新缓存时间，如果获取缓存成功后需要更新缓存时间那么设置这个值</param>
        /// <returns></returns>
        bool TryGetCache(string cacheKey, out object val, DateTime? expiry = null);

        void Remove(string key);

        void RemoveRange(IEnumerable<string> keys);

        void Clear();

    }
}
