using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCoreExtend.Commons;
using EFCoreExtend.Sql;
using System.Collections.Concurrent;
using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Lua.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies;

namespace EFCoreExtend.Lua.SqlConfig.Default
{
    public class LuaSqlConfigExecutor : ILuaSqlConfigExecutor
    {
        #region 数据字段
        protected readonly string _tableName;
        public string TableName => _tableName;

        protected readonly string _sqlName;
        public string SqlName => _sqlName;

        protected readonly DbContext _db;
        public DbContext DB => _db;

        protected readonly ISqlExecutor _sqlExecutor;
        protected readonly ISqlParamConverter _sqlParamCvt;
        protected readonly ILuaSqlConfigManager _sqlConfigMgr;
        protected readonly IEFCoreExtendUtility _util;
        protected readonly ILuaConfigSqlInfo _sqlInfo;
        protected readonly ILuaConfigTableInfo _tableInfo;
        protected readonly IDictionary<string, IDictionary<string, object>> _luaSqlParamFuncContainer;
        #endregion

        public LuaSqlConfigExecutor(ILuaSqlConfigManager sqlConfigMgr, DbContext db,
            IDictionary<string, IDictionary<string, object>> luaSqlParamFuncContainer,
            string tableName, string sqlName, ILuaConfigSqlInfo sqlInfo, ILuaConfigTableInfo tableInfo,
            ISqlExecutor sqlExecutor, ISqlParamConverter sqlParamCvt, IEFCoreExtendUtility util)
        {
            _luaSqlParamFuncContainer = luaSqlParamFuncContainer;
            _sqlConfigMgr = sqlConfigMgr;
            _db = db;
            _sqlInfo = sqlInfo;
            _tableInfo = tableInfo;
            _tableName = tableName;
            _sqlName = sqlName;

            _sqlExecutor = sqlExecutor;
            _sqlParamCvt = sqlParamCvt;
            _util = util;

        }

        public IReadOnlyList<T> QueryUseDict<T>(IDictionary<string, object> parameters,
            IEnumerable<string> ignoreProptsForRtnType = null,
            IDictionary<string, ISqlConfigPolicy> policies = null) where T : new()
        {
            var info = RunLua(parameters, ConfigSqlExecuteType.query, policies);
            info.ReturnType = typeof(IReadOnlyList<T>);

            info.ToDBExecutor = () => _sqlExecutor.Query<T>(_db, info.Sql, info.SqlParams, ignoreProptsForRtnType);
            //sql执行的策略器执行
            _sqlConfigMgr.PolicyMgr.InvokeExecutePolicyExecutors(policies, info);

            //判断是否结束
            if (info.IsEnd)
            {
                return info.ReturnValue as IReadOnlyList<T>;
            }
            else
            {
                //如果sql执行的策略器执行后没有设定IsEnd，那么直接执行sql
                return _sqlExecutor.Query<T>(_db, info.Sql, info.SqlParams, ignoreProptsForRtnType);
            }
        }

        public object ScalarUseDict(IDictionary<string, object> parameters,
            IDictionary<string, ISqlConfigPolicy> policies = null)
        {
            var info = RunLua(parameters, ConfigSqlExecuteType.scalar, policies);
            info.ReturnType = typeof(object);

            info.ToDBExecutor = () => _sqlExecutor.Scalar(_db, info.Sql, info.SqlParams);
            //sql执行的策略器执行
            _sqlConfigMgr.PolicyMgr.InvokeExecutePolicyExecutors(policies, info);

            //判断是否结束
            if (info.IsEnd)
            {
                return info.ReturnValue;
            }
            else
            {
                //如果sql执行的策略器执行后没有设定IsEnd，那么直接执行sql
                return _sqlExecutor.Scalar(_db, info.Sql, info.SqlParams);
            }
        }

        public int NonQueryUseDict(IDictionary<string, object> parameters,
            IDictionary<string, ISqlConfigPolicy> policies = null)
        {
            var info = RunLua(parameters, ConfigSqlExecuteType.nonquery, policies);
            info.ReturnType = typeof(int);

            info.ToDBExecutor = () => _sqlExecutor.NonQuery(_db, info.Sql, info.SqlParams);
            //sql执行的策略器执行
            _sqlConfigMgr.PolicyMgr.InvokeExecutePolicyExecutors(policies, info);

            //判断是否结束
            if (info.IsEnd)
            {
                return (int)info.ReturnValue;
            }
            else
            {
                //如果sql执行的策略器执行后没有设定IsEnd，那么直接执行sql
                return _sqlExecutor.NonQuery(_db, info.Sql, info.SqlParams);
            }
        }

        protected LuaSqlPolicyExecutorInfo RunLua(IDictionary<string, object> parameters, 
            ConfigSqlExecuteType exeType,
            IDictionary<string, ISqlConfigPolicy> policies)
        {
            var info = new LuaSqlPolicyExecutorInfo()
            {
                Config = _sqlConfigMgr.Config,
                DB = DB,
                SqlInfo = _sqlInfo,
                SqlName = SqlName,
                TableInfo = _tableInfo,
                TableName = TableName,
                ExecuteType = exeType,
                PreSqlParams = parameters,
            };

            //lua sql执行前的调用
            _sqlConfigMgr.PolicyMgr.InvokePreExecutePolicyExecutors(policies, info);

            var guid = Guid.NewGuid().ToString();
            _luaSqlParamFuncContainer[guid] = info.LuaSqlParamFuncs;    //设置SqlParams操作相关的函数到容器中
            //调用脚本函数生成sql
            var srtn = _sqlConfigMgr.Config.Run(TableName, SqlName, guid);
            _luaSqlParamFuncContainer.Remove(guid); //脚本运行完成后移除
            
            CheckSqlExecuteType(srtn.Type, exeType);
            info.LuaRan?.Invoke();  //触发lua运行之后的事件

            info.Sql = srtn.Sql.Trim();   //sql, 去掉头尾空格
            info.SqlParams = _sqlParamCvt.DictionaryToDBParams(DB, info.PreSqlParams);

            return info;
        }

        protected void CheckSqlExecuteType(ConfigSqlExecuteType sqlType, ConfigSqlExecuteType type)
        {
            //如果为NotSure 或 当前执行的类型 那么就通过
            if (!(sqlType == ConfigSqlExecuteType.notsure || sqlType == type))
            {
                throw new ArgumentException($"The sql type [{_util.GetEnumDescription(sqlType, true)}] is not executing type [{_util.GetEnumDescription(type, true)}].");
            }
        }

    }
}
