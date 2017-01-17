using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class SqlConfigInfoExtensions
    {

        /// <summary>
        /// 添加Table的sql
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="sqlInfo"></param>
        public static void AddSql(this IConfigTableInfoModifier info, string sqlName, IConfigSqlInfo sqlInfo)
        {
            sqlName.CheckStringIsNullOrEmpty(nameof(sqlName));
            sqlInfo.CheckNull(nameof(sqlInfo));
            sqlInfo.Sql.CheckStringIsNullOrEmpty($"{nameof(sqlInfo)}.{nameof(sqlInfo.Sql)}");

            info.Sqls.Add(sqlName, sqlInfo);
        }

        /// <summary>
        /// 移除Table的sql
        /// </summary>
        /// <param name="info"></param>
        /// <param name="sqlName"></param>
        /// <param name="sqlInfo"></param>
        /// <returns></returns>
        public static bool TryRemoveSql(this IConfigTableInfoModifier info, string sqlName, out IConfigSqlInfo sqlInfo)
        {
            return info.Sqls.DictTryRemove(sqlName, out sqlInfo);
        }

        /// <summary>
        /// 添加Table的策略
        /// </summary>
        /// <param name="policyName"></param>
        /// <param name="policy"></param>
        public static void AddPolicy(this IConfigTableInfoModifier info, string policyName, object policy)
        {
            policyName.CheckStringIsNullOrEmpty(nameof(policyName));
            policy.CheckNull(nameof(policy));

            info.Policies.Add(policyName, policy);
        }

        /// <summary>
        /// 添加Table的策略
        /// </summary>
        /// <param name="info"></param>
        /// <param name="policy"></param>
        public static void AddPolicy(this IConfigTableInfoModifier info, ISqlConfigPolicy policy)
        {
            policy.CheckNull(nameof(policy));

            info.Policies.Add(EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(policy.GetType()), 
                policy);
        }

        /// <summary>
        /// 移除Table的策略
        /// </summary>
        /// <param name="info"></param>
        /// <param name="policyName"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static bool TryRemovePolicy(this IConfigTableInfoModifier info, string policyName, out object policy)
        {
            return info.Policies.DictTryRemove(policyName, out policy);
        }

        /// <summary>
        /// 添加Sql的策略
        /// </summary>
        /// <param name="policyName"></param>
        /// <param name="policy"></param>
        public static void AddPolicy(this IConfigSqlInfoModifier info, string policyName, object policy)
        {
            policyName.CheckStringIsNullOrEmpty(nameof(policyName));
            policy.CheckNull(nameof(policy));

            info.Policies.Add(policyName, policy);
        }

        /// <summary>
        /// 添加Sql的策略
        /// </summary>
        /// <param name="policy"></param>
        public static void AddPolicy(this IConfigSqlInfoModifier info, ISqlConfigPolicy policy)
        {
            policy.CheckNull(nameof(policy));

            info.Policies.Add(EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(policy.GetType()), 
                policy);
        }

        /// <summary>
        /// 移除Sql的策略
        /// </summary>
        /// <param name="info"></param>
        /// <param name="policyName"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static bool TryRemovePolicy(this IConfigSqlInfoModifier info, string policyName, out object policy)
        {
            return info.Policies.DictTryRemove(policyName, out policy);
        }

    }
}
