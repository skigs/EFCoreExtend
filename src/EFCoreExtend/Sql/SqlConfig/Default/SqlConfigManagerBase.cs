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
    public abstract class SqlConfigManagerBase : ISqlConfigManager
    {
        protected readonly ISqlConfig _config;
        public ISqlConfig Config => _config;
        protected readonly ISqlPolicyManager _policyMgr;
        public ISqlPolicyManager PolicyMgr => _policyMgr;

        protected readonly ISqlConfigExecutorCreator _sqlConfigExecutorCreator;
        protected readonly IEFQueryCache _efcache;
        protected readonly ISqlParamConverter _sqlParamConverter;
        protected readonly IObjectReflector _objReflec;
        protected readonly IEFCoreExtendUtility _util;

        protected readonly InitAction _init;
        /// <summary>
        /// 用于保存初始化后生成的sql
        /// </summary>
        protected readonly ConcurrentDictionary<string, IDictionary<string, string>> _dictNewlySqls
            = new ConcurrentDictionary<string, IDictionary<string, string>>();

        public SqlConfigManagerBase(ISqlConfigExecutorCreator sqlConfigExecutorCreator, IEFQueryCache efcache,
            ISqlParamConverter sqlParamConverter, ISqlPolicyManager policyExecutorMgr,
            IObjectReflector objReflec, IEFCoreExtendUtility util)
        {
            sqlConfigExecutorCreator.CheckNull(nameof(sqlConfigExecutorCreator));
            efcache.CheckNull(nameof(efcache));
            sqlParamConverter.CheckNull(nameof(sqlParamConverter));
            policyExecutorMgr.CheckNull(nameof(policyExecutorMgr));
            objReflec.CheckNull(nameof(objReflec));
            util.CheckNull(nameof(util));

            _sqlParamConverter = sqlParamConverter;
            _efcache = efcache;
            _sqlConfigExecutorCreator = sqlConfigExecutorCreator;
            _policyMgr = policyExecutorMgr;
            _objReflec = objReflec;
            _util = util;

            _config = new SqlConfig(OnModified);
            _init = new InitAction(DoInit);

            //添加默认的策略
            AddDefaultPolicies();
        }

        protected abstract void AddDefaultPolicies();

        private void OnModified()
        {
            //配置有新的修改，那么重置初始化
            _init.Release();
        }

        /// <summary>
        /// 初始化策略执行，例如替换表名 / 将配置的sql中的分部sql进行合并
        /// </summary>
        protected virtual void DoInit()
        {
            _dictNewlySqls.Clear(); //清除初始化整合的sql
            //初始化sql
            foreach (var table in _config.TableSqlInfos.Values)
            {
                IDictionary<string, string> tableSqls;
                if (!_dictNewlySqls.TryGetValue(table.Name, out tableSqls))
                {
                    tableSqls = new ConcurrentDictionary<string, string>();
                    _dictNewlySqls[table.Name] = tableSqls;
                }
                foreach (var sqlPair in table.Sqls)
                {
                    tableSqls[sqlPair.Key] = sqlPair.Value.Sql;
                }
            }

            //初始化型策略执行器的调用
            var info = new PolicyExecutorInfo()
            {
                NewlySqls = _dictNewlySqls as IReadOnlyDictionary<string, IDictionary<string, string>>,
                TableSqlInfos = _config.TableSqlInfos,
            };
            _policyMgr.InvokeInitPolicyExecutors(null, info);
        }

        public ISqlConfigExecutor GetExecutor(DbContext db, string tableName, [CallerMemberName] string sqlName = null)
        {
            //在获取Executor之前，进行初始化策略执行，只初始化一次（如果配置有新的修改，那么就会重新进行初始化）
            _init.Invoke(-1);

            var tableInfo = _config.TableSqlInfos[tableName];
            var sqlInfo = tableInfo.Sqls[sqlName];
            var newlySql = _dictNewlySqls[tableName][sqlName];
            return _sqlConfigExecutorCreator.Create(this, db, tableName, sqlName, sqlInfo, tableInfo, newlySql);
        }

    }
}
