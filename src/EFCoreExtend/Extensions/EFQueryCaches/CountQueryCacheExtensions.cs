using EFCoreExtend.EFCache;
using EFCoreExtend.Extensions.EFQueryCaches;
using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class CountQueryCacheExtensions
    {
        readonly static string _cacheType = EFHelper.Services.EFCoreExUtility.GetEnumDescription(EFQueryCacheType.Count, true);
        public static string CacheType => _cacheType;

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="query">IQueryable</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static int CountCache<TEntity, T>(this IEFQueryCache cache, IQueryable<T> query, TimeSpan expiry)
        {
            return cache.Cache<TEntity, int>(_cacheType, query, () => query.Count(), expiry);
        }

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="query">IQueryable</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static int CountCache<TEntity, T>(this IEFQueryCache cache, IQueryable<T> query, DateTime expiry)
        {
            return cache.Cache<TEntity, int>(_cacheType, query, () => query.Count(), expiry);
        }

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="query">IQueryable</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static int CountCache<TEntity, T>(this IEFQueryCache cache, IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache<TEntity, int>(_cacheType, query, () => query.Count(), expiryPolicy);
        }

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static int CountCache<T>(this IEFQueryCache cache, Type tableEntityType, IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableEntityType, _cacheType, query, () => query.Count(), expiryPolicy);
        }

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static int CountCache<T>(this IEFQueryCache cache, string tableName, IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, _cacheType, query, () => query.Count(), expiryPolicy);
        }

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static int CountCache(this IEFQueryCache cache, string tableName, string cacheKey, 
            Func<int> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, _cacheType, cacheKey, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 移除Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="query"></param>
        public static void CountRemove<TEntity>(this IEFQueryCache cache, IQueryable query)
        {
            cache.Remove<TEntity>(_cacheType, query);
        }

        /// <summary>
        /// 移除Count缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="query"></param>
        public static void CountRemove(this IEFQueryCache cache, Type tableEntityType, IQueryable query)
        {
            cache.Remove(tableEntityType, _cacheType, query);
        }

        /// <summary>
        /// 移除Count缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="query"></param>
        public static void CountRemove(this IEFQueryCache cache, string tableName, IQueryable query)
        {
            cache.Remove(tableName, _cacheType, query);
        }

        /// <summary>
        /// 移除Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存的key</param>
        public static void CountRemove<TEntity>(this IEFQueryCache cache, string cacheKey)
        {
            cache.Remove<TEntity>(_cacheType, cacheKey);
        }

        /// <summary>
        /// 移除Count缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void CountRemove(this IEFQueryCache cache, Type tableEntityType, string cacheKey)
        {
            cache.Remove(tableEntityType, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除Count缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void CountRemove(this IEFQueryCache cache, string tableName, string cacheKey)
        {
            cache.Remove(tableName, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除指定表下的所有Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        public static void CountRemove<TEntity>(this IEFQueryCache cache)
        {
            cache.Remove<TEntity>(_cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有Count缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        public static void CountRemove(this IEFQueryCache cache, Type tableEntityType)
        {
            cache.Remove(tableEntityType, _cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有Count缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        public static void CountRemove(this IEFQueryCache cache, string tableName)
        {
            cache.Remove(tableName, _cacheType);
        }

        #region IQueryable
        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static int CountCache<TEntity, T>(this IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.CountCache<TEntity, T>(query, expiryPolicy);
        }

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static int CountCache<TEntity, T>(this IQueryable<T> query, TimeSpan expiry)
        {
            return EFHelper.Services.Cache.CountCache<TEntity, T>(query, expiry);
        }

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static int CountCache<TEntity, T>(this IQueryable<T> query, DateTime expiry)
        {
            return EFHelper.Services.Cache.CountCache<TEntity, T>(query, expiry);
        }

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static int CountCache<T>(this IQueryable<T> query, Type tableEntityType, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.CountCache<T>(tableEntityType, query, expiryPolicy);
        }

        /// <summary>
        /// Count缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="tableName">表名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static int CountCache<T>(this IQueryable<T> query, string tableName, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.CountCache<T>(tableName, query, expiryPolicy);
        }

        /// <summary>
        /// 移除Count缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="query"></param>
        public static void CountCacheRemove<TEntity>(this IQueryable query)
        {
            EFHelper.Services.Cache.CountRemove<TEntity>(query);
        }

        /// <summary>
        /// 移除Count缓存
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        public static void CountCacheRemove(this IQueryable query, Type tableEntityType)
        {
            EFHelper.Services.Cache.CountRemove(tableEntityType, query);
        }

        /// <summary>
        /// 移除Count缓存
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableName">表名</param>
        public static void CountCacheRemove(this IQueryable query, string tableName)
        {
            EFHelper.Services.Cache.CountRemove(tableName, query);
        }
        #endregion
    }
}
