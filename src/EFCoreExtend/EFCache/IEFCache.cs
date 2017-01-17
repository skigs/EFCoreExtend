using EFCoreExtend.EFCache.Default;
using System;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache
{
    public interface IEFCache : IDisposable
    {
        /// <summary>
        /// 进行数据缓存(数据的缓存是根据：tableName + cacheType + cacheKey 进行查询和存储的)
        /// </summary>
        /// <typeparam name="TRtn">缓存数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        TRtn Cache<TRtn>(string tableName, string cacheType, string cacheKey, 
            Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="cacheKey">缓存的key</param>
        void Remove(string tableName, string cacheType, string cacheKey);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        void Remove(string tableName, string cacheType);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="tableName">表名</param>
        void Remove(string tableName);

        /// <summary>
        /// 清空缓存
        /// </summary>
        void Clear();
    }
}
