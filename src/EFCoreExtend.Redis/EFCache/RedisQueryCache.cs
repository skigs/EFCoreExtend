using System;
using EFCoreExtend.Redis.EFCache.Helper;
using EFCoreExtend.EFCache.BaseCache.Default;

namespace EFCoreExtend.Redis.EFCache
{
    public class RedisQueryCache : QueryCacheBase
    {
        protected readonly string _splitVal = "#$";  //让表与表，类型与类型区分开的字串
        protected readonly IRedisDBConnect _redisConnect;
        protected readonly string _tableName;

        public RedisQueryCache(string tableName, IRedisDBConnect redisConnect)
        {
            _redisConnect = redisConnect;
            _tableName = tableName;
        }

        protected string GetKey(string cacheType, string cacheKey)
        {
            return _splitVal + _tableName + _splitVal + cacheType + _splitVal + cacheKey;
        }
        protected string GetKeyExt(string cacheType)
        {
            return _splitVal + _tableName + _splitVal + cacheType + _splitVal;
        }
        protected string GetTableExt()
        {
            return _splitVal + _tableName + _splitVal;
        }

        protected override bool TryGetCache<TRtn>(string cacheType, string cacheKey, out TRtn rtn)
        {
            return _redisConnect.TryGet<TRtn>(GetKey(cacheType, cacheKey), out rtn);
        }

        protected override bool TryGetCacheAndUpdateCacheTime<TRtn>(string cacheType, string cacheKey, DateTime expiry, out TRtn rtn)
        {
            return _redisConnect.TryGet<TRtn>(GetKey(cacheType, cacheKey), expiry, out rtn);
        }

        protected override void SetCache<TM>(string cacheType, string cacheKey, TM cacheModel)
        {
            _redisConnect.SetJson(GetKey(cacheType, cacheKey), cacheModel);
        }

        protected override void SetCache<TM>(string cacheType, string cacheKey, TM cacheModel, DateTime expiry)
        {
            _redisConnect.SetJson(GetKey(cacheType, cacheKey), cacheModel, expiry);
        }

        public override void Remove(string cacheType, string cacheKey)
        {
            _redisConnect.DeleteKey(GetKey(cacheType, cacheKey));
        }

        public override void RemoveRange(string cacheType)
        {
            _redisConnect.ClearKeys(GetKeyExt(cacheType) + "*");
        }

        public override void Clear()
        {
            _redisConnect.ClearKeys(GetTableExt() + "*");
        }

        public override void Dispose()
        {
            Clear();
        }
    }
}
