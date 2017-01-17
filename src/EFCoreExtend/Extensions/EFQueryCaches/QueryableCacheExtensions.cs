using EFCoreExtend.EFCache;
using EFCoreExtend.EFCache.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class QueryableCacheExtensions
    {
        #region Cache
        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="query"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TEntity, TRtn>(this IQueryable query, string cacheType, Func<TRtn> toDBGet,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.Cache<TEntity, TRtn>(cacheType, query, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="query"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static TRtn Cache<TEntity, TRtn>(this IQueryable query, string cacheType, Func<TRtn> toDBGet,
            TimeSpan expiry)
        {
            return EFHelper.Services.Cache.Cache<TEntity, TRtn>(cacheType, query, toDBGet, expiry);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="query"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static TRtn Cache<TEntity, TRtn>(this IQueryable query, string cacheType, Func<TRtn> toDBGet,
            DateTime expiry)
        {
            return EFHelper.Services.Cache.Cache<TEntity, TRtn>(cacheType, query, toDBGet, expiry);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="query"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TRtn>(this IQueryable query, Type tableEntityType, string cacheType, Func<TRtn> toDBGet,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.Cache<TRtn>(tableEntityType, cacheType, query, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="query"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TRtn>(this IQueryable query, string tableName, string cacheType, Func<TRtn> toDBGet,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.Cache<TRtn>(tableName, cacheType, query, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="query"></param>
        /// <param name="cacheType">缓存的类型</param>
        public static void CacheRemove<TEntity>(this IQueryable query, string cacheType)
        {
            EFHelper.Services.Cache.Remove<TEntity>(cacheType, query);
        }
        
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        public static void CacheRemove(this IQueryable query, Type tableEntityType, string cacheType)
        {
            EFHelper.Services.Cache.Remove(tableEntityType, cacheType, query);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        public static void CacheRemove(this IQueryable query, string tableName, string cacheType)
        {
            EFHelper.Services.Cache.Remove(tableName, cacheType, query);
        }
        #endregion

    }
}
