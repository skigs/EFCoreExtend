using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors
{
    /// <summary>
    /// 用于sql执行时的策略执行器（例如：查询缓存（一级/二级的），查询缓存清理（NonQuery的执行之后对二级缓存的清理）等等）
    /// </summary>
    public interface ISqlExecutePolicyExecutor
    {
        void Execute(ISqlExecutePolicyExecutorInfo info);
    }

    public interface ISqlExecutePolicyExecutorInfo : IPolicyExecutorInfoBase
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        string PolicyName { get; }
        /// <summary>
        /// 当前执行的类型
        /// </summary>
        ConfigSqlExecuteType ExecuteType { get; }
        /// <summary>
        /// sql所在配置的信息
        /// </summary>
        IConfigSqlInfo SqlInfo { get; }
        /// <summary>
        /// sql所在配置的表信息
        /// </summary>
        IConfigTableInfo TableInfo { get; }
        /// <summary>
        /// DB上下文
        /// </summary>
        DbContext DB { get; }
        /// <summary>
        /// sql配置的表名
        /// </summary>
        string TableName { get; }
        /// <summary>
        /// sql配置的名称
        /// </summary>
        string SqlName { get; }
        /// <summary>
        /// 执行的sql
        /// </summary>
        string Sql { get; }
        /// <summary>
        /// 执行sql的参数
        /// </summary>
        IReadOnlyList<IDataParameter> SqlParams { get; }
        /// <summary>
        /// sql到数据库中执行的执行器
        /// </summary>
        Func<object> ToDBExecutor { get; }
        /// <summary>
        /// sql执行的返回值，可修改（这些数据可以是缓存数据，也可以是数据库中返回的结果）
        /// </summary>
        object ReturnValue { get; set; }
        /// <summary>
        /// 是否结束执行(例如缓存获取成功之后，不需要再进行执行sql了，那么设置为true，否则不要设置，不然sql无法执行了)，可修改
        /// </summary>
        bool IsEnd { get; set; }

        /// <summary>
        /// 作用于SqlExecutor对象的临时数据（例如在一级缓存中缓存数据）
        /// </summary>
        IDictionary<string, object> SqlExecutorTempDatas { get; }

        /// <summary>
        /// 获取策略对象（策略对象可能 通过方法传递的policy / 全局的policy / SqlInfo和TableInfo配置的policy中获取，如果这些地方都了相同类型的policy
        /// 那么获取策略对象优先级是：通过方法传递的policy > SqlInfo配置的 > TableInfo配置的 > GlobalPolicy）
        /// </summary>
        /// <returns></returns>
        object GetPolicy();
    }

}
