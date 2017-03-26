using EFCoreExtend.EFCache.BaseCache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.Default
{
    public class EFCache : IEFCache
    {
        readonly IQueryCacheContainerMgr _cacheContainerMgr;
        public IQueryCacheContainerMgr CacheContainerMgr => _cacheContainerMgr;

        public EFCache()
        {
            _cacheContainerMgr = new QueryCacheContainerMgr();
        }

        public EFCache(IQueryCacheContainerMgr cacheContainerMgr)
        {
            cacheContainerMgr.CheckNull(nameof(cacheContainerMgr));

            _cacheContainerMgr = cacheContainerMgr;
        }

        public TRtn Cache<TRtn>(string tableName, string cacheType, string cacheKey, Func<TRtn> toDBGet, 
            IQueryCacheExpiryPolicy expiryPolicy, Type rtnType = null)
        {
            IQueryCache cache;
            if (CacheContainerMgr.TryGetIfUseCache(tableName, out cache))
            {
                return cache.Cache(cacheType, cacheKey, toDBGet, expiryPolicy, rtnType);
            }
            else
            {
                return toDBGet();
            }
        }

        public void Remove(string tableName, string cacheType, string cacheKey)
        {
            IQueryCache cache;
            if (CacheContainerMgr.TryGetIfUseCache(tableName, out cache))
            {
                cache.Remove(cacheType, cacheKey);
            }
        }

        public void Remove(string tableName, string cacheType)
        {
            IQueryCache cache;
            if (CacheContainerMgr.TryGetIfUseCache(tableName, out cache))
            {
                cache.RemoveRange(cacheType);
            }
        }

        public void Remove(string tableName)
        {
            IQueryCache cache;
            if (CacheContainerMgr.TryGetIfUseCache(tableName, out cache))
            {
                cache.Clear();
            }
        }

        public void Clear()
        {
            CacheContainerMgr.Clear();
        }

        public void Dispose()
        {
            CacheContainerMgr.Dispose();
        }

    }
}
