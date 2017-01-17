using EFCoreExtend.Commons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    /// <summary>
    /// 这个策略执行器用于将配置文件中的策略数据(json数据对象)转换成策略对象，这个策略执行器应该无论如何都要调用，
    /// 以保证策略数据正确的转换成策略对象，这个策略执行器没有策略类(默认优先级为最高级：int.MaxValue)
    /// </summary>
    [SqlConfigPolicy("SqlConfigPolicyData2Type")]
    public class SqlConfigPolicyData2ObjectExecutor : ISqlInitPolicyExecutor
    {
        ISqlPolicyManager _policyMgr;
        public SqlConfigPolicyData2ObjectExecutor(ISqlPolicyManager policyMgr)
        {
            policyMgr.CheckNull(nameof(policyMgr));
            _policyMgr = policyMgr;
        }

        public void Execute(ISqlInitPolicyExecutorInfo info)
        {
            IConfigTableInfoModifier tmodif;
            IConfigSqlInfoModifier sqlmodif;
            foreach (var tpair in info.TableSqlInfos)
            {
                tmodif = tpair.Value as IConfigTableInfoModifier;
                //可以修改的才修改
                if (tmodif != null)
                {
                    ForeachPolicies(tmodif.Policies);
                    //Sql配置的
                    foreach (var sqlPair in tmodif.Sqls)
                    {
                        sqlmodif = sqlPair.Value as IConfigSqlInfoModifier;
                        if (sqlmodif != null)
                        {
                            ForeachPolicies(sqlmodif.Policies); 
                        }
                    } 
                }
            }
        }

        protected void ForeachPolicies(IDictionary<string, object> policies)
        {
            if (policies?.Count > 0)
            {
                var tempPolicies = policies.ToList();
                Type ptype;
                foreach (var tp in tempPolicies)
                {
                    if (tp.Value != null && _policyMgr.PolicyTypes.TryGetValue(tp.Key, out ptype))
                    {
                        policies[tp.Key] = GetPolicyValue(tp.Value, ptype);
                    }
                }
            }
        }

        /// <summary>
        /// //将配置的策略JSON数据对象 转换成 策略对象
        /// </summary>
        /// <param name="policyValue">JSON数据对象</param>
        /// <param name="policyType">策略类型</param>
        /// <returns></returns>
        protected object GetPolicyValue(object policyValue, Type policyType)
        {
            if (policyValue.GetType() != policyType)
            {
                return CommonExtensions.JsonToObjectNeedDefaultValue(JsonConvert.SerializeObject(policyValue), policyType);
            }
            return policyValue;
        }

    }
}
