using EFCoreExtend.EFCache;
using EFCoreExtend.Extensions.EFQueryCaches;
using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class ListQueryCacheExtensions
    {
        readonly static string _cacheType = EFHelper.Services.EFCoreExUtility.GetEnumDescription(EFQueryCacheType.List, true);
        public static string CacheType => _cacheType;

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<TEntity, T>(this IEFQueryCache cache, IQueryable<T> query, TimeSpan expiry)
        {
            return cache.Cache<TEntity, IReadOnlyList<T>>(_cacheType, query, () => query.ToList(), expiry);
        }

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<TEntity, T>(this IEFQueryCache cache, IQueryable<T> query, DateTime expiry)
        {
            return cache.Cache<TEntity, IReadOnlyList<T>>(_cacheType, query, () => query.ToList(), expiry);
        }

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<TEntity, T>(this IEFQueryCache cache, IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache<TEntity, IReadOnlyList<T>>(_cacheType, query, () => query.ToList(), expiryPolicy);
        }

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<T>(this IEFQueryCache cache, Type tableEntityType, IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableEntityType, _cacheType, query, () => query.ToList(), expiryPolicy);
        }

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<T>(this IEFQueryCache cache, string tableName, IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, _cacheType, query, () => query.ToList(), expiryPolicy);
        }

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="T">List缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<T>(this IEFQueryCache cache, string tableName, string cacheKey, 
            Func<IReadOnlyList<T>> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, _cacheType, cacheKey, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 移除List缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="query"></param>
        public static void ListRemove<TEntity>(this IEFQueryCache cache, IQueryable query)
        {
            cache.Remove<TEntity>(_cacheType, query);
        }

        /// <summary>
        /// 移除List缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="query"></param>
        public static void ListRemove(this IEFQueryCache cache, Type tableEntityType, IQueryable query)
        {
            cache.Remove(tableEntityType, _cacheType, query);
        }

        /// <summary>
        /// 移除List缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="query"></param>
        public static void ListRemove(this IEFQueryCache cache, string tableName, IQueryable query)
        {
            cache.Remove(tableName, _cacheType, query);
        }

        /// <summary>
        /// 移除List缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存的key</param>
        public static void ListRemove<TEntity>(this IEFQueryCache cache, string cacheKey)
        {
            cache.Remove<TEntity>(_cacheType, cacheKey);
        }

        /// <summary>
        /// 移除List缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void ListRemove(this IEFQueryCache cache, Type tableEntityType, string cacheKey)
        {
            cache.Remove(tableEntityType, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除List缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void ListRemove(this IEFQueryCache cache, string tableName, string cacheKey)
        {
            cache.Remove(tableName, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除指定表下的所有List缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        public static void ListRemove<TEntity>(this IEFQueryCache cache)
        {
            cache.Remove<TEntity>(_cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有List缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        public static void ListRemove(this IEFQueryCache cache, Type tableEntityType)
        {
            cache.Remove(tableEntityType, _cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有List缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        public static void ListRemove(this IEFQueryCache cache, string tableName)
        {
            cache.Remove(tableName, _cacheType);
        }

        #region IQueryable
        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<TEntity, T>(this IQueryable<T> query, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.ListCache<TEntity, T>(query, expiryPolicy);
        }

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<TEntity, T>(this IQueryable<T> query, TimeSpan expiry)
        {
            return EFHelper.Services.Cache.ListCache<TEntity, T>(query, expiry);
        }

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<TEntity, T>(this IQueryable<T> query, DateTime expiry)
        {
            return EFHelper.Services.Cache.ListCache<TEntity, T>(query, expiry);
        }

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<T>(this IQueryable<T> query, Type tableEntityType, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.ListCache<T>(tableEntityType, query, expiryPolicy);
        }

        /// <summary>
        /// ToList缓存
        /// </summary>
        /// <typeparam name="T">IQueryable的泛型类型</typeparam>
        /// <param name="query"></param>
        /// <param name="tableName">表名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ListCache<T>(this IQueryable<T> query, string tableName, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.ListCache<T>(tableName, query, expiryPolicy);
        }

        /// <summary>
        /// 移除List缓存
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        public static void ListCacheRemove(this IQueryable query, Type tableEntityType)
        {
            EFHelper.Services.Cache.ListRemove(tableEntityType, query);
        }

        /// <summary>
        /// 移除List缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="query"></param>
        public static void ListCacheRemove<TEntity>(this IQueryable query)
        {
            EFHelper.Services.Cache.ListRemove<TEntity>(query);
        }

        /// <summary>
        /// 移除List缓存
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableName">表名</param>
        public static void ListCacheRemove(this IQueryable query, string tableName)
        {
            EFHelper.Services.Cache.ListRemove(tableName, query);
        }
        #endregion
    }
}
