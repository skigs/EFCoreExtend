using EFCoreExtend.EFCache.BaseCache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.Default
{
    public class QueryCacheContainerMgr : IQueryCacheContainerMgr
    {
        readonly object _lockGetCacheContainer = new object();
        IDictionary<string, IQueryCache> _caches = new ConcurrentDictionary<string, IQueryCache>();
        IQueryCacheCreator _queryCacheCreator = new QueryCacheCreator();

        bool _isUseCache = true;
        public bool IsUseCache
        {
            get { return _isUseCache; }
            set { _isUseCache = value; }
        }

        public QueryCacheContainerMgr()
        {
        }

        public QueryCacheContainerMgr(IQueryCacheCreator queryCacheCreator)
        {
            queryCacheCreator.CheckNull(nameof(queryCacheCreator));

            _queryCacheCreator = queryCacheCreator;
        }

        public bool TryGetIfUseCache(string tableName, out IQueryCache cache)
        {
            if (IsUseCache)
            {
                if (!_caches.TryGetValue(tableName, out cache))
                {
                    lock (_lockGetCacheContainer)
                    {
                        if (!_caches.TryGetValue(tableName, out cache))
                        {
                            cache = _queryCacheCreator.Create(tableName);
                            if(cache == null)
                            {
                                throw new ArgumentException($"The type [{_queryCacheCreator.GetType().Name}] to create a {nameof(IQueryCacheCreator)} instance can not be null");
                            }
                            _caches[tableName] = cache;
                        }
                    }
                }
                return true;
            }
            else
            {
                cache = null;
                return false;
            }
        }

        public void Clear()
        {
            var caches = _caches.Values.ToList();
            if (caches != null && caches.Count > 0)
            {
                foreach (var l in caches)
                {
                    try
                    {
                        l.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            _caches.Clear();
        }

        public void Dispose()
        {
            Clear();
        }

    }
}
