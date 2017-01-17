using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    [SqlConfigPolicy(SqlConfigConst.TableNamePolicyName)]
    public class TableNamePolicyExecutor : PolicyExecutorBase, ISqlInitPolicyExecutor
    {
        public void Execute(ISqlInitPolicyExecutorInfo info)
        {
            TableNamePolicy policy;
            //进行替换表名
            foreach (var table in info.TableSqlInfos.Values)
            {
                var tableSqls = info.NewlySqls[table.Name]; //获取最新的sql
                foreach (var sqlPair in table.Sqls)
                {
                    var sql = tableSqls[sqlPair.Key];
                    policy = info.GetPolicy(sqlPair.Value, table) as TableNamePolicy;
                    if (IsUsePolicy(policy))
                    {
                        var tname = string.IsNullOrEmpty(policy.Tag) ? SqlConfigConst.TableNameLabel : policy.Tag;
                        //替换sql
                        tableSqls[sqlPair.Key] = sql.Replace(tname, policy.Prefix + table.Name + policy.Suffix);
                    }
                }
            }
        }
    }
}
