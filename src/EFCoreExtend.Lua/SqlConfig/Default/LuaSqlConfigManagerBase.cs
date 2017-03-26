using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCoreExtend.Lua.SqlConfig.Policies;
using EFCoreExtend.EFCache;
using EFCoreExtend.Sql;
using EFCoreExtend.Commons;
using EFCoreExtend.Lua.SqlConfig.Policies.Default;
using System.Collections.Concurrent;
using EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs;

namespace EFCoreExtend.Lua.SqlConfig.Default
{
    public abstract class LuaSqlConfigManagerBase : ILuaSqlConfigManager
    {
        protected readonly ILuaSqlConfig _config;
        public ILuaSqlConfig Config => _config;
        protected readonly ILuaSqlPolicyManager _policyMgr;
        public ILuaSqlPolicyManager PolicyMgr => _policyMgr;

        /// <summary>
        /// 用于存储SqlParameters在lua中处理的相关函数
        /// </summary>
        protected readonly IDictionary<string, IDictionary<string, object>> _luaSqlParamFuncContainer =
            new ConcurrentDictionary<string, IDictionary<string, object>>();

        protected readonly ISqlParamConverter _sqlParamConverter;
        protected readonly IEFCoreExtendUtility _util;
        protected readonly ISqlExecutor _sqlExecutor;

        protected readonly InitAction _init;

        public LuaSqlConfigManagerBase(
            ISqlExecutor sqlExecutor,
            ISqlParamConverter sqlParamConverter, ILuaSqlPolicyManager policyExecutorMgr, 
            IEFCoreExtendUtility util, ILuaSqlConfig config)
        {
            
            sqlExecutor.CheckNull(nameof(sqlExecutor));
            sqlParamConverter.CheckNull(nameof(sqlParamConverter));
            policyExecutorMgr.CheckNull(nameof(policyExecutorMgr));
            util.CheckNull(nameof(util));
            config.CheckNull(nameof(config));

            _sqlParamConverter = sqlParamConverter;
            _policyMgr = policyExecutorMgr;
            _util = util;
            _sqlExecutor = sqlExecutor;

            _config = config;
            _config.OnModified += OnModified;

            _init = new InitAction(DoInit);

        }

        private void OnModified()
        {
            //配置有新的修改，那么重置初始化
            _init.Release();
        }

        /// <summary>
        /// 初始化策略执行，例如Lua sql的布局页(分部页)合并
        /// </summary>
        protected virtual void DoInit()
        {
            //初始化型策略执行器的调用
            var info = new LuaSqlInitPolicyExecutorInfo()
            {
                Config = _config,
                LuaSqlParamFuncsContainer = _luaSqlParamFuncContainer,
            };
            _policyMgr.InvokeInitPolicyExecutors(null, info);
        }

        public ILuaSqlConfigExecutor GetExecutor(DbContext db, string tableName, 
            [CallerMemberName] string sqlName = null)
        {
            //在获取Executor之前，进行初始化策略执行，只初始化一次（如果配置有新的修改，那么就会重新进行初始化）
            _init.Invoke(-1);

            var tableInfo = _config.TableSqlInfos[tableName];
            ILuaConfigSqlInfo sqlInfo;
            tableInfo.Sqls.TryGetValue(sqlName, out sqlInfo);            
            return new LuaSqlConfigExecutor(this, db, _luaSqlParamFuncContainer, tableName, sqlName, sqlInfo, tableInfo, _sqlExecutor,
                _sqlParamConverter, _util);
        }

    }
}
