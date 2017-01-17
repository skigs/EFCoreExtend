using EFCoreExtend.Evaluators;
using EFCoreExtend.Evaluators.Default;
using System;
using System.Linq;

namespace EFCoreExtend.EFCache.BaseCache.Default
{
    public abstract class QueryCacheBase : IQueryCache
    {
        public TRtn Cache<TRtn>(string cacheType, string cacheKey, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            cacheKey.CheckStringIsNullOrEmpty(nameof(cacheKey));
            cacheType.CheckStringIsNullOrEmpty(nameof(cacheType));
            //toDBGet.CheckNull(nameof(toDBGet));

            TRtn rtn;
            bool bRtn;

            if (expiryPolicy == null)
            {
                bRtn = TryGetCache(cacheType, cacheKey, out rtn);
            }
            else
            {
                var expiryTime = expiryPolicy.GetExpiryTime();
                if (expiryPolicy.IsUpdateEach && expiryTime.HasValue)
                {
                    bRtn = TryGetCacheAndUpdateCacheTime(cacheType, cacheKey, expiryTime.Value, out rtn);
                }
                else
                {
                    bRtn = TryGetCache(cacheType, cacheKey, out rtn);
                }
            }

            if (!bRtn)
            {
                rtn = toDBGet();
                if (expiryPolicy == null)
                {
                    SetCache(cacheType, cacheKey, rtn);
                }
                else
                {
                    var expiryTime = expiryPolicy.GetExpiryTime();
                    if (expiryTime.HasValue)
                    {
                        SetCache(cacheType, cacheKey, rtn, expiryTime.Value);
                    }
                    else
                    {
                        SetCache(cacheType, cacheKey, rtn);
                    }
                }
            }

            return rtn;
        }

        #region abstract
        protected abstract bool TryGetCache<TRtn>(string cacheType, string cacheKey, out TRtn rtn);
        protected abstract bool TryGetCacheAndUpdateCacheTime<TRtn>(string cacheType, string cacheKey, DateTime expiry, out TRtn rtn);

        protected abstract void SetCache<TM>(string cacheType, string cacheKey, TM cacheModel);
        protected abstract void SetCache<TM>(string cacheType, string cacheKey, TM cacheModel, DateTime expiry);

        public abstract void Remove(string cacheType, string cacheKey);

        public abstract void RemoveRange(string cacheType);

        public abstract void Clear();

        public abstract void Dispose();
        #endregion

    }
}
