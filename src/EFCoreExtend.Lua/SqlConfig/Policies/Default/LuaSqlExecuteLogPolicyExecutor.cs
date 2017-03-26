using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.Default
{
    /// <summary>
    /// 用于SqlConfigExecutor执行sql前记录所生成sql
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlConfigExecuteLogPolicyName)]
    public class LuaSqlExecuteLogPolicyExecutor : PolicyExecutorBase, ILuaSqlExecutePolicyExecutor
    {
        Action<string, string, string, IReadOnlyList<IDataParameter>> _doLog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doLog">Action中的参数一：TableName；参数二：SqlName；参数三：Sql；参数四：SqlParameters</param>
        public LuaSqlExecuteLogPolicyExecutor(Action<string, string,
            string, IReadOnlyList<IDataParameter>> doLog)
        {
            _doLog = doLog;
        }

        public void Execute(ILuaSqlExecutePolicyExecutorInfo info)
        {
            var policy = info.GetPolicy() as SqlConfigExecuteLogPolicy;
            if (IsUsePolicy(policy) && _doLog != null)
            {
                if (policy.IsAsync)
                {
                    Task.Run(() =>
                    {
                        _doLog.Invoke(info.TableName, info.SqlName, info.Sql, info.SqlParams);
                    });
                }
                else
                {
                    _doLog.Invoke(info.TableName, info.SqlName, info.Sql, info.SqlParams);
                }
            }
        }

    }
}
