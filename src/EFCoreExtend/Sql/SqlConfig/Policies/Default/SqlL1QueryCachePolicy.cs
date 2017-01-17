using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// 一级查询缓存策略
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlL1QueryCachePolicyName)]
    public class SqlL1QueryCachePolicy : SqlConfigPolicy
    {
    }
}
