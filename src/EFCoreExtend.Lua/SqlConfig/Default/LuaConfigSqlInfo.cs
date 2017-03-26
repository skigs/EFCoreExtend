using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Default
{
    public class LuaConfigSqlInfo : ILuaConfigSqlInfo
    {
        ConcurrentDictionary<string, object> _policies = new ConcurrentDictionary<string, object>();
        public IDictionary<string, object> Policies => _policies;
    }
}
