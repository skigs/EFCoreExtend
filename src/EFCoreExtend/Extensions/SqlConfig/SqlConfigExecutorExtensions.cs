using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Executors;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class SqlConfigExecutorExtensions
    {
        /// <summary>
        /// 转换成策略存储的Dictionary
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, ISqlConfigPolicy> ToPolicies(this ISqlConfigPolicy policy)
        {
            return policy == null ? null :
                    new Dictionary<string, ISqlConfigPolicy>()
                    {
                        { EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(policy.GetType(), true), policy }
                    };
        }

        /// <summary>
        /// 转换成策略存储的Dictionary
        /// </summary>
        /// <param name="policies"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, ISqlConfigPolicy> ToPolicies(this IReadOnlyCollection<ISqlConfigPolicy> policies)
        {
            if(policies?.Count > 0)
            {
                var dict = new Dictionary<string, ISqlConfigPolicy>();
                foreach (var l in policies)
                {
                    dict[EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(l.GetType(), true)] = l;
                }
                return dict;
            }
            else
            {
                return null;
            }
        }

        #region Query

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="cachePolicy">缓存策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseDict<T>(this ISqlConfigExecutor configExecutor,
            IReadOnlyDictionary<string, object> parameters,
            IReadOnlyCollection<string> ignoreProptsForRtnType = null, SqlL2QueryCachePolicy cachePolicy = null)
            where T : new()
        {
            return configExecutor.Query<T>(EFHelper.Services.SqlParamConverter.DictionaryToDBParams(configExecutor.DB, parameters),
                ignoreProptsForRtnType, ToPolicies(cachePolicy));
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseDict<T>(this ISqlConfigExecutor configExecutor,
            IReadOnlyDictionary<string, object> parameters,
            IReadOnlyCollection<string> ignoreProptsForRtnType,
            IReadOnlyCollection<ISqlConfigPolicy> policies)
            where T : new()
        {
            return configExecutor.Query<T>(
                EFHelper.Services.SqlParamConverter.DictionaryToDBParams(configExecutor.DB, parameters),
                ignoreProptsForRtnType, ToPolicies(policies));
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseDict<T>(this ISqlConfigExecutor configExecutor,
            IReadOnlyDictionary<string, object> parameters,
            IReadOnlyCollection<string> ignoreProptsForRtnType,
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies)
            where T : new()
        {
            return configExecutor.Query<T>(
                EFHelper.Services.SqlParamConverter.DictionaryToDBParams(configExecutor.DB, parameters),
                ignoreProptsForRtnType, policies);
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="cachePolicy">缓存策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseModel<T>(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyCollection<string> ignoreProptsForRtnType, SqlL2QueryCachePolicy cachePolicy = null)
            where T : new()
        {
            return configExecutor.Query<T>(
                EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), ignoreProptsForRtnType, ToPolicies(cachePolicy));
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseModel<T>(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel = null,
            params string[] ignoreProptsForRtnType)
            where T : new()
        {
            return configExecutor.Query<T>(
                EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), ignoreProptsForRtnType, null);
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseModel<T>(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyCollection<string> ignoreProptsForRtnType, 
            IReadOnlyCollection<ISqlConfigPolicy> policies)
            where T : new()
        {
            return configExecutor.Query<T>(EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), ignoreProptsForRtnType, ToPolicies(policies));
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseModel<T>(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyCollection<string> ignoreProptsForRtnType,
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies)
            where T : new()
        {
            return configExecutor.Query<T>(EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), ignoreProptsForRtnType, policies);
        }
        #endregion

        #region Scalar

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="cachePolicy">缓存策略</param>
        /// <returns></returns>
        public static object ScalarUseDict(this ISqlConfigExecutor configExecutor, 
            IReadOnlyDictionary<string, object> parameters,
            SqlL2QueryCachePolicy cachePolicy = null)
        {
            return configExecutor.Scalar(EFHelper.Services.SqlParamConverter.DictionaryToDBParams(configExecutor.DB, parameters),
                ToPolicies(cachePolicy));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static object ScalarUseDict(this ISqlConfigExecutor configExecutor,
            IReadOnlyDictionary<string, object> parameters,
            IReadOnlyCollection<ISqlConfigPolicy> policies)
        {
            return configExecutor.Scalar(EFHelper.Services.SqlParamConverter.DictionaryToDBParams(configExecutor.DB, parameters),
                ToPolicies(policies));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static object ScalarUseDict(this ISqlConfigExecutor configExecutor,
            IReadOnlyDictionary<string, object> parameters,
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies)
        {
            return configExecutor.Scalar(EFHelper.Services.SqlParamConverter.DictionaryToDBParams(configExecutor.DB, parameters),
                policies);
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="cachePolicy">缓存策略</param>
        /// <returns></returns>
        public static object ScalarUseModel(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel,
            SqlL2QueryCachePolicy cachePolicy = null)
        {
            return configExecutor.Scalar(EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), ToPolicies(cachePolicy));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static object ScalarUseModel(this ISqlConfigExecutor configExecutor, object paramsModel,
            params string[] ignoreProptsForParamModel)
        {
            return configExecutor.Scalar(
                EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), null);
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static object ScalarUseModel(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyCollection<ISqlConfigPolicy> policies)
        {
            return configExecutor.Scalar(EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), ToPolicies(policies));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static object ScalarUseModel(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies)
        {
            return configExecutor.Scalar(EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), policies);
        }

        #endregion

        #region NonQuery
        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="clearPolicy">缓存清理策略</param>
        /// <returns></returns>
        public static int NonQueryUseDict(this ISqlConfigExecutor configExecutor, 
            IReadOnlyDictionary<string, object> parameters,
            SqlClearCachePolicy clearPolicy = null)
        {
            return configExecutor.NonQuery(EFHelper.Services.SqlParamConverter.DictionaryToDBParams(configExecutor.DB, parameters),
                ToPolicies(clearPolicy));
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static int NonQueryUseDict(this ISqlConfigExecutor configExecutor,
            IReadOnlyDictionary<string, object> parameters,
            IReadOnlyCollection<ISqlConfigPolicy> policies)
        {
            return configExecutor.NonQuery(EFHelper.Services.SqlParamConverter.DictionaryToDBParams(configExecutor.DB, parameters),
                ToPolicies(policies));
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static int NonQueryUseDict(this ISqlConfigExecutor configExecutor,
            IReadOnlyDictionary<string, object> parameters,
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies)
        {
            return configExecutor.NonQuery(EFHelper.Services.SqlParamConverter.DictionaryToDBParams(configExecutor.DB, parameters),
                policies);
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="clearPolicy">缓存清理策略</param>
        /// <returns></returns>
        public static int NonQueryUseModel(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel,
            SqlClearCachePolicy clearPolicy = null)
        {
            return configExecutor.NonQuery(EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), ToPolicies(clearPolicy));
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static int NonQueryUseModel(this ISqlConfigExecutor configExecutor, object paramsModel,
            params string[] ignoreProptsForParamModel)
        {
            return configExecutor.NonQuery(EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), null);
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static int NonQueryUseModel(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyCollection<ISqlConfigPolicy> policies)
        {
            return configExecutor.NonQuery(EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), ToPolicies(policies));
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="policies">策略</param>
        /// <returns></returns>
        public static int NonQueryUseModel(this ISqlConfigExecutor configExecutor, object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel,
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies)
        {
            return configExecutor.NonQuery(EFHelper.Services.SqlParamConverter.ObjectToDBParams(configExecutor.DB, paramsModel,
                ignoreProptsForParamModel), policies);
        }

        #endregion

    }
}
