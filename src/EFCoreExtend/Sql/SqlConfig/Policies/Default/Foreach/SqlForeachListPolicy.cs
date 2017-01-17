using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// 对sql的参数SqlParameter为List类型的参数进行遍历策略
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlForeachListPolicyName)]
    public class SqlForeachListPolicy : SqlParamObjEachPolicyBase<SqlForeachListPolicyInfo>
    {
        /// <summary>
        /// 是否所有List类型的都进行遍历
        /// </summary>
        public bool IsAll { get; set; }
    }

    public class SqlForeachListPolicyInfo : SqlParamObjEachPolicyInfoBase
    {

    }

}
