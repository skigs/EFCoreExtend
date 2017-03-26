using EFCoreExtend.Sql.SqlConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public class DBConfigTable : IDBConfigTable, IDisposable
    {
        ISqlConfig _config;
        IConfigTableInfo _tableInfo;
        public IConfigTableInfo TableInfo
        {
            get
            {
                if (_tableInfo == null)
                {
                    _tableInfo = _config.TableSqlInfos[TableName];
                }
                return _tableInfo;
            }
        }
        public DbContext DB { get; }
        public string TableName { get; }

        public DBConfigTable(ISqlConfig config, string tableName, DbContext db)
        {
            config.CheckNull(nameof(config));
            tableName.CheckStringIsNullOrEmpty(nameof(tableName));
            db.CheckNull(nameof(db));

            _config = config;
            DB = db;
            TableName = tableName;

        }

        public void Dispose()
        {
            DB.Dispose();
        }

    }
}
