using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies
{
    /// <summary>
    /// 用于初始化的策略执行器，就是在程序运行期间只执行一次，除非sql的配置数据发生了改变
    /// </summary>
    public interface ILuaSqlInitPolicyExecutor : ILuaSqlPolicyExecutor<ILuaSqlInitPolicyExecutorInfo>
    {
    }

    /// <summary>
    /// 初始化的策略执行器相关信息
    /// </summary>
    public interface ILuaSqlInitPolicyExecutorInfo : ILuaSqlPolicyExecutorInfoBase
    {
        /// <summary>
        /// 用于存储SqlParameters在lua中处理的相关函数
        /// </summary>
        IDictionary<string, IDictionary<string, object>> LuaSqlParamFuncsContainer { get; }

        /// <summary>
        /// 获取策略对象（策略对象可能 通过方法传递的policy / 全局的policy / SqlInfo和TableInfo配置的policy中获取，如果这些地方都了相同类型的policy
        /// 那么获取策略对象优先级是：通过方法传递的policy > SqlInfo配置的 > TableInfo配置的 > GlobalPolicy）
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        object GetPolicy(ILuaConfigSqlInfo sqlInfo, ILuaConfigTableInfo tableInfo);
    }

}
