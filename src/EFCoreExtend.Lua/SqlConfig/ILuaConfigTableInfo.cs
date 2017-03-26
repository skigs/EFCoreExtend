using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig
{
    public interface ILuaConfigTableInfo
    {
        string Name { get; }

        IDictionary<string, object> Policies { get; }

        IDictionary<string, ILuaConfigSqlInfo> Sqls { get; }

    }
}
