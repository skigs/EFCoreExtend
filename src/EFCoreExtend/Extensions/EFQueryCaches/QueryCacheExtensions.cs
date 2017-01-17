using EFCoreExtend.EFCache;
using EFCoreExtend.Extensions.EFQueryCaches;
using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCoreExtend
{
    public static class QueryCacheExtensions
    {
        readonly static string _cacheType = EFHelper.Services.EFCoreExUtility.GetEnumDescription(EFQueryCacheType.query, true);
        public static string CacheType => _cacheType;

        #region CacheKey
        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCache<TEntity, T>(this IEFQueryCache cache, string cacheKey,
            Func<IReadOnlyList<T>> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache<TEntity, IReadOnlyList<T>>(_cacheType, cacheKey, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCache<T>(this IEFQueryCache cache, Type tableEntityType, string cacheKey,
            Func<IReadOnlyList<T>> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableEntityType, _cacheType, cacheKey, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCache<T>(this IEFQueryCache cache, string tableName, string cacheKey,
            Func<IReadOnlyList<T>> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, _cacheType, cacheKey, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存的key</param>
        public static void QueryRemove<TEntity>(this IEFQueryCache cache, string cacheKey)
        {
            cache.Remove<TEntity>(_cacheType, cacheKey);
        }

        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void QueryRemove(this IEFQueryCache cache, Type tableEntityType, string cacheKey)
        {
            cache.Remove(tableEntityType, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void QueryRemove(this IEFQueryCache cache, string tableName, string cacheKey)
        {
            cache.Remove(tableName, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除指定表下的所有SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        public static void QueryRemove<TEntity>(this IEFQueryCache cache)
        {
            cache.Remove<TEntity>(_cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        public static void QueryRemove(this IEFQueryCache cache, Type tableEntityType)
        {
            cache.Remove(tableEntityType, _cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        public static void QueryRemove(this IEFQueryCache cache, string tableName)
        {
            cache.Remove(tableName, _cacheType);
        }

        #endregion

        #region SqlAndParams
        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCache<TEntity, T>(this DbContext db, string sql,
            IDataParameter[] sqlParams, IReadOnlyCollection<string> ignoreProptsForRtnType,
            IQueryCacheExpiryPolicy expiryPolicy)
            where T : new()
        {
            return EFHelper.Services.Cache.Cache<TEntity, IReadOnlyList<T>>(_cacheType, sql, sqlParams, 
                () => db.Query<T>(sql, sqlParams, ignoreProptsForRtnType), expiryPolicy);
        }

        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCache<T>(this DbContext db, Type tableEntityType, string sql,
            IDataParameter[] sqlParams, IReadOnlyCollection<string> ignoreProptsForRtnType,
            IQueryCacheExpiryPolicy expiryPolicy)
            where T : new()
        {
            return EFHelper.Services.Cache.Cache(tableEntityType, _cacheType, sql, sqlParams,
                () => db.Query<T>(sql, sqlParams, ignoreProptsForRtnType), expiryPolicy);
        }

        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCache<T>(this DbContext db, string tableName, string sql,
            IDataParameter[] sqlParams, IReadOnlyCollection<string> ignoreProptsForRtnType,
            IQueryCacheExpiryPolicy expiryPolicy)
            where T : new()
        {
            return EFHelper.Services.Cache.Cache(tableName, _cacheType, sql, sqlParams,
                () => db.Query<T>(sql, sqlParams, ignoreProptsForRtnType), expiryPolicy);
        }

        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCacheUseDict<TEntity, T>(this DbContext db, string sql,
            IReadOnlyDictionary<string, object> sqlParams, IReadOnlyCollection<string> ignoreProptsForRtnType,
            IQueryCacheExpiryPolicy expiryPolicy)
            where T : new()
        {
            return EFHelper.Services.Cache.CacheUseDict<TEntity, IReadOnlyList<T>>(_cacheType, sql, sqlParams,
                () => db.QueryUseDict<T>(sql, sqlParams, ignoreProptsForRtnType), expiryPolicy);
        }

        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCacheUseDict<T>(this DbContext db, Type tableEntityType, string sql,
            IReadOnlyDictionary<string, object> sqlParams, IReadOnlyCollection<string> ignoreProptsForRtnType,
            IQueryCacheExpiryPolicy expiryPolicy)
            where T : new()
        {
            return EFHelper.Services.Cache.CacheUseDict(tableEntityType, _cacheType, sql, sqlParams,
                () => db.QueryUseDict<T>(sql, sqlParams, ignoreProptsForRtnType), expiryPolicy);
        }

        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCacheUseDict<T>(this DbContext db, string tableName, string sql,
            IReadOnlyDictionary<string, object> sqlParams, IReadOnlyCollection<string> ignoreProptsForRtnType,
            IQueryCacheExpiryPolicy expiryPolicy)
            where T : new()
        {
            return EFHelper.Services.Cache.CacheUseDict(tableName, _cacheType, sql, sqlParams,
                () => db.QueryUseDict<T>(sql, sqlParams, ignoreProptsForRtnType), expiryPolicy);
        }


        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCacheUseModel<TEntity, T>(this DbContext db, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyCollection<string> ignoreProptsForRtnType,
            IQueryCacheExpiryPolicy expiryPolicy)
            where T : new()
        {
            return EFHelper.Services.Cache.CacheUseModel<TEntity, IReadOnlyList<T>>(_cacheType, sql, sqlParamModel, ignoreProptsForParamModel,
                () => db.QueryUseModel<T>(sql, sqlParamModel, ignoreProptsForParamModel, ignoreProptsForRtnType), expiryPolicy);
        }

        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCacheUseModel<T>(this DbContext db, Type tableEntityType, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyCollection<string> ignoreProptsForRtnType,
            IQueryCacheExpiryPolicy expiryPolicy)
            where T : new()
        {
            return EFHelper.Services.Cache.CacheUseModel(tableEntityType, _cacheType, sql, sqlParamModel, ignoreProptsForParamModel,
                () => db.QueryUseModel<T>(sql, sqlParamModel, ignoreProptsForParamModel, ignoreProptsForRtnType), expiryPolicy);
        }

        /// <summary>
        /// SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryCacheUseModel<T>(this DbContext db, string tableName, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyCollection<string> ignoreProptsForRtnType,
            IQueryCacheExpiryPolicy expiryPolicy)
            where T : new()
        {
            return EFHelper.Services.Cache.CacheUseModel(tableName, _cacheType, sql, sqlParamModel, ignoreProptsForParamModel,
                () => db.QueryUseModel<T>(sql, sqlParamModel, ignoreProptsForParamModel, ignoreProptsForRtnType), expiryPolicy);
        }


        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void QueryCacheRemove<TEntity>(this DbContext db, string sql,
            IReadOnlyCollection<IDataParameter> sqlParams)
        {
            EFHelper.Services.Cache.Remove(typeof(TEntity), _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void QueryCacheRemove(this DbContext db, string tableName, string sql,
            IReadOnlyCollection<IDataParameter> sqlParams)
        {
            EFHelper.Services.Cache.Remove(tableName, _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void QueryCacheRemove(this DbContext db, Type tableEntityType, string sql,
            IReadOnlyCollection<IDataParameter> sqlParams)
        {
            EFHelper.Services.Cache.Remove(tableEntityType, _cacheType, sql, sqlParams);
        }


        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void QueryCacheRemoveUseDict(this DbContext db, Type tableEntityType, string sql,
            IReadOnlyDictionary<string, object> sqlParams)
        {
            EFHelper.Services.Cache.RemoveUseDict(tableEntityType, _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void QueryCacheRemoveUseDict<TEntity>(this DbContext db, string sql,
            IReadOnlyDictionary<string, object> sqlParams)
        {
            EFHelper.Services.Cache.RemoveUseDict(typeof(TEntity), _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void QueryCacheRemoveUseDict(this DbContext db, string tableName, string sql,
            IReadOnlyDictionary<string, object> sqlParams)
        {
            EFHelper.Services.Cache.RemoveUseDict(tableName, _cacheType, sql, sqlParams);
        }


        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        public static void QueryCacheRemoveUseModel<TEntity>(this DbContext db, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel)
        {
            EFHelper.Services.Cache.RemoveUseModel(typeof(TEntity), _cacheType, sql, sqlParamModel, ignoreProptsForParamModel);
        }

        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        public static void QueryCacheRemoveUseModel(this DbContext db, Type tableEntityType, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel)
        {
            EFHelper.Services.Cache.RemoveUseModel(tableEntityType, _cacheType, sql, sqlParamModel, ignoreProptsForParamModel);
        }

        /// <summary>
        /// 移除SqlQuery缓存(缓存的类型为：query)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        public static void QueryCacheRemoveUseModel(this DbContext db, string tableName, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel)
        {
            EFHelper.Services.Cache.RemoveUseModel(tableName, _cacheType, sql, sqlParamModel, ignoreProptsForParamModel);
        }
        #endregion

    }
}
