using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// 对sql的参数SqlParameter为Dictionary<string, object>(IEnumerable<KeyValuePair<string, object>>)类型的参数进行遍历策略
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlForeachDictPolicyName)]
    public class SqlForeachDictPolicy : SqlParamObjEachPolicyBase<SqlForeachDictPolicyInfo>
    {
        /// <summary>
        /// 是否所有Dictionary<string, object>(IEnumerable<KeyValuePair<string, object>>)类型类型的都进行遍历
        /// </summary>
        public bool IsAll { get; set; }
    }

    public class SqlForeachDictPolicyInfo : KVPairForeachPolicyInfoBase
    {

    }

}
