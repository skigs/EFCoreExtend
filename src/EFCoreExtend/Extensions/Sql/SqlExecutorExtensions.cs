using EFCoreExtend.Sql;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class SqlExecutorExtensions
    {
        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="executor"></param>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseDict<T>(this ISqlExecutor executor, DbContext db, string sql, 
            IReadOnlyDictionary<string, object> parameters, 
            IReadOnlyCollection<string> ignoreProptsForRtnType = null)
            where T : new()
        {
            return executor.Query<T>(db, sql, EFHelper.Services.SqlParamConverter.DictionaryToDBParams(db, parameters), ignoreProptsForRtnType);
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="executor"></param>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseModel<T>(this ISqlExecutor executor, DbContext db, string sql, 
            object paramsModel, 
            IReadOnlyCollection<string> ignoreProptsForParamModel, 
            IReadOnlyCollection<string> ignoreProptsForRtnType)
            where T : new()
        {
            return executor.Query<T>(db, sql, 
                EFHelper.Services.SqlParamConverter.ObjectToDBParams(db, paramsModel, ignoreProptsForParamModel), 
                ignoreProptsForRtnType);
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="executor"></param>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseModel<T>(this ISqlExecutor executor, DbContext db, string sql,
            object paramsModel,
            IReadOnlyCollection<string> ignoreProptsForParamModel = null,
            params string[] ignoreProptsForRtnType)
            where T : new()
        {
            return executor.Query<T>(db, sql, 
                EFHelper.Services.SqlParamConverter.ObjectToDBParams(db, paramsModel, ignoreProptsForParamModel), 
                ignoreProptsForRtnType);
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static object ScalarUseDict(this ISqlExecutor executor, DbContext db, string sql, 
            IReadOnlyDictionary<string, object> parameters)
        {
            return executor.Scalar(db, sql, EFHelper.Services.SqlParamConverter.DictionaryToDBParams(db, parameters));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static object ScalarUseModel(this ISqlExecutor executor, DbContext db, string sql, 
            object paramsModel, 
            IReadOnlyCollection<string> ignoreProptsForParamModel)
        {
            return executor.Scalar(db, sql, EFHelper.Services.SqlParamConverter.ObjectToDBParams(db, paramsModel, ignoreProptsForParamModel));
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static object ScalarUseModel(this ISqlExecutor executor, DbContext db, string sql,
            object paramsModel,
            params string[] ignoreProptsForParamModel)
        {
            return executor.Scalar(db, sql, 
                EFHelper.Services.SqlParamConverter.ObjectToDBParams(db, paramsModel, ignoreProptsForParamModel));
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static int NonQueryUseDict(this ISqlExecutor executor, DbContext db, string sql, 
            IReadOnlyDictionary<string, object> parameters)
        {
            return executor.NonQuery(db, sql, EFHelper.Services.SqlParamConverter.DictionaryToDBParams(db, parameters));
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static int NonQueryUseModel(this ISqlExecutor executor, DbContext db, string sql, 
            object paramsModel, 
            IReadOnlyCollection<string> ignoreProptsForParamModel)
        {
            return executor.NonQuery(db, sql, EFHelper.Services.SqlParamConverter.ObjectToDBParams(db, paramsModel, ignoreProptsForParamModel));
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static int NonQueryUseModel(this ISqlExecutor executor, DbContext db, string sql,
            object paramsModel,
            params string[] ignoreProptsForParamModel)
        {
            return executor.NonQuery(db, sql, EFHelper.Services.SqlParamConverter.ObjectToDBParams(db, paramsModel, ignoreProptsForParamModel));
        }

    }
}
