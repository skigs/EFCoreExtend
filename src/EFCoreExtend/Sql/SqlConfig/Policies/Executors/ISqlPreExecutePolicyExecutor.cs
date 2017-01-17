using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors
{
    /// <summary>
    /// 用于在sql执行前的策略执行器（例如：foreach执行器对某些数据类型（list/dict等等）进行生成字串替换到sql中）
    /// </summary>
    public interface ISqlPreExecutePolicyExecutor
    {
        void Execute(ISqlPreExecutePolicyExecutorInfo info);
    }

    public interface ISqlPreExecutePolicyExecutorInfo : IPolicyExecutorInfoBase
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
        /// 即将执行的sql，可修改
        /// </summary>
        string Sql { get; set; }
        /// <summary>
        /// 即将执行的sql参数，可修改
        /// </summary>
        IDataParameter[] SqlParams { get; set; }

        /// <summary>
        /// 获取策略对象（策略对象可能 通过方法传递的policy / 全局的policy / SqlInfo和TableInfo配置的policy中获取，如果这些地方都了相同类型的policy
        /// 那么获取策略对象优先级是：通过方法传递的policy > SqlInfo配置的 > TableInfo配置的 > GlobalPolicy）
        /// </summary>
        /// <returns></returns>
        object GetPolicy();
    }

}
