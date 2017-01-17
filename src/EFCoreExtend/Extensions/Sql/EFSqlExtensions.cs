using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class SqlExtensions
    {

        #region query
        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <returns></returns>
        public static IReadOnlyList<T> Query<T>(this DbContext db, string sql,
            IDataParameter[] parameters = null, IReadOnlyCollection<string> ignoreProptsForRtnType = null)
            where T : new()
        {
            return EFHelper.Services.SqlExecutor.Query<T>(db, sql, parameters, ignoreProptsForRtnType);
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">sql的参数</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseDict<T>(this DbContext db, string sql, 
            IReadOnlyDictionary<string, object> parameters,
            IReadOnlyCollection<string> ignoreProptsForRtnType = null)
            where T : new()
        {
            return EFHelper.Services.SqlExecutor.QueryUseDict<T>(db, sql, parameters, ignoreProptsForRtnType);
        }

        /// <summary>
        /// SqlQuery
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <param name="ignoreProptsForRtnType">数据类型需要忽略的属性名</param>
        /// <returns></returns>
        public static IReadOnlyList<T> QueryUseModel<T>(this DbContext db, string sql,
            object paramsModel, 
            IReadOnlyCollection<string> ignoreProptsForParamModel = null, 
            IReadOnlyCollection<string> ignoreProptsForRtnType = null)
            where T : new()
        {
            return EFHelper.Services.SqlExecutor.QueryUseModel<T>(db, sql, paramsModel, 
                ignoreProptsForParamModel, ignoreProptsForRtnType);
        }
        #endregion

        #region scalar
        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static object Scalar(this DbContext db, string sql, params IDataParameter[] parameters)
        {
            return EFHelper.Services.SqlExecutor.Scalar(db, sql, parameters);
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static object ScalarUseDict(this DbContext db, string sql, 
            IReadOnlyDictionary<string, object> parameters)
        {
            return EFHelper.Services.SqlExecutor.ScalarUseDict(db, sql, parameters);
        }

        /// <summary>
        /// SqlScalar
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static object ScalarUseModel(this DbContext db, string sql, object paramsModel, 
            IReadOnlyCollection<string> ignoreProptsForParamModel = null)
        {
            return EFHelper.Services.SqlExecutor.ScalarUseModel(db, sql, paramsModel, ignoreProptsForParamModel);
        }
        #endregion

        #region nonquery
        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static int NonQuery(this DbContext db, string sql, params IDataParameter[] parameters)
        {
            return EFHelper.Services.SqlExecutor.NonQuery(db, sql, parameters);
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">sql的参数</param>
        /// <returns></returns>
        public static int NonQueryUseDict(this DbContext db, string sql, 
            IReadOnlyDictionary<string, object> parameters)
        {
            return EFHelper.Services.SqlExecutor.NonQueryUseDict(db, sql, parameters);
        }

        /// <summary>
        /// SqlNonQuery
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptsForParamModel">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static int NonQueryUseModel(this DbContext db, string sql, object paramsModel, 
            IReadOnlyCollection<string> ignoreProptsForParamModel = null)
        {
            return EFHelper.Services.SqlExecutor.NonQueryUseModel(db, sql, paramsModel, ignoreProptsForParamModel);
        }
        #endregion

    }
}
