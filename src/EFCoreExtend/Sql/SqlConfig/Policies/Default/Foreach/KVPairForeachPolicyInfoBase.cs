using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    public abstract class KVPairForeachPolicyInfoBase : SqlParamObjEachPolicyInfoBase
    {
        /// <summary>
        /// key的前缀
        /// </summary>
        public string KPrefix { get; set; }

        /// <summary>
        /// key的后缀
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

        /// <summary>
        /// 不需要进行操作的key
        /// </summary>
        public IReadOnlyList<string> IgnoreKeys { get; set; }

    }
}
