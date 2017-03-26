using EFCoreExtend.Commons;
using EFCoreExtend.Lua.SqlConfig.Default;
using EFCoreExtend.Sql.SqlConfig.Policies;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.Default
{    
    /// <summary>
    /// 这个策略执行器用于将配置文件中的策略数据(lua数据对象)转换成策略对象，这个策略执行器应该无论如何都要调用，
    /// 以保证策略数据正确的转换成策略对象，这个策略执行器没有策略类(默认优先级为最高级：int.MaxValue)
    /// </summary>
    [SqlConfigPolicy("LuaSqlInit")]
    public class LuaSqlConfigInitExecutor : ILuaSqlInitPolicyExecutor
    {
        ILuaSqlPolicyManager _policyMgr;
        public LuaSqlConfigInitExecutor(ILuaSqlPolicyManager policyMgr)
        {
            policyMgr.CheckNull(nameof(policyMgr));
            _policyMgr = policyMgr;
        }

        public void Execute(ILuaSqlInitPolicyExecutorInfo info)
        {
            info.Config.Init(); //配置初始化

            //全局策略对象初始化
            var luacfg = (Table)info.Config.GetGlobalLuaParam(LuaSqlConfigConst.SqlConfigLabel)[0];
            var gcfg = (Table)luacfg[LuaSqlConfigConst.SqlConfigGlobalLabel];
            ForeachPolicies(info,
                tname => (Table)gcfg[LuaSqlConfigConst.TablePoliciesLabel],
                tname => (Table)gcfg[LuaSqlConfigConst.SqlPoliciesLabel]);

            //表配置的
            ForeachPolicies(info, 
                tname => (Table)info.Config.GetLuaParam(tname, LuaSqlConfigConst.TablePoliciesLabel)[0],
                tname => (Table)info.Config.GetLuaParam(tname, LuaSqlConfigConst.SqlPoliciesLabel)[0]);
        }

        protected void ForeachPolicies(ILuaSqlInitPolicyExecutorInfo info, Func<string, Table> getTps, Func<string, Table> getSps)
        {
            ILuaConfigTableInfo tinfo;
            ILuaConfigSqlInfo sinfo;
            //从lua配置中提取策略对象
            foreach (var tpair in info.Config.TableSqlInfos)
            {
                tinfo = tpair.Value;
                //lua中tabel的策略配置
                var tps = getTps(tpair.Key);
                LuaPoliciesParse(tinfo.Policies, tps);

                //lua中sql的策略配置
                var sps = getSps(tpair.Key);
                foreach (var spair in sps.Pairs)
                {
                    sinfo = new LuaConfigSqlInfo();
                    tinfo.Sqls[spair.Key.String] = sinfo;
                    LuaPoliciesParse(sinfo.Policies, spair.Value.Table);
                }
            }
        }

        protected void LuaPoliciesParse(IDictionary<string, object> policies, object luapolis)
        {
            if (luapolis is Table)
            {
                var table = (Table)luapolis;
                var temppolis = table.ToObject<IDictionary<string, object>>();
                ForeachPolicies(temppolis);
                foreach (var p in temppolis)
                {
                    policies[p.Key] = p.Value;
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
        /// 将配置的策略lua数据对象 转换成 C#策略对象
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
