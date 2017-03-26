using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;

namespace EFCoreExtend.Sql
{
    /// <summary>
    /// sql执行器
    /// </summary>
    public interface ISqlExecutor
    {

        /// <summary>
        /// 执行sql的查询语句
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ignoreProptsForRtnType">需要忽略的返回值类型的属性</param>
        /// <returns></returns>
        IReadOnlyList<T> Query<T>(DbContext db, string sql, IDataParameter[] parameters = null, 
            IEnumerable<string> ignoreProptsForRtnType = null)
            where T : new();

        /// <summary>
        /// 执行sql的Scalar查询语句
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object Scalar(DbContext db, string sql, params IDataParameter[] parameters);

        /// <summary>
        /// 执行sql的非查询语句
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int NonQuery(DbContext db, string sql, params IDataParameter[] parameters);

    }
}
