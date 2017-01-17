using EFCoreExtend.EFCache;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Executors
{
    public interface ISqlConfigExecutorCreator
    {
        ISqlConfigExecutor Create(ISqlConfigManager sqlConfigMgr, DbContext db,
            string tableName, string sqlName, IConfigSqlInfo sqlInfo, IConfigTableInfo tableInfo, string sql);
    }
}
