using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using System.Data;
using System.Text;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    /// <summary>
    /// 对SqlParameter的数据进行遍历的策略 执行器
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlForeachParamsPolicyName)]
    public class SqlForeachParamsPolicyExecutor : SqlParamObjForeachPolicyExecutorBase, ISqlPreExecutePolicyExecutor
    {
        public SqlForeachParamsPolicyExecutor(ISqlParamConverter sqlParamCvt, IObjectReflector objReflec, IEFCoreExtendUtility util)
            : base(sqlParamCvt, objReflec, util)
        {
        }

        public void Execute(ISqlPreExecutePolicyExecutorInfo info)
        {
            var policy = info.GetPolicy() as SqlForeachParamsPolicy;
            if (IsUsePolicy(policy) && info.SqlParams?.Length > 0)
            {
                IDataParameter[] feachParams, reserveParams;
                GetSqlParams(policy, info.SqlParams, out feachParams, out reserveParams);

                if (feachParams?.Length > 0)
                {
                    if (policy.IsKVSplit)
                    {
                        info.Sql = DoForeachKVSplit(info, policy, info.Sql, feachParams, reserveParams, out reserveParams);
                    }
                    else
                    {
                        info.Sql = DoForeachNotKVSplit(info, policy, info.Sql, feachParams, reserveParams, out reserveParams);
                    }
                }

                info.SqlParams = reserveParams;
            }
        }

        protected string DoForeachKVSplit(ISqlPreExecutePolicyExecutorInfo info, SqlForeachParamsPolicy policy, string sql,
            IDataParameter[] feachParams, IDataParameter[] reserveParams, out IDataParameter[] outParams)
        {
            outParams = reserveParams;
            var tag = string.IsNullOrEmpty(policy.Tag) ? SqlConfigConst.SqlForeachParamsLabel : policy.Tag;
            var ksb = new StringBuilder();
            var vsb = new StringBuilder();
            bool bSplit = false;
            if (policy.IsToSqlParam)
            {
                int i = 0;
                string vname;
                var dictParams = new Dictionary<string, object>();

                foreach (var l in feachParams)
                {
                    if (bSplit)
                    {
                        ksb.Append(policy.KSeparator);
                        vsb.Append(policy.VSeparator);
                    }
                    else
                    {
                        bSplit = true;
                    }
                    vname = SqlConfigConst.ForeachParamsVNamePrefix + l.ParameterName + "_" + i++;
                    dictParams[vname] = l.Value;
                    ksb.Append(policy.KPrefix + l.ParameterName + policy.KSuffix);
                    vsb.Append(policy.VPrefix + SqlConfigConst.DBSymbol + vname + policy.VSuffix);
                }

                //合并SqlParameter
                outParams = _util.CombineDataParams(reserveParams,
                    _sqlParamCvt.DictionaryToDBParams(info.DB, dictParams));
                //替换到sql中
                sql = sql.Replace(tag + SqlConfigConst.SqlForeachKeyLabel, ksb.ToString());
                sql = sql.Replace(tag + SqlConfigConst.SqlForeachValueLabel, vsb.ToString());
            }
            else
            {
                foreach (var l in feachParams)
                {
                    if (bSplit)
                    {
                        ksb.Append(policy.KSeparator);
                        vsb.Append(policy.VSeparator);
                    }
                    else
                    {
                        bSplit = true;
                    }
                    ksb.Append(policy.KPrefix + l.ParameterName + policy.KSuffix);
                    vsb.Append(policy.VPrefix + l.Value + policy.VSuffix);
                }

                //替换到sql中
                sql = sql.Replace(tag + SqlConfigConst.SqlForeachKeyLabel, ksb.ToString());
                sql = sql.Replace(tag + SqlConfigConst.SqlForeachValueLabel, vsb.ToString());
            }
            return sql;
        }

        protected string DoForeachNotKVSplit(ISqlPreExecutePolicyExecutorInfo info, SqlForeachParamsPolicy policy, string sql,
            IDataParameter[] feachParams, IDataParameter[] reserveParams, out IDataParameter[] outParams)
        {
            outParams = reserveParams;
            var tag = string.IsNullOrEmpty(policy.Tag) ? SqlConfigConst.SqlForeachParamsLabel : policy.Tag;
            if (policy.IsToSqlParam)
            {
                int i = 0;
                string vname;
                var dictParams = new Dictionary<string, object>();
                //对key-value进行拼接
                var strVals = feachParams.JoinToString(policy.Separator, l =>
                {
                    vname = SqlConfigConst.ForeachParamsVNamePrefix + l.ParameterName + "_" + i++;
                    dictParams[vname] = l.Value;
                    return policy.KPrefix + l.ParameterName + policy.KSuffix + policy.KVSeparator +
                        policy.VPrefix + SqlConfigConst.DBSymbol + vname + policy.VSuffix;
                });
                //合并SqlParameter
                outParams = _util.CombineDataParams(reserveParams,
                    _sqlParamCvt.DictionaryToDBParams(info.DB, dictParams));
                //替换到sql中
                sql = sql.Replace(tag, strVals);
            }
            else
            {
                //对key-value进行拼接
                var strVals = feachParams.JoinToString(policy.Separator,
                l => policy.KPrefix + l.ParameterName + policy.KSuffix + policy.KVSeparator +
                    policy.VPrefix + l.Value + policy.VSuffix);
                //替换到sql中
                sql = sql.Replace(tag, strVals);
            }
            return sql;
        }

        protected void GetSqlParams(SqlForeachParamsPolicy policy, IDataParameter[] parameters,
            out IDataParameter[] feachParams, out IDataParameter[] reserveParams)
        {
            if (policy.IgnoreParams?.Count > 0)
            {
                var tempFeachParams = new List<IDataParameter>();   //需要进行遍历的
                var tempReserveParams = new List<IDataParameter>(); //不需要进行遍历的
                foreach (var p in parameters)
                {
                    if (policy.IgnoreParams.Contains(p.ParameterName))
                    {
                        tempReserveParams.Add(p);
                    }
                    else
                    {
                        tempFeachParams.Add(p);
                    }
                }
                feachParams = tempFeachParams.ToArray();
                reserveParams = tempReserveParams.ToArray();
            }
            else
            {
                feachParams = parameters;
                reserveParams = new IDataParameter[0];
            }
        }

    }
}
