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
    public abstract class SqlParamObjForeachPolicyExecutorBase : PolicyExecutorBase
    {
        protected readonly ISqlParamConverter _sqlParamCvt;
        protected readonly IObjectReflector _objReflec;
        protected readonly IEFCoreExtendUtility _util;

        public SqlParamObjForeachPolicyExecutorBase(ISqlParamConverter sqlParamCvt, IObjectReflector objReflec, IEFCoreExtendUtility util)
        {
            sqlParamCvt.CheckNull(nameof(sqlParamCvt));
            objReflec.CheckNull(nameof(objReflec));
            util.CheckNull(nameof(util));

            _sqlParamCvt = sqlParamCvt;
            _objReflec = objReflec;
            _util = util;
        }

        protected Tuple<string, string> GetSymbol(SqlParamObjEachPolicyInfoBase policy)
        {
            return Tuple.Create(
                string.IsNullOrEmpty(policy.TagPrefix) ? SqlConfigConst.SqlParamsForeachPrefixSymbol : policy.TagPrefix,
                string.IsNullOrEmpty(policy.TagSuffix) ? SqlConfigConst.SqlParamsForeachSuffixSymbol : policy.TagSuffix);
        }

        protected T GetPInfo<T>(IReadOnlyDictionary<string, T> pinfos, string paramName, T defInfo)
            where T : SqlParamObjEachPolicyInfoBase
        {
            T tempInfo = null;
            if (pinfos?.TryGetValue(paramName, out tempInfo) == true)
            {
                return tempInfo ?? defInfo;
            }
            else
            {
                return defInfo;
            }
        }

        protected bool TryGetPInfo<T>(IReadOnlyDictionary<string, T> pinfos, string paramName, T defInfo, out T outPInfo)
            where T : SqlParamObjEachPolicyInfoBase
        {
            outPInfo = null;
            if (pinfos?.TryGetValue(paramName, out outPInfo) == true)
            {
                outPInfo = outPInfo ?? defInfo;
                return true;
            }
            return false;
        }

        protected IDataParameter[] CombineListParams(IEnumerable<IDataParameter[]> list)
        {
            IDataParameter[] tempNewParams = null;
            foreach (var ps in list)
            {
                tempNewParams = _util.CombineDataParams(tempNewParams, ps);
            }
            return tempNewParams;
        }

        protected IDataParameter[] DoForeachAndRemove(IDataParameter[] parameters, Func<IDataParameter, bool> doParamForeach)
        {
            var listReserve = new List<IDataParameter>();
            foreach (var p in parameters)
            {
                //移除执行了Foreach的SqlParam
                if (!doParamForeach(p))
                {
                    listReserve.Add(p);
                }
            }
            return listReserve.ToArray();
        }

        protected void CheckPInfo(SqlParamObjEachPolicyInfoBase pinfo, string pinfoName, string sqlParamName,
            string policyName, ISqlPreExecutePolicyExecutorInfo info)
        {
            if (pinfo == null)
            {
                throw new ArgumentException(
                                $"{pinfoName} not found, SqlParameterName=[{sqlParamName}], " +
                                $"PolicyName=[{policyName}], " +
                                $"SqlName=[{info.SqlName}], " +
                                $"TableName=[{info.TableName}]");
            }
        }

    }
}
