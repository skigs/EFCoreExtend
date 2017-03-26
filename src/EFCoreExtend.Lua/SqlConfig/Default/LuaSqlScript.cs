using EFCoreExtend.Sql.SqlConfig;
using MoonSharp.Interpreter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Default
{
    public class LuaSqlScript
    {
        Script _script;
        public Script Script => _script;
        Table _config;
        public Table Gonfig => _config;
        Table _configGlobal;
        public Table ConfigGlobal => _configGlobal;
        Table _sqlTables;
        public Table SqlTables => _sqlTables;
        readonly ConcurrentDictionary<string, LuaSqlModel> _luasqls = new ConcurrentDictionary<string, LuaSqlModel>();
        public ConcurrentDictionary<string, LuaSqlModel> Luasqls => _luasqls;
        public event Func<LuaSqlScript, string, LuaSqlModel> AddOrGetLuaSql;

        public LuaSqlScript()
        {
            ResetScript();
        }

        public void ResetScript()
        {
            _script?.Globals?.Clear();
            _config?.Clear();
            _sqlTables?.Clear();
            _luasqls.Clear();

            _script = null;
            _config = null;
            _configGlobal = null;
            _sqlTables = null;

            _script = new Script();
            LuaConfigInit(_script);
        }

        protected void LuaConfigInit(Script script)
        {
            //初始化默认配置操作对象
            script.Globals[LuaSqlConfigConst.SqlConfigLabel] = new Dictionary<string, object>();
            _config = (Table)script.Globals[LuaSqlConfigConst.SqlConfigLabel];
            LuaConfigGlobalInit();

            _config[LuaSqlConfigConst.SqlConfigTablesLabel] = new Dictionary<string, object>();
            _sqlTables = (Table)_config[LuaSqlConfigConst.SqlConfigTablesLabel];

            _config[LuaSqlConfigConst.SqlConfigGetTableLabel] = (Func<string, Table>)(tname =>
            {
                //获取指定表的配置
                return AddOrGetLuaSql(this, tname).Config;
            });
            //配置指定表的配置
            script.DoString($@"
                function {LuaSqlConfigConst.SqlConfigLabel}.{LuaSqlConfigConst.SqlConfigConfigTableLabel}(tname, cfgFunc)
                    cfgFunc(cfg.{LuaSqlConfigConst.SqlConfigGetTableLabel}(tname));
                end
            ");
        }

        void LuaConfigGlobalInit()
        {
            _config[LuaSqlConfigConst.SqlConfigGlobalLabel] = new Dictionary<string, object>();
            _configGlobal = (Table)_config[LuaSqlConfigConst.SqlConfigGlobalLabel];
            _configGlobal[LuaSqlConfigConst.TablePoliciesLabel] = new Dictionary<string, object>();
            _configGlobal[LuaSqlConfigConst.SqlPoliciesLabel] = new Dictionary<string, object>();
        }
        
    }
}
