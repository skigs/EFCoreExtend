using EFCoreExtend.EFCache;
using EFCoreExtend.Extensions.EFQueryCaches;
using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class LongCountQueryCacheExtensions
    {
        readonly static string _cacheType = EFHelper.Services.EFCoreExUtility.GetEnumDescription(EFQueryCacheType.LongCount, true);
        public static string CacheType => _cacheType;

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static long LongCountCache<TEntity, T>(this IEFQueryCache cache, IQueryable<T> query, TimeSpan expiry)
        {
            return cache.Cache<TEntity, long>(_cacheType, query, () => query.LongCount(), expiry);
        }

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static long LongCountCache<TEntity, T>(this IEFQueryCache cache, IQueryable<T> query, DateTime expiry)
        {
            return cache.Cache<TEntity, long>(_cacheType, query, () => query.LongCount(), expiry);
        }

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static long LongCountCache<TEntity, T>(this IEFQueryCache cache, IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache<TEntity, long>(_cacheType, query, () => query.LongCount(), expiryPolicy);
        }

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static long LongCountCache<T>(this IEFQueryCache cache, Type tableEntityType, IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableEntityType, _cacheType, query, () => query.LongCount(), expiryPolicy);
        }

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static long LongCountCache<T>(this IEFQueryCache cache, string tableName, IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, _cacheType, query, () => query.LongCount(), expiryPolicy);
        }

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey"></param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static long LongCountCache(this IEFQueryCache cache, string tableName, string cacheKey, Func<long> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, _cacheType, cacheKey, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 移除LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="query"></param>
        public static void LongCountRemove<TEntity>(this IEFQueryCache cache, IQueryable query)
        {
            cache.Remove<TEntity>(_cacheType, query);
        }

        /// <summary>
        /// 移除LongCount缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="query"></param>
        public static void LongCountRemove(this IEFQueryCache cache, Type tableEntityType, IQueryable query)
        {
            cache.Remove(tableEntityType, _cacheType, query);
        }

        /// <summary>
        /// 移除LongCount缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="query"></param>
        public static void LongCountRemove(this IEFQueryCache cache, string tableName, IQueryable query)
        {
            cache.Remove(tableName, _cacheType, query);
        }

        /// <summary>
        /// 移除LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存的key</param>
        public static void LongCountRemove<TEntity>(this IEFQueryCache cache, string cacheKey)
        {
            cache.Remove<TEntity>(_cacheType, cacheKey);
        }

        /// <summary>
        /// 移除LongCount缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void LongCountRemove(this IEFQueryCache cache, Type tableEntityType, string cacheKey)
        {
            cache.Remove(tableEntityType, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除LongCount缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void LongCountRemove(this IEFQueryCache cache, string tableName, string cacheKey)
        {
            cache.Remove(tableName, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除指定表下的所有LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        public static void LongCountRemove<TEntity>(this IEFQueryCache cache)
        {
            cache.Remove<TEntity>(_cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有LongCount缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        public static void LongCountRemove(this IEFQueryCache cache, Type tableEntityType)
        {
            cache.Remove(tableEntityType, _cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有LongCount缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        public static void LongCountRemove(this IEFQueryCache cache, string tableName)
        {
            cache.Remove(tableName, _cacheType);
        }

        #region IQueryable
        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static long LongCountCache<TEntity, T>(this IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.LongCountCache<TEntity, T>(query, expiryPolicy);
        }

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static long LongCountCache<TEntity, T>(this IQueryable<T> query, TimeSpan expiry)
        {
            return EFHelper.Services.Cache.LongCountCache<TEntity, T>(query, expiry);
        }

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static long LongCountCache<TEntity, T>(this IQueryable<T> query, DateTime expiry)
        {
            return EFHelper.Services.Cache.LongCountCache<TEntity, T>(query, expiry);
        }

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static long LongCountCache<T>(this IQueryable<T> query, Type tableEntityType, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.LongCountCache<T>(tableEntityType, query, expiryPolicy);
        }

        /// <summary>
        /// LongCount缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="tableName">表名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static long LongCountCache<T>(this IQueryable<T> query, string tableName, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.LongCountCache<T>(tableName, query, expiryPolicy);
        }

        /// <summary>
        /// 移除LongCount缓存
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        public static void LongCountCacheRemove(this IQueryable query, Type tableEntityType)
        {
            EFHelper.Services.Cache.LongCountRemove(tableEntityType, query);
        }

        /// <summary>
        /// 移除LongCount缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="query"></param>
        public static void LongCountCacheRemove<TEntity>(this IQueryable query)
        {
            EFHelper.Services.Cache.LongCountRemove<TEntity>(query);
        }

        /// <summary>
        /// 移除LongCount缓存
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableName">表名</param>
        public static void LongCountCacheRemove(this IQueryable query, string tableName)
        {
            EFHelper.Services.Cache.LongCountRemove(tableName, query);
        }
        #endregion

    }
}
