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
    /// 分部sql策略
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlSectionPolicyName)]
    public class SqlSectionPolicy : SqlConfigPolicy
    {

        /// <summary>
        /// 策略前缀标记符，默认为 #{
        /// </summary>
        [DefaultValue(SqlConfigConst.SqlSectionPrefixSymbol)]
        public string TagPrefix { get; set; }

        /// <summary>
        /// 策略后缀标记符，默认为 }
        /// </summary>
        [DefaultValue(SqlConfigConst.SqlSectionSuffixSymbol)]
        public string TagSuffix { get; set; }

        /// <summary>
        /// 指定sql的名称(同表下的SqlName)
        /// </summary>
        public IReadOnlyList<string> SqlNames { get; set; }

        /// <summary>
        /// 指定其他表的sql名称(key为TableName，value为SqlName)
        /// </summary>
        public IReadOnlyDictionary<string, string> TableSqlNames { get; set; }

    }
}
