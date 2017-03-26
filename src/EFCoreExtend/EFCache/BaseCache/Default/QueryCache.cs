using System;
using System.Linq;
using EFCoreExtend.EFCache.BaseCache.LocalCaches.Default;
using EFCoreExtend.EFCache.BaseCache.LocalCaches;
using EFCoreExtend.Evaluators;

namespace EFCoreExtend.EFCache.BaseCache.Default
{
    public class QueryCache : QueryCacheBase
    {
        protected readonly static string _keyExt = "_";
        protected readonly ILocalCache _dictCache = new LocalCache();

        protected string GetCacheKeyExt(string cacheType)
        {
            return cacheType + _keyExt;
        }

        protected string GetCacheKey(string cacheType, string cacheKey)
        {
            return GetCacheKeyExt(cacheType) + cacheKey;
        }

        protected override bool TryGetCache<TRtn>(string cacheType, string cacheKey, out TRtn rtn, Type rtnType = null)
        {
            return _dictCache.TryGetCache<TRtn>(GetCacheKey(cacheType, cacheKey), out rtn);
        }

        protected override bool TryGetCacheAndUpdateCacheTime<TRtn>(string cacheType, string cacheKey, DateTime expiry, 
            out TRtn rtn, Type rtnType = null)
        {
            return _dictCache.TryGetCache<TRtn>(GetCacheKey(cacheType, cacheKey), out rtn, expiry);
        }

        protected override void SetCache<TM>(string cacheType, string cacheKey, TM cacheModel, Type rtnType = null)
        {
            _dictCache.SetCache(GetCacheKey(cacheType, cacheKey), cacheModel);
        }

        protected override void SetCache<TM>(string cacheType, string cacheKey, TM cacheModel, DateTime expiry, Type rtnType = null)
        {
            _dictCache.SetCache(GetCacheKey(cacheType, cacheKey), cacheModel, expiry);
        }

        public override void Remove(string cacheType, string cacheKey)
        {
            _dictCache.Remove(GetCacheKey(cacheType, cacheKey));
        }

        public override void RemoveRange(string cacheType)
        {
            if (_dictCache.Caches?.Count > 0)
            {
                string kt = GetCacheKeyExt(cacheType);
                _dictCache.RemoveRange(_dictCache.Caches.Keys.Where(l => l.StartsWith(kt)).ToList()); 
            }
        }

        public override void Clear()
        {
            _dictCache.Clear();
        }

        public override void Dispose()
        {
            _dictCache.Dispose();
        }
    }
}
