using EFCoreExtend.EFCache;
using EFCoreExtend.Extensions.EFQueryCaches;
using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCoreExtend
{
    public static class ScalarCacheExtensions
    {
        readonly static string _cacheType = EFHelper.Services.EFCoreExUtility.GetEnumDescription(EFQueryCacheType.scalar, true);
        public static string CacheType => _cacheType;

        #region CacheKey
        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCache<TEntity>(this IEFQueryCache cache, string cacheKey, Func<object> toDBGet,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache<TEntity, object>(_cacheType, cacheKey, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCache(this IEFQueryCache cache, Type tableEntityType, string cacheKey, Func<object> toDBGet,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableEntityType, _cacheType, cacheKey, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="toDBGet">用于初始化缓存数据（例如：到数据库获取数据进行缓存的操作）</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCache(this IEFQueryCache cache, string tableName, string cacheKey, Func<object> toDBGet,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return cache.Cache(tableName, _cacheType, cacheKey, toDBGet, expiryPolicy);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey">缓存的key</param>
        public static void ScalarRemove<TEntity>(this IEFQueryCache cache, string cacheKey)
        {
            cache.Remove<TEntity>(_cacheType, cacheKey);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void ScalarRemove(this IEFQueryCache cache, Type tableEntityType, string cacheKey)
        {
            cache.Remove(tableEntityType, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        /// <param name="cacheKey">缓存的key</param>
        public static void ScalarRemove(this IEFQueryCache cache, string tableName, string cacheKey)
        {
            cache.Remove(tableName, _cacheType, cacheKey);
        }

        /// <summary>
        /// 移除指定表下的所有SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="cache"></param>
        public static void ScalarRemove<TEntity>(this IEFQueryCache cache)
        {
            cache.Remove<TEntity>(_cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        public static void ScalarRemove(this IEFQueryCache cache, Type tableEntityType)
        {
            cache.Remove(tableEntityType, _cacheType);
        }

        /// <summary>
        /// 移除指定表下的所有SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="tableName">表名</param>
        public static void ScalarRemove(this IEFQueryCache cache, string tableName)
        {
            cache.Remove(tableName, _cacheType);
        }

        #endregion

        #region SqlAndParams
        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCache<TEntity>(this DbContext db, string sql,
            IDataParameter[] sqlParams,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.Cache<TEntity, object>(_cacheType, sql, sqlParams, 
                () => db.Scalar(sql, sqlParams), expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCache(this DbContext db, Type tableEntityType, string sql,
            IDataParameter[] sqlParams,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.Cache(tableEntityType, _cacheType, sql, sqlParams, 
                () => db.Scalar(sql, sqlParams), expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCache(this DbContext db, string tableName, string sql,
            IDataParameter[] sqlParams,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.Cache(tableName, _cacheType, sql, sqlParams, 
                () => db.Scalar(sql, sqlParams), expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCacheUseDict<TEntity>(this DbContext db, string sql,
            IDictionary<string, object> sqlParams,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.CacheUseDict<TEntity, object>(_cacheType, sql, sqlParams, 
                () => db.ScalarUseDict(sql, sqlParams), expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCacheUseDict(this DbContext db, Type tableEntityType, string sql,
            IDictionary<string, object> sqlParams,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.CacheUseDict(tableEntityType, _cacheType, sql, sqlParams, 
                () => db.ScalarUseDict(sql, sqlParams), expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCacheUseDict(this DbContext db, string tableName, string sql,
            IDictionary<string, object> sqlParams,
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.CacheUseDict(tableName, _cacheType, sql, sqlParams, 
                () => db.ScalarUseDict(sql, sqlParams), expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCacheUseModel<TEntity>(this DbContext db, string sql,
            object sqlParamModel, IEnumerable<string> ignoreProptsForParamModel, 
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.CacheUseModel<TEntity, object>(_cacheType, sql, sqlParamModel, ignoreProptsForParamModel,
                () => db.ScalarUseModel(sql, sqlParamModel, ignoreProptsForParamModel), expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCacheUseModel(this DbContext db, Type tableEntityType, string sql,
            object sqlParamModel, IEnumerable<string> ignoreProptsForParamModel, 
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.CacheUseModel(tableEntityType, _cacheType, sql, sqlParamModel, ignoreProptsForParamModel,
                () => db.ScalarUseModel(sql, sqlParamModel, ignoreProptsForParamModel), expiryPolicy);
        }

        /// <summary>
        /// SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="expiryPolicy">缓存过期策略</param>
        /// <returns></returns>
        public static object ScalarCacheUseModel(this DbContext db, string tableName, string sql,
            object sqlParamModel, IEnumerable<string> ignoreProptsForParamModel, 
            IQueryCacheExpiryPolicy expiryPolicy)
        {
            return EFHelper.Services.Cache.CacheUseModel(tableName, _cacheType, sql, sqlParamModel, ignoreProptsForParamModel,
                () => db.ScalarUseModel(sql, sqlParamModel, ignoreProptsForParamModel), expiryPolicy);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void ScalarCacheRemove<TEntity>(this DbContext db, string sql,
            ICollection<IDataParameter> sqlParams)
        {
            EFHelper.Services.Cache.Remove(typeof(TEntity), _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void ScalarCacheRemove(this DbContext db, string tableName, string sql,
            ICollection<IDataParameter> sqlParams)
        {
            EFHelper.Services.Cache.Remove(tableName, _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void ScalarCacheRemove(this DbContext db, Type tableEntityType, string sql,
            ICollection<IDataParameter> sqlParams)
        {
            EFHelper.Services.Cache.Remove(tableEntityType, _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void ScalarCacheRemoveUseDict<TEntity>(this DbContext db, string sql,
            IDictionary<string, object> sqlParams)
        {
            EFHelper.Services.Cache.RemoveUseDict(typeof(TEntity), _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void ScalarCacheRemoveUseDict(this DbContext db, Type tableEntityType, string sql,
            IDictionary<string, object> sqlParams)
        {
            EFHelper.Services.Cache.RemoveUseDict(tableEntityType, _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParams">sql的参数</param>
        public static void ScalarCacheRemoveUseDict(this DbContext db, string tableName, string sql,
            IDictionary<string, object> sqlParams)
        {
            EFHelper.Services.Cache.RemoveUseDict(tableName, _cacheType, sql, sqlParams);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        public static void ScalarCacheRemoveUseModel(this DbContext db, Type tableEntityType, string sql,
            object sqlParamModel, IEnumerable<string> ignoreProptsForParamModel)
        {
            EFHelper.Services.Cache.RemoveUseModel(tableEntityType, _cacheType, sql, sqlParamModel, ignoreProptsForParamModel);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        public static void ScalarCacheRemoveUseModel<TEntity>(this DbContext db, string sql,
            object sqlParamModel, IEnumerable<string> ignoreProptsForParamModel)
        {
            EFHelper.Services.Cache.RemoveUseModel(typeof(TEntity), _cacheType, sql, sqlParamModel, ignoreProptsForParamModel);
        }

        /// <summary>
        /// 移除SqlScalar缓存(缓存的类型为：scalar)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sql"></param>
        /// <param name="sqlParamModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        public static void ScalarCacheRemoveUseModel(this DbContext db, string tableName, string sql,
            object sqlParamModel, IEnumerable<string> ignoreProptsForParamModel)
        {
            EFHelper.Services.Cache.RemoveUseModel(tableName, _cacheType, sql, sqlParamModel, ignoreProptsForParamModel);
        }

        #endregion

    }
}
