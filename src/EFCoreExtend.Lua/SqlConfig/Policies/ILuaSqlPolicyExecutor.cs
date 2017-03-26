using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies
{
    /// <summary>
    /// lua sql策略执行器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILuaSqlPolicyExecutor<T> where T : ILuaSqlPolicyExecutorInfoBase
    {
        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="info">策略信息</param>
        void Execute(T info);
    }

    /// <summary>
    /// 策略执行器信息
    /// </summary>
    public interface ILuaSqlPolicyExecutorInfoBase
    {
        /// <summary>
        /// 所有的表的策略配置信息
        /// </summary>
        ILuaSqlConfig Config { get; }

        /// <summary>
        /// 策略名称
        /// </summary>
        string PolicyName { get; }

        /// <summary>
        /// 全局的策略对象
        /// </summary>
        object GlobalPolicy { get; }

        /// <summary>
        /// 通过方法形参传递的策略对象
        /// </summary>
        object ParameterPolicy { get; }
    }

    public interface ILuaSqlPolicyExecutorInfo : ILuaSqlPolicyExecutorInfoBase
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        new string PolicyName { get; set; }

        /// <summary>
        /// 全局的策略对象
        /// </summary>
        new object GlobalPolicy { get; set; }

        /// <summary>
        /// 通过方法形参传递的策略对象
        /// </summary>
        new object ParameterPolicy { get; set; }
    }

}
