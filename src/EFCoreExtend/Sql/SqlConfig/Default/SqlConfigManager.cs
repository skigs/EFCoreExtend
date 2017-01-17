using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCoreExtend.EFCache;
using EFCoreExtend.Commons;
using System.Data;
using EFCoreExtend.Sql.SqlConfig.Executors;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors;
using System.Collections.Concurrent;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default;
using Newtonsoft.Json;

namespace EFCoreExtend.Sql.SqlConfig.Default
{
    public class SqlConfigManager : SqlConfigManagerBase
    {
        public SqlConfigManager(ISqlConfigExecutorCreator sqlConfigExecutorCreator, IEFQueryCache efcache,
            ISqlParamConverter sqlParamConverter, ISqlPolicyManager policyExecutorMgr, 
            IObjectReflector objReflec, IEFCoreExtendUtility util)
            :base(sqlConfigExecutorCreator, efcache, sqlParamConverter, policyExecutorMgr, objReflec, util)
        {
        }

        protected override void AddDefaultPolicies()
        {
            var d2tExc = new SqlConfigPolicyData2ObjectExecutor(_policyMgr);
            //策略配置数据替换的策略执行器要最先执行(这个执行器没有策略对象)
            _policyMgr.SetExecutor(_util.GetSqlConfigPolicyName(d2tExc.GetType()), () => (ISqlInitPolicyExecutor)d2tExc, int.MaxValue);

            var tnExc = new TableNamePolicyExecutor();
            //表名的替换要比分部sql合并先执行
            AddDefaultPolicy(typeof(TableNamePolicy), () => (ISqlInitPolicyExecutor)tnExc, int.MaxValue - 100, true);

            var ssExc = new SqlSectionPolicyExecutor();
            //分部sql合并策略执行器
            AddDefaultPolicy(typeof(SqlSectionPolicy), () => (ISqlInitPolicyExecutor)ssExc, int.MaxValue - 200);

            //foreach
            var eachParamsExc = new SqlForeachParamsPolicyExecutor(_sqlParamConverter, _objReflec, _util);
            AddDefaultPolicy(typeof(SqlForeachParamsPolicy), () => (ISqlPreExecutePolicyExecutor)eachParamsExc, int.MaxValue - 104);
            var eachListExc = new SqlForeachListPolicyExecutor(_sqlParamConverter, _objReflec, _util);
            AddDefaultPolicy(typeof(SqlForeachListPolicy), () => (ISqlPreExecutePolicyExecutor)eachListExc, int.MaxValue - 101);
            var eachDictExc = new SqlForeachDictPolicyExecutor(_sqlParamConverter, _objReflec, _util);
            AddDefaultPolicy(typeof(SqlForeachDictPolicy), () => (ISqlPreExecutePolicyExecutor)eachDictExc, int.MaxValue - 102);
            var eachModelExc = new SqlForeachModelPolicyExecutor(_sqlParamConverter, _objReflec, _util);
            AddDefaultPolicy(typeof(SqlForeachModelPolicy), () => (ISqlPreExecutePolicyExecutor)eachModelExc, int.MaxValue - 103);

            #region sql执行的策略执行器
            //sql执行器的都先设置负数，因为方便设置sql日志记录策略执行器

            //一级缓存
            var l1cacheExc = new SqlL1QueryCachePolicyExecutor(_util);
            AddDefaultPolicy(typeof(SqlL1QueryCachePolicy), () => (ISqlExecutePolicyExecutor)l1cacheExc,
                -200);

            var l2cacheExc = new SqlL2QueryCachePolicyExecutor(_efcache, _util);
            //二级缓存要比一级缓存要先执行
            AddDefaultPolicy(typeof(SqlL2QueryCachePolicy), () => (ISqlExecutePolicyExecutor)l2cacheExc, -100);

            var clearcacheExc = new SqlClearCachePolicyExecutor(_efcache);
            //二级缓存清理（优先级：Clear要比L2Cache低，因为scalar是既可以缓存也可以清理缓存的，因此如果scalar类型的，默认先执行缓存）
            AddDefaultPolicy(typeof(SqlClearCachePolicy), () => (ISqlExecutePolicyExecutor)clearcacheExc, -101); 

            #endregion

            //sql日志记录策略与执行器默认不添加
            //var logExc = new SqlConfigExecuteLogPolicyExecutor();
            //AddDefaultPolicy(typeof(SqlConfigExecuteLogPolicy), () => (ISqlExecutePolicyExecutor)logExc);
            //AddDefaultPolicy<SqlConfigExecuteLogPolicyExecutor>(typeof(SqlConfigExecuteLogPolicy), null, 0, true);
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
