using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EFCoreExtend.Commons;
using EFCoreExtend.EFCache;
using EFCoreExtend.Lua.SqlConfig.Policies;
using EFCoreExtend.Sql;
using EFCoreExtend.Sql.SqlConfig.Policies;
using Microsoft.EntityFrameworkCore;
using EFCoreExtend.Lua.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs;
using EFCoreExtend.EFCache.Default;

namespace EFCoreExtend.Lua.SqlConfig.Default
{
    public class LuaSqlConfigManager : LuaSqlConfigManagerBase
    {
        protected readonly ILuaFuncManager _luafuncs;
        protected readonly IObjectReflector _objReflec;
        protected readonly IEFQueryCache _efcache;

        public LuaSqlConfigManager(IEFQueryCache efcache, ISqlExecutor sqlExecutor, 
            ISqlParamConverter sqlParamConverter, ILuaSqlPolicyManager policyExecutorMgr, 
            IObjectReflector objReflec, IEFCoreExtendUtility util, ILuaFuncManager luafuncs, ILuaSqlConfig config)
            : base(sqlExecutor, sqlParamConverter, policyExecutorMgr, util, config)
        {
            objReflec.CheckNull(nameof(objReflec));
            efcache.CheckNull(nameof(efcache));
            luafuncs.CheckNull(nameof(luafuncs));

            _luafuncs = luafuncs;
            _efcache = efcache;
            _objReflec = objReflec;

            //添加默认的策略
            AddDefaultPolicies();
        }

        protected void AddDefaultPolicies()
        {
            var luasqlInitExc = new LuaSqlConfigInitExecutor(_policyMgr);
            //策略配置数据替换的策略执行器要最先执行(这个执行器没有策略对象)
            _policyMgr.SetExecutor<ILuaSqlInitPolicyExecutor>(_util.GetSqlConfigPolicyName(luasqlInitExc.GetType()), 
                () => luasqlInitExc, int.MaxValue);

            var defFuncInitExc = new LuaSqlInitDefaultFuncsExecutor(_luafuncs);
            //策略配置数据替换的策略执行器要最先执行(这个执行器没有策略对象)
            _policyMgr.SetExecutor<ILuaSqlInitPolicyExecutor>(_util.GetSqlConfigPolicyName(defFuncInitExc.GetType()),
                () => defFuncInitExc, int.MaxValue - 1);

            //lua函数运行时默认参数初始化
            var luapfuncExe = new LuaSqlParamFuncsExecutor(_luafuncs);
            _policyMgr.SetExecutor<ILuaSqlPreExecutePolicyExecutor>(
                _util.GetSqlConfigPolicyName(luapfuncExe.GetType()), () => luapfuncExe, int.MaxValue);

            var l2cacheExc = new LuaSqlL2QueryCachePolicyExecutor(_efcache, _util);
            //查询缓存
            AddDefaultPolicy<ILuaSqlExecutePolicyExecutor>(typeof(SqlL2QueryCachePolicy), 
                () => l2cacheExc, -100);
            var clearcacheExc = new LuaSqlClearCachePolicyExecutor(_efcache);
            //缓存清理(scalar也是可以清理缓存的，因为scalar也是可以执行非查询的sql语句)
            AddDefaultPolicy<ILuaSqlExecutePolicyExecutor>(typeof(SqlClearCachePolicy), 
                () => clearcacheExc, -101);


            //////sql日志记录策略与执行器默认不添加
            ////var logExc = new LuaSqlExecuteLogPolicyExecutor();
            ////AddDefaultPolicy(typeof(SqlConfigExecuteLogPolicy), () => (ILuaSqlExecutePolicyExecutor)logExc);
            ////AddDefaultPolicy<SqlConfigExecuteLogPolicyExecutor>(typeof(SqlConfigExecuteLogPolicy), null, 0, true);
            var logtype = typeof(SqlConfigExecuteLogPolicy);
            var logname = _util.GetSqlConfigPolicyName(logtype);
            _policyMgr.PolicyTypes[logname] = logtype;

        }

        protected void AddDefaultPolicy<T>(Type ptype, Func<T> getExecutorFunc, int priority,
            bool isAddPolicyObj = false)
        {
            var pname = _util.GetSqlConfigPolicyName(ptype);
            _policyMgr.PolicyTypes[pname] = ptype;
            //添加策略执行器
            if (getExecutorFunc != null)
            {
                _policyMgr.SetExecutor(pname, getExecutorFunc, priority);
            }
            //添加默认的全局策略对象
            if (isAddPolicyObj)
            {
                //使用JsonConvert目的为了生成默认的数据对象
                var policyObj = CommonExtensions.JsonToObjectNeedDefaultValue("{}", ptype);
                _policyMgr.GlobalPolicies[_util.GetSqlConfigPolicyName(ptype)] = (ISqlConfigPolicy)policyObj;
            }
        }

    }
}
