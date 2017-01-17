using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    /// <summary>
    /// 策略扩展测试：Insert Into策略执行器
    /// </summary>
    [SqlConfigPolicy("insinto")]
    public class InsertIntoPolicyExecutor : PolicyExecutorBase, ISqlInitPolicyExecutor
    {
        public void Execute(ISqlInitPolicyExecutorInfo info)
        {
            InsertIntoPolicy policy;
            foreach (var table in info.TableSqlInfos.Values)
            {
                var tableSqls = info.NewlySqls[table.Name]; //获取最新的sql
                foreach (var sqlPair in table.Sqls)
                {
                    var sql = tableSqls[sqlPair.Key];
                    policy = info.GetPolicy(sqlPair.Value, table) as InsertIntoPolicy;
                    if (IsUsePolicy(policy))
                    {
                        var mdf = sqlPair.Value as IConfigSqlInfoModifier;  //获取sql的配置修改器
                        if (mdf != null)
                        {
                            //添加$$params的策略
                            mdf.Policies[SqlConfigConst.SqlForeachParamsPolicyName] = new SqlForeachParamsPolicy
                            {
                                IsKVSplit = true,
                                IsToSqlParam = true,
                                KSeparator = ",",
                                VSeparator = ",",
                            };
                            var tname = string.IsNullOrEmpty(policy.Tag) ? "##insinto" : policy.Tag;
                            //替换sql
                            tableSqls[sqlPair.Key] = sql.Replace(tname,
                                "insert into " + table.Name + "($$params.keys) values($$params.vals)");
                        }
                        else
                        {
                            throw new ArgumentException($"The table [{table.Name}] can not as IConfigTableInfoModifier");
                        }
                    }
                }
            }
        }
    }
}
