using EFCoreExtend.EFCache;
using EFCoreExtend.EFCache.Default;
using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class EFQueryCacheExtensions
    {

        #region SqlAndParams

        #region Normal
        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TRtn>(this IEFQueryCache cache, string tableName, string cacheType, string sql,
            IReadOnlyCollection<IDataParameter> sqlParams, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, cacheType,
                EFHelper.Services.EFCoreExUtility.CombineSqlAndParamsToString(sql, sqlParams),
                    toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn CacheUseDict<TRtn>(this IEFQueryCache cache, string tableName, string cacheType, string sql,
            IReadOnlyDictionary<string, object> sqlParams, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, cacheType,
                EFHelper.Services.EFCoreExUtility.CombineSqlAndParamsToString(sql, sqlParams),
                    toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn CacheUseModel<TRtn>(this IEFQueryCache cache, string tableName, string cacheType, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel, Func<TRtn> toDBGet, 
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, cacheType,
                EFHelper.Services.EFCoreExUtility.CombineSqlAndParamsToString(sql,
                EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(sqlParamModel, ignoreProptsForParamModel)),
                    toDBGet, expiryPolicy);
        }
        #endregion

        #region TEntity
        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TEntity, TRtn>(this IEFQueryCache cache, string cacheType, string sql,
            IReadOnlyCollection<IDataParameter> sqlParams, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(typeof(TEntity), cacheType, sql, sqlParams, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn CacheUseDict<TEntity, TRtn>(this IEFQueryCache cache, string cacheType, string sql,
            IReadOnlyDictionary<string, object> sqlParams, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.CacheUseDict(typeof(TEntity), cacheType, sql, sqlParams, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn CacheUseModel<TEntity, TRtn>(this IEFQueryCache cache, string cacheType, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.CacheUseModel(typeof(TEntity), cacheType, sql, sqlParamModel, ignoreProptsForParamModel,
                    toDBGet, expiryPolicy);
        }
        #endregion

        #region TableEntityType
        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TRtn>(this IEFQueryCache cache, Type tableEntityType, string cacheType, string sql,
            IReadOnlyCollection<IDataParameter> sqlParams, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType,
                sql, sqlParams, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn CacheUseDict<TRtn>(this IEFQueryCache cache, Type tableEntityType, string cacheType, string sql,
            IReadOnlyDictionary<string, object> sqlParams, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.CacheUseDict(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType,
                sql, sqlParams, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn CacheUseModel<TRtn>(this IEFQueryCache cache, Type tableEntityType, string cacheType, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel, 
            Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.CacheUseModel(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType,
                sql, sqlParamModel, ignoreProptsForParamModel, toDBGet, expiryPolicy);
        }

        #endregion

        #endregion

        #region TEntity

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="query"></param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static TRtn Cache<TEntity, TRtn>(this IEFQueryCache cache, string cacheType, 
            IQueryable query, Func<TRtn> toDBGet, TimeSpan expiry)
        {
            return cache.Cache(typeof(TEntity), cacheType, query, toDBGet, new QueryCacheExpiryPolicy(expiry));
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="query"></param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public static TRtn Cache<TEntity, TRtn>(this IEFQueryCache cache, string cacheType, 
            IQueryable query, Func<TRtn> toDBGet, DateTime expiry)
        {
            return cache.Cache(typeof(TEntity), cacheType, query, toDBGet, new QueryCacheExpiryPolicy(expiry));
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="query">IQueryable</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TEntity, TRtn>(this IEFQueryCache cache, string cacheType, 
            IQueryable query, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(typeof(TEntity), cacheType, query, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TEntity, TRtn>(this IEFQueryCache cache, string cacheType, 
            string cacheKey, Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(typeof(TEntity), cacheType, cacheKey, toDBGet, expiryPolicy);
        }
        #endregion

        #region TableEntityType
        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="query"></param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TRtn>(this IEFQueryCache cache, Type tableEntityType, string cacheType, IQueryable query, 
            Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType, query, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        /// <typeparam name="TRtn">缓存的数据类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="cacheKey"></param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static TRtn Cache<TRtn>(this IEFQueryCache cache, Type tableEntityType, string cacheType, string cacheKey, 
            Func<TRtn> toDBGet, IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType, cacheKey, toDBGet, expiryPolicy);
        }
        #endregion

        #region Remove
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="query"></param>
        public static void Remove<TEntity>(this IEFQueryCache cache, string cacheType, IQueryable query)
        {
            cache.Remove(typeof(TEntity), cacheType, query);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="query"></param>
        public static void Remove(this IEFQueryCache cache, Type tableEntityType, string cacheType, IQueryable query)
        {
            cache.Remove(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType, query);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void Remove<TEntity>(this IEFQueryCache cache, string cacheType, string cacheKey)
        {
            cache.Remove(typeof(TEntity), cacheType, cacheKey);
        }

        /// <summary>
        /// 移除缓存(指定表下 指定的缓存类型下 的所有缓存)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheType">缓存的类型</param>
        public static void Remove<TEntity>(this IEFQueryCache cache, string cacheType)
        {
            cache.Remove(typeof(TEntity), cacheType);
        }

        /// <summary>
        /// 移除缓存(指定表下的所有缓存)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        public static void Remove<TEntity>(this IEFQueryCache cache)
        {
            cache.Remove(typeof(TEntity));
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void Remove(this IEFQueryCache cache, Type tableEntityType, string cacheType, string cacheKey)
        {
            cache.Remove(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType, cacheKey);
        }

        /// <summary>
        /// 移除缓存(指定表下 指定的缓存类型下 的所有缓存)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        public static void Remove(this IEFQueryCache cache, Type tableEntityType, string cacheType)
        {
            cache.Remove(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType);
        }

        /// <summary>
        /// 移除缓存(指定表下的所有缓存)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        public static void Remove(this IEFQueryCache cache, Type tableEntityType)
        {
            cache.Remove(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType));
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void Remove(this IEFQueryCache cache, Type tableEntityType, string cacheType, string sql,
            IReadOnlyCollection<IDataParameter> sqlParams)
        {
            cache.Remove(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType,
                sql, sqlParams);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void Remove(this IEFQueryCache cache, string tableName, string cacheType, string sql,
            IReadOnlyCollection<IDataParameter> sqlParams)
        {
            cache.Remove(tableName, cacheType,
                EFHelper.Services.EFCoreExUtility.CombineSqlAndParamsToString(sql, sqlParams));
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void RemoveUseDict(this IEFQueryCache cache, Type tableEntityType, string cacheType, string sql,
            IReadOnlyDictionary<string, object> sqlParams)
        {
            cache.RemoveUseDict(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType,
                sql, sqlParams);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void RemoveUseDict(this IEFQueryCache cache, string tableName, string cacheType, string sql,
            IReadOnlyDictionary<string, object> sqlParams)
        {
            cache.Remove(tableName, cacheType,
                EFHelper.Services.EFCoreExUtility.CombineSqlAndParamsToString(sql, sqlParams));
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        public static void RemoveUseModel(this IEFQueryCache cache, Type tableEntityType, string cacheType, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel)
        {
            cache.RemoveUseModel(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), cacheType,
                sql, sqlParamModel, ignoreProptsForParamModel);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheType">缓存的类型</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        public static void RemoveUseModel(this IEFQueryCache cache, string tableName, string cacheType, string sql,
            object sqlParamModel, IReadOnlyCollection<string> ignoreProptsForParamModel)
        {
            cache.Remove(tableName, cacheType,
                EFHelper.Services.EFCoreExUtility.CombineSqlAndParamsToString(sql,
                    EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(sqlParamModel, ignoreProptsForParamModel)));
        }
        #endregion

    }
}
