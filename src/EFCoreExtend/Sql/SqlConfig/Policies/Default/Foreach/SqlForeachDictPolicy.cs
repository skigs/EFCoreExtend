using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// 对sql的参数SqlParameter为Dictionary类型的参数进行遍历策略
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlForeachDictPolicyName)]
    public class SqlForeachDictPolicy : SqlParamObjEachPolicyBase<SqlForeachDictPolicyInfo>
    {
        /// <summary>
        /// 是否所有Dictionary类型类型的都进行遍历
        /// </summary>
        public bool IsAll { get; set; }
    }

    public class SqlForeachDictPolicyInfo : KVPairForeachPolicyInfoBase
    {

    }

}
