using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    /// <summary>
    /// 对sql的参数SqlParameter为Model对象的参数进行遍历策略 的执行器
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlForeachModelPolicyName)]
    public class SqlForeachModelPolicyExecutor : ForeachKVPairPolicyExecutorBase, ISqlPreExecutePolicyExecutor
    {
        public SqlForeachModelPolicyExecutor(ISqlParamConverter sqlParamCvt, IObjectReflector objReflec, IEFCoreExtendUtility util) 
            : base(sqlParamCvt, objReflec, util)
        {
        }

        public void Execute(ISqlPreExecutePolicyExecutorInfo info)
        {
            var policy = info.GetPolicy() as SqlForeachModelPolicy;
            if (IsUsePolicy(policy) && info.SqlParams?.Length > 0)
            {
                string tempSql = info.Sql;
                var tempParams = info.SqlParams;

                IDataParameter[] newParams = null;
                tempParams = DoModelSqlParamsForeach(info, policy, tempParams, tempSql, out newParams, out tempSql);

                //旧的SqlParams和新的SqlParams合并
                tempParams = _util.CombineDataParams(tempParams, newParams);

                info.SqlParams = tempParams;
                info.Sql = tempSql;
            }
        }

        protected IDataParameter[] DoModelSqlParamsForeach(ISqlPreExecutePolicyExecutorInfo info, SqlForeachModelPolicy policy,
            IDataParameter[] parameters, string sql,
            out IDataParameter[] newParameters, out string outSql)
        {
            newParameters = null;
            SqlForeachModelPolicyInfo pinfo;
            List<IDataParameter[]> listNewParams = new List<IDataParameter[]>();    //用于保存新生成的SqlParams
            IDataParameter[] newParams = null;
            int i = 0;

            if (policy.Infos?.Count > 0)
            {
                //对指定名称的进行遍历
                parameters = DoForeachAndRemove(parameters, p =>
                {
                    if (TryGetPInfo(policy.Infos, p.ParameterName, policy.DefInfo, out pinfo))
                    {
                        CheckPInfo(pinfo, nameof(SqlForeachModelPolicyInfo), p.ParameterName, SqlConfigConst.SqlForeachModelPolicyName, info);

                        var dict = _objReflec.GetPublicInstanceProptValues(p.Value, pinfo.IgnoreKeys);
                        //遍历与替换sql
                        sql = DoKVPairForeach(info.DB, pinfo, p.ParameterName,
                            dict, sql, SqlConfigConst.ForeachModelVNamePrefix + p.ParameterName + "_" + i++ + "_", out newParams);
                        listNewParams.Add(newParams);
                        return true;
                    }
                    return false;
                });
            }

            //合并新的SqlParams
            newParameters = CombineListParams(listNewParams);

            outSql = sql;
            return parameters;
        }


    }
}
