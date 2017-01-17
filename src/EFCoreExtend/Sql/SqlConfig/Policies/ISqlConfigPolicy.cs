using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies
{
    /// <summary>
    /// SqlConfig策略
    /// </summary>
    public interface ISqlConfigPolicy
    {
        /// <summary>
        /// 是否使用策略(null/true为使用)
        /// </summary>
        bool? IsUse { get; }
    }
}
