using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// sql日志记录策略
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlConfigExecuteLogPolicyName)]
    public class SqlConfigExecuteLogPolicy : SqlConfigPolicy
    {
        /// <summary>
        /// 是否异步，默认为true
        /// </summary>
        [DefaultValue(true)]
        public bool IsAsync { get; set; }
    }
}
