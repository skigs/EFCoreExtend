using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// 对sql的参数SqlParameter为Model(属性类)对象的参数进行遍历策略 的执行器
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlForeachModelPolicyName)]
    public class SqlForeachModelPolicy : SqlParamObjEachPolicyBase<SqlForeachModelPolicyInfo>
    {
    }

    public class SqlForeachModelPolicyInfo : KVPairForeachPolicyInfoBase
    {

    }

}
