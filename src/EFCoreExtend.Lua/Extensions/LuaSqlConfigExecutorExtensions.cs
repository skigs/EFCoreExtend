using EFCoreExtend.Commons;
using EFCoreExtend.Lua.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Policies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class LuaSqlConfigExecutorExtensions
    {

        #region Query

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <returns></returns>
        public static IReadOnlyList<T> Query<T>(this ILuaSqlConfigExecutor configExecutor,
            IDataParameter[] parameters = null,
            params string[] ignoreProptsForRtnType)
            where T : new()
        {
            if (parameters?.Length > 0)
            {
                return configExecutor.QueryUseDict<T>(parameters.ToDictionary(l => l.ParameterName, l => l.Value),
                    ignoreProptsForRtnType);
            }
            else
            {
                return configExecutor.QueryUseDict<T>(null, ignoreProptsForRtnType);
            }
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        public static IReadOnlyList<T> Query<T>(this ILuaSqlConfigExecutor configExecutor,
            IDataParameter[] parameters,
            IEnumerable<string> ignoreProptsForRtnType,
            IDictionary<string, ISqlConfigPolicy> policies)
            where T : new()
        {
            if (parameters?.Length > 0)
            {
                return configExecutor.QueryUseDict<T>(parameters.ToDictionary(l => l.ParameterName, l => l.Value),
                    ignoreProptsForRtnType, policies);
            }
            else
            {
                return configExecutor.QueryUseDict<T>(null, ignoreProptsForRtnType, policies);
            }
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
        public static IReadOnlyList<T> QueryUseModel<T>(this ILuaSqlConfigExecutor configExecutor, object paramsModel,
            IEnumerable<string> ignoreProptsForParamModel = null,
            params string[] ignoreProptsForRtnType)
            where T : new()
        {
            return configExecutor.QueryUseDict<T>(
                EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(paramsModel, ignoreProptsForParamModel), 
                ignoreProptsForRtnType);
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseModel<T>(this ILuaSqlConfigExecutor configExecutor, object paramsModel,
            IEnumerable<string> ignoreProptsForParamModel,
            IEnumerable<string> ignoreProptsForRtnType,
            IDictionary<string, ISqlConfigPolicy> policies)
            where T : new()
        {
            return configExecutor.QueryUseDict<T>(
                EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(paramsModel, ignoreProptsForParamModel),
                ignoreProptsForRtnType, policies);
        }

        #endregion

        #region Scalar

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static object Scalar(this ILuaSqlConfigExecutor configExecutor,
            params IDataParameter[] parameters)
        {
            if (parameters?.Length > 0)
            {
                return configExecutor.ScalarUseDict(parameters.ToDictionary(l => l.ParameterName, l => l.Value));
            }
            else
            {
                return configExecutor.ScalarUseDict(null);
            }
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        public static object Scalar(this ILuaSqlConfigExecutor configExecutor,
            IDataParameter[] parameters,
            IDictionary<string, ISqlConfigPolicy> policies)
        {
            if (parameters?.Length > 0)
            {
                return configExecutor.ScalarUseDict(parameters.ToDictionary(l => l.ParameterName, l => l.Value), policies);
            }
            else
            {
                return configExecutor.ScalarUseDict(null, policies);
            }
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static T Scalar<T>(this ILuaSqlConfigExecutor configExecutor,
            params IDataParameter[] parameters)
            where T : struct
        {
            if (parameters?.Length > 0)
            {
                return (T)typeof(T).ChangeValueType(
                    configExecutor.ScalarUseDict(parameters.ToDictionary(l => l.ParameterName, l => l.Value)));
            }
            else
            {
                return (T)typeof(T).ChangeValueType(configExecutor.ScalarUseDict(null));
            }
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        public static T Scalar<T>(this ILuaSqlConfigExecutor configExecutor,
            IDataParameter[] parameters,
            IDictionary<string, ISqlConfigPolicy> policies)
            where T : struct
        {
            if (parameters?.Length > 0)
            {
                return (T)typeof(T).ChangeValueType(
                    configExecutor.ScalarUseDict(parameters.ToDictionary(l => l.ParameterName, l => l.Value), policies));
            }
            else
            {
                return (T)typeof(T).ChangeValueType(configExecutor.ScalarUseDict(null, policies));
            }
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static object ScalarUseModel(this ILuaSqlConfigExecutor configExecutor, object paramsModel,
            params string[] ignoreProptsForParamModel)
        {
            return configExecutor.ScalarUseDict(
                EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(paramsModel, ignoreProptsForParamModel));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static T ScalarUseModel<T>(this ILuaSqlConfigExecutor configExecutor, object paramsModel,
            params string[] ignoreProptsForParamModel)
            where T : struct
        {
            return (T)typeof(T).ChangeValueType(
                    configExecutor.ScalarUseDict(
                    EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(paramsModel, 
                    ignoreProptsForParamModel)));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        public static object ScalarUseModel(this ILuaSqlConfigExecutor configExecutor, object paramsModel,
            IEnumerable<string> ignoreProptsForParamModel,
            IDictionary<string, ISqlConfigPolicy> policies)
        {
            return configExecutor.ScalarUseDict(
                EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(paramsModel, 
                ignoreProptsForParamModel), policies);
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        public static T ScalarUseModel<T>(this ILuaSqlConfigExecutor configExecutor, object paramsModel,
            IEnumerable<string> ignoreProptsForParamModel,
            IDictionary<string, ISqlConfigPolicy> policies)
            where T : struct
        {
            return (T)typeof(T).ChangeValueType(
                    configExecutor.ScalarUseDict(
                    EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(paramsModel,
                    ignoreProptsForParamModel), policies));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static T ScalarUseDict<T>(this ILuaSqlConfigExecutor configExecutor,
            IDictionary<string, object> parameters)
            where T : struct
        {
            return (T)typeof(T).ChangeValueType(configExecutor.ScalarUseDict(parameters));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        public static T ScalarUseDict<T>(this ILuaSqlConfigExecutor configExecutor,
            IDictionary<string, object> parameters,
            IDictionary<string, ISqlConfigPolicy> policies)
            where T : struct
        {
            return (T)typeof(T).ChangeValueType(configExecutor.ScalarUseDict(parameters, policies));
        }

        #endregion

        #region NonQuery

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static int NonQuery(this ILuaSqlConfigExecutor configExecutor,
            params IDataParameter[] parameters)
        {
            if (parameters?.Length > 0)
            {
                return configExecutor.NonQueryUseDict(parameters.ToDictionary(l => l.ParameterName, l => l.Value));
            }
            else
            {
                return configExecutor.NonQueryUseDict(null);
            }
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        public static int NonQuery(this ILuaSqlConfigExecutor configExecutor,
            IDataParameter[] parameters,
            IDictionary<string, ISqlConfigPolicy> policies)
        {
            if (parameters?.Length > 0)
            {
                return configExecutor.NonQueryUseDict(parameters.ToDictionary(l => l.ParameterName, l => l.Value), policies);
            }
            else
            {
                return configExecutor.NonQueryUseDict(null, policies);
            }
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static int NonQueryUseModel(this ILuaSqlConfigExecutor configExecutor, object paramsModel,
            params string[] ignoreProptsForParamModel)
        {
            return configExecutor.NonQueryUseDict(
                EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(paramsModel, 
                ignoreProptsForParamModel));
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="configExecutor"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        public static int NonQueryUseModel(this ILuaSqlConfigExecutor configExecutor, object paramsModel,
            IEnumerable<string> ignoreProptsForParamModel,
            IDictionary<string, ISqlConfigPolicy> policies)
        {
            return configExecutor.NonQueryUseDict(
                EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(paramsModel,
                ignoreProptsForParamModel), policies);
        }

        #endregion

    }
}
