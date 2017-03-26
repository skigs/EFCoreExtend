using EFCoreExtend.EFCache.BaseCache;
using EFCoreExtend.Evaluators;
using EFCoreExtend.Evaluators.Default;
using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EFCoreExtend.EFCache.Default
{
    public class EFQueryCache : EFCache, IEFQueryCache
    {
        protected readonly IEvaluator _evaluator = new Evaluator();
        public EFQueryCache()
        {
        }

        public EFQueryCache(IQueryCacheContainerMgr cacheContainerMgr, IEvaluator evaluator)
            :base(cacheContainerMgr)
        {
            evaluator.CheckNull(nameof(evaluator));

            _evaluator = evaluator;
        }

        protected string QueryableToCacheKey(IQueryable query)
        {
            return _evaluator.PartialEval(query.Expression).ToString().ToMD5();
        }

        public TRtn Cache<TRtn>(string tableName, string cacheType, IQueryable query, Func<TRtn> toDBGet, 
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return Cache(tableName, cacheType, QueryableToCacheKey(query), toDBGet, expiryPolicy);
        }

        public void Remove(string tableName, string cacheType, IQueryable query)
        {
            Remove(tableName, cacheType, QueryableToCacheKey(query));
        }
    }
}
