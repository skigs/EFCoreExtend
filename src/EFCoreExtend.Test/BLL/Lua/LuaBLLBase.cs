using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public abstract class LuaBLLBase<T>
        where T : class
    {
        protected readonly LuaDBConfigTable config;
        public LuaBLLBase(DbContext db)
        {
            config = db.GetLuaConfigTable<T>();
        }

        public LuaBLLBase(DbContext db, string tableName)
        {
            config = db.GetLuaConfigTable(tableName);
        }

    }
}
