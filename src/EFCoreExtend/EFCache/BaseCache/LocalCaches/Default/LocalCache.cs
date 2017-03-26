using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.BaseCache.LocalCaches.Default
{
    public class LocalCache : LocalCacheBase
    {
        readonly string _dictKey;
        readonly ConcurrentDictionary<string, ILocalCacheInfo> _dict;

        public LocalCache()
        {
            _dict = new ConcurrentDictionary<string, ILocalCacheInfo>();
            _dictKey = Guid.NewGuid().ToString();
            LocalCacheClearManager.Add(_dictKey, _dict);//添加到缓存清理器中
        }

        public override IReadOnlyDictionary<string, ILocalCacheInfo> Caches => _dict as IReadOnlyDictionary<string, ILocalCacheInfo>;

        public override void SetCache(string cacheKey, object val, DateTime? expiry = null)
        {
            if (expiry.HasValue)
            {
                _dict[cacheKey] = new LocalCacheInfo(val, expiry.Value); 
            }
            else
            {
                _dict[cacheKey] = new LocalCacheInfo(val);
            }
        }

        public override bool TryGetCache(string cacheKey, out object val, DateTime? expiry = null)
        {
            ILocalCacheInfo cacheVal;
            if (_dict.TryGetValue(cacheKey, out cacheVal))
            {
                if (cacheVal.IsValid)
                {
                    val = cacheVal.Data;
                    //更新缓存时间
                    if (expiry.HasValue)
                    {
                        cacheVal.UpdateExpiry(expiry.Value);
                    }
                    return true;
                }
                else
                {
                    //过期缓存无效清理
                    _dict.TryRemove(cacheKey, out cacheVal);
                }
            }
            val = null;
            return false;
        }

        public override void Clear()
        {
            _dict.Clear();
        }

        public override void Remove(string key)
        {
            ILocalCacheInfo model;
            _dict.TryRemove(key, out model); 
        }

        public override void RemoveRange(IEnumerable<string> keys)
        {
            ILocalCacheInfo model;
            foreach (var key in keys)
            {
                _dict.TryRemove(key, out model);
            }
        }

        public override void Dispose()
        {
            Clear();
            //从缓存清理器中去除
            LocalCacheClearManager.Remove(_dictKey);
        }

    }
}
