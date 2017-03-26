using EFCoreExtend.Sql.SqlConfig.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies
{
    /// <summary>
    /// lua配置策略管理器
    /// </summary>
    public interface ILuaSqlPolicyManager
    {

        /// <summary>
        /// 策略类型集合
        /// </summary>
        IDictionary<string, Type> PolicyTypes { get; }

        /// <summary>
        /// 用于保存全局的策略对象
        /// </summary>
        IDictionary<string, ISqlConfigPolicy> GlobalPolicies { get; }

        /// <summary>
        /// 设置策略执行器
        /// </summary>
        /// <param name="policyName">策略名称</param>
        /// <param name="getExecutorFunc">获取执行器对象的Func</param>
        /// <param name="executorBaseType">执行器基类类型</param>
        /// <param name="priority">策略执行器的执行优先级（值越大越先执行）</param>
        void SetExecutor<T>(string policyName, Func<T> getExecutorFunc, int priority = 0);

        /// <summary>
        /// 执行相关的策略执行器
        /// </summary>
        /// <param name="policies">通过形参传递的策略对象集合</param>
        /// <param name="info">策略执行器相关信息</param>
        /// <param name="Type">策略执行器基类(接口)类型</param>
        /// <returns></returns>
        void InvokeExecutors(IDictionary<string, ISqlConfigPolicy> policies,
            ILuaSqlPolicyExecutorInfo info, Type executorInfoBaseType);

    }
}
