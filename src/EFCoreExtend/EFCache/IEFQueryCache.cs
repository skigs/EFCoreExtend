using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EFCoreExtend.EFCache
{
    public interface IEFQueryCache : IEFCache
    {
        /// <summary>
        /// 进行数据缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="query">IQueryable</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        TRtn Cache<TRtn>(string tableName, string cacheType, IQueryable query, Func<TRtn> toDBGet,
            IQueryCacheExpiryPolicy expiryPolicy);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="query">IQueryable</param>
        void Remove(string tableName, string cacheType, IQueryable query);
    }
}
