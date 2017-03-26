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
    /// 对sql的参数SqlParameter为Dictionary类型的参数进行遍历策略 的执行器
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlForeachDictPolicyName)]
    public class SqlForeachDictPolicyExecutor : ForeachKVPairPolicyExecutorBase, ISqlPreExecutePolicyExecutor
    {
        public SqlForeachDictPolicyExecutor(ISqlParamConverter sqlParamCvt, IObjectReflector objReflec, IEFCoreExtendUtility util) 
            : base(sqlParamCvt, objReflec, util)
        {
        }

        public void Execute(ISqlPreExecutePolicyExecutorInfo info)
        {
            var policy = info.GetPolicy() as SqlForeachDictPolicy;
            if (IsUsePolicy(policy) && info.SqlParams?.Length > 0)
            {
                string tempSql = info.Sql;
                var tempParams = info.SqlParams;

                IDataParameter[] newParams = null;
                tempParams = DoDictSqlParamsForeach(info, policy, 
                    tempParams, tempSql, out newParams, out tempSql);

                //旧的SqlParams和新的SqlParams合并
                tempParams = _util.CombineDataParams(tempParams, newParams);

                info.SqlParams = tempParams;
                info.Sql = tempSql;
            }
        }

        protected IDataParameter[] DoDictSqlParamsForeach(ISqlPreExecutePolicyExecutorInfo info, SqlForeachDictPolicy policy, 
            IDataParameter[] parameters, string sql,
            out IDataParameter[] newParameters, out string outSql)
        {
            newParameters = null;
            SqlForeachDictPolicyInfo pinfo;
            List<IDataParameter[]> listNewParams = new List<IDataParameter[]>();    //用于保存新生成的SqlParams
            IDataParameter[] newParams = null;
            int i = 0;

            //判断是否为检测所有的list类型进行遍历
            if (policy.IsAll)
            {
                parameters = DoForeachAndRemove(parameters, p =>
                {
                    if (IsDictType(p.Value))
                    {
                        pinfo = GetPInfo(policy.Infos, p.ParameterName, policy.DefInfo);
                        CheckPInfo(pinfo, nameof(SqlForeachDictPolicyInfo), p.ParameterName, SqlConfigConst.SqlForeachDictPolicyName, info);

                        var dict = GetDict(p.Value, pinfo);
                        //遍历与替换sql
                        sql = DoKVPairForeach(info.DB, pinfo, p.ParameterName,
                                dict, sql, SqlConfigConst.ForeachDictVNamePrefix + p.ParameterName + "_" + i++ + "_", out newParams);
                        listNewParams.Add(newParams);
                        return true;
                    }
                    return false;
                });
            }
            else if (policy.Infos?.Count > 0)
            {
                //对指定名称的进行遍历
                parameters = DoForeachAndRemove(parameters, p =>
                {
                    if (TryGetPInfo(policy.Infos, p.ParameterName, policy.DefInfo, out pinfo))
                    {
                        CheckDictType(p.Value, p.ParameterName);
                        CheckPInfo(pinfo, nameof(SqlForeachDictPolicyInfo), p.ParameterName, SqlConfigConst.SqlForeachDictPolicyName, info);

                        var dict = GetDict(p.Value, pinfo);
                        //遍历与替换sql
                        sql = DoKVPairForeach(info.DB, pinfo, p.ParameterName,
                                dict, sql, SqlConfigConst.ForeachDictVNamePrefix + p.ParameterName + "_" + i++ + "_", out newParams);
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

        protected IEnumerable<KeyValuePair<string, object>> GetDict(object value, SqlForeachDictPolicyInfo policy)
        {
            var dict = value as IEnumerable<KeyValuePair<string, object>>;
            if (policy.IgnoreKeys?.Count > 0)
            {
                //去除忽略的keys
                dict = dict.Where(l => !policy.IgnoreKeys.Contains(l.Key)).ToList();
            }
            return dict;
        }

        protected bool IsDictType(object obj)
        {
            //var e = obj as IEnumerable;
            //if (e != null)
            //{
            //    var types = e.GetType().GetGenericArguments();
            //    if (types != null && types.Length == 2)
            //    {
            //        return true;
            //    }
            //}
            //return false;

            return obj is IEnumerable<KeyValuePair<string, object>>;
        }

        protected void CheckDictType(object obj, string paramName)
        {
            if (!IsDictType(obj))
            {
                obj.CheckNull(paramName);
                throw new ArgumentException($"The type [{obj.GetType().FullName}] can not as IEnumerable<KeyValuePair<string, object>>", paramName);
            }
        }

    }
}
