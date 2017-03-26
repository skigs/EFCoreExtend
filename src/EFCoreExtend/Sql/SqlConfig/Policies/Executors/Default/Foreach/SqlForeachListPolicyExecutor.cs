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
    /// 对sql的参数SqlParameter为List类型的参数进行遍历策略 的执行器
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlForeachListPolicyName)]
    public class SqlForeachListPolicyExecutor : SqlParamObjForeachPolicyExecutorBase, ISqlPreExecutePolicyExecutor
    {
        public SqlForeachListPolicyExecutor(ISqlParamConverter sqlParamCvt, IObjectReflector objReflec, IEFCoreExtendUtility util)
            : base(sqlParamCvt, objReflec, util)
        {
        }

        public void Execute(ISqlPreExecutePolicyExecutorInfo info)
        {
            var policy = info.GetPolicy() as SqlForeachListPolicy;
            if (IsUsePolicy(policy) && info.SqlParams?.Length > 0)
            {
                string tempSql = info.Sql;
                var tempParams = info.SqlParams;

                IDataParameter[] newParams = null;

                tempParams = DoListSqlParamsForeach(info, policy, tempParams, tempSql, out newParams, out tempSql);

                //旧的SqlParams和新的SqlParams合并
                tempParams = _util.CombineDataParams(tempParams, newParams);

                info.SqlParams = tempParams;
                info.Sql = tempSql;
            }
        }

        protected IDataParameter[] DoListSqlParamsForeach(ISqlPreExecutePolicyExecutorInfo info, SqlForeachListPolicy policy,
            IDataParameter[] parameters, string sql,
            out IDataParameter[] newParameters, out string outSql)
        {
            newParameters = null;
            SqlForeachListPolicyInfo pinfo;
            Tuple<string, string> symbol;
            List<IDataParameter[]> listNewParams = new List<IDataParameter[]>();    //用于保存新生成的SqlParams
            IDataParameter[] newParams = null;
            int i = 0;

            //判断是否为检测所有的list类型进行遍历
            if (policy.IsAll)
            {
                parameters = DoForeachAndRemove(parameters, p =>
                {
                    if (IsListType(p.Value))
                    {
                        pinfo = GetPInfo(policy.Infos, p.ParameterName, policy.DefInfo);
                        CheckPInfo(pinfo, nameof(SqlForeachListPolicyInfo), p.ParameterName, SqlConfigConst.SqlForeachListPolicyName, info);

                        symbol = GetSymbol(pinfo);
                        //遍历与替换sql
                        sql = DoListForeach(info.DB, pinfo, symbol.Item1 + p.ParameterName + symbol.Item2,
                            SqlConfigConst.ForeachListVNamePrefix + p.ParameterName + "_" + i++ + "_",
                                p.Value as IEnumerable, sql, out newParams);
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
                        CheckListType(p.Value, p.ParameterName);
                        CheckPInfo(pinfo, nameof(SqlForeachListPolicyInfo), p.ParameterName, SqlConfigConst.SqlForeachListPolicyName, info);

                        symbol = GetSymbol(pinfo);
                        //遍历与替换sql
                        sql = DoListForeach(info.DB, pinfo, symbol.Item1 + p.ParameterName + symbol.Item2,
                            SqlConfigConst.ForeachListVNamePrefix + p.ParameterName + "_" + i++ + "_",
                                p.Value as IEnumerable, sql, out newParams);
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

        protected string DoListForeach(DbContext db, SqlForeachListPolicyInfo pinfo, string paramName, string valueName,
            IEnumerable list, string sql, out IDataParameter[] newParameters)
        {
            newParameters = null;
            if (pinfo.IsToSqlParam)
            {
                IDictionary<string, object> dictParam = new Dictionary<string, object>();
                int i = 0;
                string vpName;
                var strVals = list.JoinToString(pinfo.Separator, l =>
                {
                    vpName = valueName + i++;
                    //提取集合中的值成SqlParam
                    dictParam[vpName] = l;
                    //进行值的拼接(例如: [@_EL_Param_1_1])
                    return pinfo.VPrefix + SqlConfigConst.DBSymbol + vpName + pinfo.VSuffix;
                });
                //替换到sql中
                sql = sql.Replace(paramName, strVals);
                //获取新的SqlParams
                newParameters = _sqlParamCvt.DictionaryToDBParams(db, dictParam);
            }
            else
            {
                //对集合中的值进行拼接
                var strVals = list.JoinToString(pinfo.Separator, l => pinfo.VPrefix + l + pinfo.VSuffix);
                //替换到sql中
                sql = sql.Replace(paramName, strVals);
            }
            return sql;
        }

        protected bool IsListType(object obj)
        {
            var e = obj as IEnumerable;
            if (e != null)
            {
                var etype = e.GetType();
                var types = etype.GetGenericArguments();
                if (etype.IsArray || (types != null && types.Length == 1))
                {
                    return true;
                }
            }
            return false;
        }

        protected void CheckListType(object obj, string paramName)
        {
            if (!IsListType(obj))
            {
                obj.CheckNull(paramName);
                throw new ArgumentException($"The type [{obj.GetType().FullName}] can not as IEnumerable", paramName);
            }
        }

    }
}
