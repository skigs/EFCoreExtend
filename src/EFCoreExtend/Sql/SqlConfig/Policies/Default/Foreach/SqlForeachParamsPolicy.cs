using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// 对SqlParameter的数据进行遍历的策略
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlForeachParamsPolicyName)]
    public class SqlForeachParamsPolicy : SqlConfigPolicy
    {
        /// <summary>
        /// 标记符，默认为 $$params
        /// </summary>
        [DefaultValue(SqlConfigConst.SqlForeachParamsLabel)]
        public string Tag { get; set; }

        /// <summary>
        /// 是否将遍历获取到的值(value)转换成SqlParameter，默认为true
        /// </summary>
        [DefaultValue(true)]
        public bool IsToSqlParam { get; set; }

        /// <summary>
        /// 不需要进行遍历的SqlParameter
        /// </summary>
        public IReadOnlyList<string> IgnoreParams { get; set; }

        /// <summary>
        /// SqlParameter Value的前缀
        /// </summary>
        public string VPrefix { get; set; }

        /// <summary>
        /// SqlParameter Value的后缀
        /// </summary>
        public string VSuffix { get; set; }

        /// <summary>
        /// SqlParameter与SqlParameter之间的分隔符
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// SqlParameter Name（key）的前缀
        /// </summary>
        public string KPrefix { get; set; }

        /// <summary>
        /// SqlParameter Name（key）的后缀
        /// </summary>
        public string KSuffix { get; set; }

        /// <summary>
        /// key-value之间的分隔符
        /// </summary>
        public string KVSeparator { get; set; }

        /// <summary>
        /// 是否key-value分开(keys 和 values在不同地方单独生成字串)
        /// </summary>
        public bool IsKVSplit { get; set; }

        /// <summary>
        /// 使用iskvSplit的时候，key-key之间的分隔符
        /// </summary>
        public string KSeparator { get; set; }

        /// <summary>
        /// 使用iskvSplit的时候，value-value之间的分隔符
        /// </summary>
        public string VSeparator { get; set; }

    }
}
