using EFCoreExtend.Sql.SqlConfig.Policies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig
{
    /// <summary>
    /// lua配置sql执行器
    /// </summary>
    public interface ILuaSqlConfigExecutor
    {
        DbContext DB { get; }
        string TableName { get; }
        string SqlName { get; }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="ignoreProptsForRtnType"></param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        IReadOnlyList<T> QueryUseDict<T>(IDictionary<string, object> parameters,
            IEnumerable<string> ignoreProptsForRtnType = null, 
            IDictionary<string, ISqlConfigPolicy> policies = null) where T : new();

        /// <summary>
        /// Scalar
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        object ScalarUseDict(IDictionary<string, object> parameters,
            IDictionary<string, ISqlConfigPolicy> policies = null);

        /// <summary>
        /// 非查询
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="policies">策略对象集合</param>
        /// <returns></returns>
        int NonQueryUseDict(IDictionary<string, object> parameters,
            IDictionary<string, ISqlConfigPolicy> policies = null);

    }
}
