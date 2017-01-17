using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EFCoreExtend.EFCache;
using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig.Policies;
using System.Collections;
using System.Reflection;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default;
using System.Collections.Concurrent;

namespace EFCoreExtend.Sql.SqlConfig.Executors.Default
{
    public class SqlConfigExecutor : ISqlConfigExecutor
    {
        #region 数据字段
        protected readonly string _tableName;
        public string TableName => _tableName;

        protected readonly string _sqlName;
        public string SqlName => _sqlName;

        protected readonly IConfigSqlInfo _sqlInfo;
        public IConfigSqlInfo SqlInfo => _sqlInfo;

        protected readonly DbContext _db;
        public DbContext DB => _db;

        protected readonly string _sql;
        protected readonly ISqlExecutor _sqlExecutor;
        protected readonly ISqlParamConverter _sqlParamCvt;
        protected readonly ISqlConfigManager _sqlConfigMgr;
        protected readonly IConfigTableInfo _tableInfo;
        protected readonly IEFCoreExtendUtility _util;
        protected readonly IDictionary<string, object> _tempDatas = new ConcurrentDictionary<string, object>();
        #endregion

        public SqlConfigExecutor(ISqlConfigManager sqlConfigMgr, DbContext db,
            string tableName, string sqlName, IConfigSqlInfo sqlInfo, IConfigTableInfo tableInfo, string sql,
            ISqlExecutor sqlExecutor, ISqlParamConverter sqlParamCvt, IEFCoreExtendUtility util)
        {
            _sqlConfigMgr = sqlConfigMgr;
            _db = db;
            _sqlInfo = sqlInfo;
            _tableName = tableName;
            _sqlName = sqlName;
            _tableInfo = tableInfo;
            _sql = sql;

            _sqlExecutor = sqlExecutor;
            _sqlParamCvt = sqlParamCvt;
            _util = util;

        }

        protected PolicyExecutorInfo GetPolicyExecutorInfo()
        {
            return new PolicyExecutorInfo()
            {
                TableSqlInfos = _sqlConfigMgr.Config.TableSqlInfos,
                DB = DB,
                Sql = _sql,
                SqlInfo = SqlInfo,
                SqlName = SqlName,
                TableInfo = _tableInfo,
                TableName = TableName,
                SqlExecutorTempDatas = _tempDatas,
            };
        }

        public IReadOnlyList<T> Query<T>(IDataParameter[] parameters = null,
            IReadOnlyCollection<string> ignoreProptsForRtnType = null, 
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies = null) where T : new()
        {
            CheckSqlExecuteType(_sqlInfo, ConfigSqlExecuteType.query);

            var info = GetPolicyExecutorInfo();
            info.ExecuteType = ConfigSqlExecuteType.query;
            info.SqlParams = parameters;

            //sql执行之前的策略器执行
            _sqlConfigMgr.PolicyMgr.InvokePreExecutePolicyExecutors(policies, info);

            info.Sql = info.Sql.Trim(); //去掉头尾空格
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

        public object Scalar(IDataParameter[] parameters = null, 
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies = null)
        {
            CheckSqlExecuteType(_sqlInfo, ConfigSqlExecuteType.scalar);

            var info = GetPolicyExecutorInfo();
            info.ExecuteType = ConfigSqlExecuteType.scalar;
            info.SqlParams = parameters;

            //sql执行之前的策略器执行
            _sqlConfigMgr.PolicyMgr.InvokePreExecutePolicyExecutors(policies, info);

            info.Sql = info.Sql.Trim(); //去掉头尾空格
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

        public int NonQuery(IDataParameter[] parameters = null, IReadOnlyDictionary<string, ISqlConfigPolicy> policies = null)
        {
            CheckSqlExecuteType(_sqlInfo, ConfigSqlExecuteType.nonquery);

            var info = GetPolicyExecutorInfo();
            info.ExecuteType = ConfigSqlExecuteType.nonquery;
            info.SqlParams = parameters;

            //sql执行之前的策略器执行
            _sqlConfigMgr.PolicyMgr.InvokePreExecutePolicyExecutors(policies, info);

            info.Sql = info.Sql.Trim(); //去掉头尾空格
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

        protected void CheckSqlExecuteType(IConfigSqlInfo sqlInfo, ConfigSqlExecuteType type)
        {
            //如果为NotSure 或 当前执行的类型 那么就通过
            if (!(sqlInfo.Type == ConfigSqlExecuteType.notsure || sqlInfo.Type == type))
            {
                throw new ArgumentException($"The sql type [{_util.GetEnumDescription(sqlInfo.Type, true)}] is not executing type [{_util.GetEnumDescription(type, true)}].");
            }
        }

    }
}
