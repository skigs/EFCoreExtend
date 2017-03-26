using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors
{
    /// <summary>
    /// sql策略执行器
    /// </summary>
    public interface ISqlPolicyExecutor<T> where T : IPolicyExecutorInfoBase
    {
        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="info">策略信息</param>
        void Execute(T info);
    }
}
