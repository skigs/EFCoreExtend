using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    public abstract class ForeachKVPairPolicyExecutorBase : SqlParamObjForeachPolicyExecutorBase
    {
        public ForeachKVPairPolicyExecutorBase(ISqlParamConverter sqlParamCvt, IObjectReflector objReflec, IEFCoreExtendUtility util)
            : base(sqlParamCvt, objReflec, util)
        {
        }

        protected string DoKVPairForeach(DbContext db, KVPairForeachPolicyInfoBase policy, string paramName,
            IEnumerable<KeyValuePair<string, object>> dict, string sql, string valueName, out IDataParameter[] newParameters)
        {
            var symbol = GetSymbol(policy);
            if (policy.IsToSqlParam)
            {
                return DoForeachToSqlParam(db, policy, paramName, symbol, dict, sql, valueName, out newParameters);
            }
            else
            {
                newParameters = null;
                return DoForeachNotToSqlParam(db, policy, paramName, symbol, dict, sql, valueName);
            }
        }

        protected string DoForeachToSqlParam(DbContext db, KVPairForeachPolicyInfoBase policy, string paramName, 
            Tuple<string, string> symbol,
            IEnumerable<KeyValuePair<string, object>> dict, string sql, string valueName, out IDataParameter[] newParameters)
        {
            IDictionary<string, object> dictParam = new Dictionary<string, object>();
            string vpName;
            int i = 0;

            if (policy.IsKVSplit)
            {
                var kvVals = dict.JoinToString<string, object>(policy.KSeparator, key =>
                {
                    //进行key的拼接(例如: [name1],[name2])
                    return policy.KPrefix + key + policy.KSuffix;
                }, policy.VSeparator, value =>
                {
                    vpName = valueName + i++;
                    //value转为SqlParam
                    dictParam[vpName] = value;
                    //进行value的拼接(例如: @valueName_1,@valueName_2)
                    return policy.VPrefix + SqlConfigConst.DBSymbol + vpName + policy.VSuffix;
                });
                //替换到sql中
                sql = sql.Replace(symbol.Item1 + paramName + SqlConfigConst.SqlForeachKeyLabel + symbol.Item2, kvVals.Key);
                sql = sql.Replace(symbol.Item1 + paramName + SqlConfigConst.SqlForeachValueLabel + symbol.Item2, kvVals.Value);
            }
            else
            {
                var strVals = dict.JoinToString<KeyValuePair<string, object>>(policy.Separator, l =>
                {
                    vpName = valueName + i++;
                    //value转为SqlParam
                    dictParam[vpName] = l.Value;
                    //进行key-value的拼接(例如: [name]=@valueName)
                    return policy.KPrefix + l.Key + policy.KSuffix + policy.KVSeparator +
                                    policy.VPrefix + SqlConfigConst.DBSymbol + vpName + policy.VSuffix;
                });
                //替换到sql中
                sql = sql.Replace(symbol.Item1 + paramName + symbol.Item2, strVals);
            }

            //获取新的SqlParams
            newParameters = _sqlParamCvt.DictionaryToDBParams(db, dictParam);
            return sql;
        }

        protected string DoForeachNotToSqlParam(DbContext db, KVPairForeachPolicyInfoBase policy, string paramName,
            Tuple<string, string> symbol,
            IEnumerable<KeyValuePair<string, object>> dict, string sql, string valueName)
        {
            if (policy.IsKVSplit)
            {
                var kvVals = dict.JoinToString<string, object>(policy.KSeparator, key =>
                {
                    //进行key的拼接(例如: [name1],[name2])
                    return policy.KPrefix + key + policy.KSuffix;
                }, policy.VSeparator, value =>
                {
                    //进行value的拼接(例如: @value_1,@value_2)
                    return policy.VPrefix + value + policy.VSuffix;
                });
                //替换到sql中
                sql = sql.Replace(symbol.Item1 + paramName + SqlConfigConst.SqlForeachKeyLabel + symbol.Item2, kvVals.Key);
                sql = sql.Replace(symbol.Item1 + paramName + SqlConfigConst.SqlForeachValueLabel + symbol.Item2, kvVals.Value);
            }
            else
            {
                //对集合中的key-value进行拼接
                var strVals = dict.JoinToString<KeyValuePair<string, object>>(policy.Separator,
                l => policy.KPrefix + l.Key + policy.KSuffix + policy.KVSeparator +
                    policy.VPrefix + l.Value + policy.VSuffix);
                //替换到sql中
                sql = sql.Replace(symbol.Item1 + paramName + symbol.Item2, strVals);
            }
            return sql;
        }

    }
}
