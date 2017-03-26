using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Default
{
    public class LuaConfigTableInfo : ILuaConfigTableInfo
    {
        #region 数据字段

        public string Name { get; set; }

        ConcurrentDictionary<string, object> _policies = new ConcurrentDictionary<string, object>();
        public IDictionary<string, object> Policies => _policies;

        ConcurrentDictionary<string, ILuaConfigSqlInfo> _sqls = new ConcurrentDictionary<string, ILuaConfigSqlInfo>();
        public IDictionary<string, ILuaConfigSqlInfo> Sqls => _sqls;

        #endregion

        public LuaConfigTableInfo()
        {
        }

        public LuaConfigTableInfo(string name)
            : this()
        {
            name.CheckStringIsNullOrEmpty(nameof(name));

            this.Name = name;
        }

    }
}
