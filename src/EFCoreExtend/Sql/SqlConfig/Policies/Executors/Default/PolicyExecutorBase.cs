using EFCoreExtend.Sql.SqlConfig.Policies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    public abstract class PolicyExecutorBase
    {
        public bool IsUsePolicy(ISqlConfigPolicy policy)
        {
            return policy != null && (policy.IsUse == null || policy.IsUse.Value);
        }
    }
}
