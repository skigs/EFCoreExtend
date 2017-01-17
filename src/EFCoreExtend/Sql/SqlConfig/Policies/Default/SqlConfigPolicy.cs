using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// SqlConfig策略
    /// </summary>
    //[SqlConfigPolicy(nameof(SqlConfigPolicy))]
    public abstract class SqlConfigPolicy : ISqlConfigPolicy
    {
        /// <summary>
        /// 是否使用策略(null/true为使用)
        /// </summary>
        public bool? IsUse { get; set; }
    }
}
