using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    /// <summary>
    /// 用于SqlConfigExecutor执行sql前记录所生成sql
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlConfigExecuteLogPolicyName)]
    public class SqlConfigExecuteLogPolicyExecutor : PolicyExecutorBase, ISqlExecutePolicyExecutor
    {
        Action<string, string, string, IReadOnlyList<IDataParameter>> _doLog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doLog">Action中的参数一：TableName；参数二：SqlName；参数三：Sql；参数四：SqlParameters</param>
        public SqlConfigExecuteLogPolicyExecutor(Action<string, string,
            string, IReadOnlyList<IDataParameter>> doLog)
        {
            _doLog = doLog;
        }

        public void Execute(ISqlExecutePolicyExecutorInfo info)
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
