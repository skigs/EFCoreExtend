using EFCoreExtend.Commons;
using EFCoreExtend.EFCache;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Executors.Default
{
    public class SqlConfigExecutorCreator : ISqlConfigExecutorCreator
    {
        protected readonly ISqlExecutor _sqlExecutor;
        protected readonly ISqlParamConverter _sqlParamCvt;
        protected readonly IEFCoreExtendUtility _util;

        public SqlConfigExecutorCreator(ISqlExecutor sqlExecutor, 
            ISqlParamConverter sqlParamCvt, IEFCoreExtendUtility util)
        {
            sqlExecutor.CheckNull(nameof(sqlExecutor));
            sqlParamCvt.CheckNull(nameof(sqlParamCvt));
            util.CheckNull(nameof(util));

            _sqlExecutor = sqlExecutor;
            _sqlParamCvt = sqlParamCvt;
            _util = util;
        }

        public ISqlConfigExecutor Create(ISqlConfigManager sqlConfigMgr, DbContext db,
            string tableName, string sqlName, IConfigSqlInfo sqlInfo, IConfigTableInfo tableInfo, string sql)
        {
            return new SqlConfigExecutor(sqlConfigMgr, db, tableName, sqlName, sqlInfo, tableInfo, sql, _sqlExecutor, _sqlParamCvt, _util);
        }
    }
}
