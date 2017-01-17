using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// 清理二级查询缓存策略（用于非查询(NonQuery)中）
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlClearCachePolicyName)]
    public class SqlClearCachePolicy : SqlConfigPolicy
    {

        /// <summary>
        /// 是否异步清理缓存
        /// </summary>
        //[DefaultValue(true)]
        public bool IsAsync { get; set; }

        /// <summary>
        /// 是否清理 所在表下 的所有缓存
        /// </summary>
        public bool IsSelfAll { get; set; }

        /// <summary>
        /// 需要进行缓存清理的表的名称（一般用于清理 其他表下 的所有查询缓存）
        /// </summary>
        public IReadOnlyList<string> Tables { get; set; }

        /// <summary>
        /// 需要进行缓存清理的类型（用于清理 所在表下 的CacheType查询缓存）
        /// </summary>
        public IReadOnlyList<string> CacheTypes { get; set; }

        /// <summary>
        /// 需要进行缓存清理的类型(key为TableName，value为CacheType，一般用于清理 其他表下 的CacheType)
        /// </summary>
        public IReadOnlyDictionary<string, string> TableCacheTypes { get; set; }

    }
}
