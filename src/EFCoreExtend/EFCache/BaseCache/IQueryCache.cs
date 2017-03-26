using System;
using System.Linq;

namespace EFCoreExtend.EFCache.BaseCache
{
    public interface IQueryCache : IDisposable
    {
        /// <summary>
        /// 进行数据缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存数据类型</typeparam>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <param name="rtnType">缓存数据类型(对于反序列化成对象(JSON => Type)时有用，TRtn可能为object，但是可以传递Type进行反序列化成对象)</param>
        /// <returns>缓存数据</returns>
        TRtn Cache<TRtn>(string cacheType, string cacheKey, Func<TRtn> toDBGet, 
            IQueryCacheExpiryPolicy expiryPolicy, Type rtnType = null);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="cacheKey">缓存的key</param>
        void Remove(string cacheType, string cacheKey);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cacheType">缓存的类型</param>
        void RemoveRange(string cacheType);

        /// <summary>
        /// 清空缓存
        /// </summary>
        void Clear();
    }
}
