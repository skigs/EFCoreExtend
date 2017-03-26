using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig
{
    public interface ILuaConfigSqlInfo
    {
        IDictionary<string, object> Policies { get; }
    }
}
