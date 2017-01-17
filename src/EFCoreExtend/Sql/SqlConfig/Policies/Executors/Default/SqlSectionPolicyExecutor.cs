using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    [SqlConfigPolicy(SqlConfigConst.SqlSectionPolicyName)]
    public class SqlSectionPolicyExecutor : PolicyExecutorBase, ISqlInitPolicyExecutor
    {

        public void Execute(ISqlInitPolicyExecutorInfo info)
        {
            //进行分部sql的合并
            foreach (var tableInfo in info.TableSqlInfos.Values)
            {
                var sqls = info.NewlySqls[tableInfo.Name];   //获取最新的sql
                foreach (var sqlPair in tableInfo.Sqls)
                {
                    var sql = sqls[sqlPair.Key];
                    //替换sql
                    sqls[sqlPair.Key] = RecurReplaceSectionSql(info, info.GetPolicy(sqlPair.Value, tableInfo) as SqlSectionPolicy, tableInfo, sql);
                }
            }
        }

        /// <summary>
        /// 递归所关联的所有Sql片段(注意配置的Sql不要互相嵌套，那么就死循环了)
        /// </summary>
        protected string RecurReplaceSectionSql(ISqlInitPolicyExecutorInfo info, SqlSectionPolicy section,
            IConfigTableInfo tableInfo, string sql)
        {
            if (IsUsePolicy(section))
            {
                var tagPrefix = string.IsNullOrEmpty(section.TagPrefix) ? SqlConfigConst.SqlSectionPrefixSymbol : section.TagPrefix;
                var tagSuffix = string.IsNullOrEmpty(section.TagSuffix) ? SqlConfigConst.SqlSectionSuffixSymbol : section.TagSuffix;
                IConfigSqlInfo tempSqlModel;

                //同一Table下的
                if (section.SqlNames?.Count > 0)
                {
                    foreach (var sqlName in section.SqlNames)
                    {
                        tempSqlModel = tableInfo.Sqls[sqlName];
                        sql = sql.Replace(tagPrefix + sqlName + tagSuffix,
                            //递归
                            RecurReplaceSectionSql(info, info.GetPolicy(tempSqlModel, tableInfo) as SqlSectionPolicy, tableInfo,
                                //获取最新的sql
                                info.NewlySqls[tableInfo.Name][sqlName]));
                    }
                }

                //指定了Table Name的
                if (section.TableSqlNames?.Count > 0)
                {
                    IConfigTableInfo tempTableInfo;
                    foreach (var pair in section.TableSqlNames)
                    {
                        tempTableInfo = info.TableSqlInfos[pair.Key];
                        tempSqlModel = tempTableInfo.Sqls[pair.Value];
                        sql = sql.Replace(tagPrefix + pair.Key + "." + pair.Value + tagSuffix,
                            //递归
                            RecurReplaceSectionSql(info, info.GetPolicy(tempSqlModel, tempTableInfo) as SqlSectionPolicy, tempTableInfo,
                                //获取最新的sql
                                info.NewlySqls[pair.Key][pair.Value]));
                    }
                }
            }

            return sql;
        }

    }
}
