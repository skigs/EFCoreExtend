using EFCoreExtend.EFCache;
using EFCoreExtend.EFCache.Default;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// 二级查询缓存策略
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlL2QueryCachePolicyName)]
    public class SqlL2QueryCachePolicy : SqlConfigPolicy
    {

        /// <summary>
        /// 缓存的类型(Query默认为：query, Scalar默认为：scalar)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 查询缓存过期策略
        /// </summary>
        public QueryCacheExpiryPolicy Expiry { get; set; }

    }
}
