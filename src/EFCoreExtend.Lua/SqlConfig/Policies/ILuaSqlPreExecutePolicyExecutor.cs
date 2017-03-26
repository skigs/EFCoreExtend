using EFCoreExtend.Sql.SqlConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies
{
    /// <summary>
    /// 用于在lua sql执行前的策略执行器（例如：生成相关的lua函数的参数）
    /// </summary>
    public interface ILuaSqlPreExecutePolicyExecutor : ILuaSqlPolicyExecutor<ILuaSqlPreExecutePolicyExecutorInfo>
    {
    }

    /// <summary>
    /// lua sql执行前的策略执行器相关信息
    /// </summary>
    public interface ILuaSqlPreExecutePolicyExecutorInfo : ILuaSqlPolicyExecutorInfoBase
    {
        /// <summary>
        /// 当前执行的类型
        /// </summary>
        ConfigSqlExecuteType ExecuteType { get; }

        /// <summary>
        /// sql所在配置的信息
        /// </summary>
        ILuaConfigSqlInfo SqlInfo { get; }

        /// <summary>
        /// sql所在配置的表信息
        /// </summary>
        ILuaConfigTableInfo TableInfo { get; }

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
        /// 即将执行的sql参数，可修改
        /// </summary>
        IDictionary<string, object> PreSqlParams { get; set; }

        /// <summary>
        /// lua SqlParameters操作相关的函数
        /// </summary>
        IDictionary<string, object> LuaSqlParamFuncs { get; set; }

        /// <summary>
        /// lua脚本运行之后触发的事件
        /// </summary>
        Action LuaRan { get; set; }

        /// <summary>
        /// 获取策略对象（策略对象可能 通过方法传递的policy / 全局的policy / SqlInfo和TableInfo配置的policy中获取，如果这些地方都了相同类型的policy
        /// 那么获取策略对象优先级是：通过方法传递的policy > SqlInfo配置的 > TableInfo配置的 > GlobalPolicy）
        /// </summary>
        /// <returns></returns>
        object GetPolicy();

    }

}
