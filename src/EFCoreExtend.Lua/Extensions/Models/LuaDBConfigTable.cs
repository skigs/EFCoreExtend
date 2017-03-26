using EFCoreExtend.Lua;
using EFCoreExtend.Lua.SqlConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public class LuaDBConfigTable : IDBConfigTable, IDisposable
    {
        ILuaSqlConfig _config;
        public DbContext DB { get; }
        public string TableName { get; }

        public LuaDBConfigTable(ILuaSqlConfig config, string tableName, DbContext db)
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
