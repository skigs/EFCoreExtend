using EFCoreExtend.Sql.SqlConfig;
using MoonSharp.Interpreter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Default
{
    public abstract class LuaSqlConfigBase : ILuaSqlConfig
    {
        protected readonly ConcurrentDictionary<string, ILuaConfigTableInfo> _tableSqlInfos = 
            new ConcurrentDictionary<string, ILuaConfigTableInfo>();
        public IReadOnlyDictionary<string, ILuaConfigTableInfo> TableSqlInfos => _tableSqlInfos as IReadOnlyDictionary<string, ILuaConfigTableInfo>;

        protected IList<LuaSqlScript> _lsqlscripts;
        int _doCount = 0;

        public LuaSqlConfigBase(int luasqlScriptCount)
        {
            luasqlScriptCount = luasqlScriptCount < 1 ? 1 : luasqlScriptCount;

            _lsqlscripts = new List<LuaSqlScript>();
            for (int i = 0; i < luasqlScriptCount; i++)
            {
                var script = new LuaSqlScript();
                _lsqlscripts.Add(script);
                LuaConfigInit(script);
            }
        }

        /// <summary>
        /// 配置数据被修改了
        /// </summary>
        public event Action OnModified;

        public bool SetGlobalLuaParam(string parameterName, object parameter)
        {
            if (parameterName == LuaSqlConfigConst.SqlConfigLabel)
            {
                throw new ArgumentException($"Can not set the parameter [{parameterName}], the parameter is the default configuration.", 
                    nameof(parameterName));
            }


            foreach (var script in _lsqlscripts)
            {
                lock (script)
                {
                    script.Script.Globals[parameterName] = parameter;
                }
            }
            return true;
        }

        public IReadOnlyList<object> GetGlobalLuaParam(string parameterName)
        {
            List<object> list = new List<object>();
            foreach (var script in _lsqlscripts)
            {
                list.Add(script.Script.Globals[parameterName]);
            }
            return list;
        }

        public bool SetLuaParam(string tableName, string parameterName, object parameter)
        {
            var deflabels = DefaultCfgLabel();
            if (deflabels.Contains(parameterName))
            {
                throw new ArgumentException($"Can not set the parameter [{parameterName}], the parameter is the default configuration.", 
                    nameof(parameterName));
            }

            foreach (var script in _lsqlscripts)
            {
                var luasql = script.Luasqls[tableName];
                lock (script)
                {
                    luasql.Config[parameterName] = parameter;
                }
            }

            return true;
        }

        public IReadOnlyList<object> GetLuaParam(string tableName, string parameterName)
        {
            List<object> list = new List<object>();
            foreach (var script in _lsqlscripts)
            {
                var luasql = script.Luasqls[tableName];
                list.Add(luasql.Config[parameterName]);
            }
            return list;            
        }

        public void Add(string tableName, string luascript)
        {
            foreach (var script in _lsqlscripts)
            {
                lock (script)
                {
                    script.Script.DoString(luascript); //加载脚本
                }
            }

            OnModified?.Invoke();
        }

        public bool Remove(string tableName)
        {
            foreach (var script in _lsqlscripts)
            {
                lock (script)
                {
                    LuaSqlModel luasql;
                    bool bRtn = script.Luasqls.TryRemove(tableName, out luasql);
                    if (bRtn)
                    {
                        script.SqlTables.Remove(tableName);

                        ILuaConfigTableInfo tinfo;
                        _tableSqlInfos.TryRemove(tableName, out tinfo);
                    } 
                }
            }

            OnModified?.Invoke();
            return true;
        }

        public void Clear()
        {
            _tableSqlInfos.Clear();
            foreach (var script in _lsqlscripts)
            {
                script.ResetScript();
            }

            OnModified?.Invoke();
        }

        public LuaSqlConfigRunReturn Run(string tableName, string sqlName,
            object luaFuncParameters)
        {
            _doCount = _doCount % _lsqlscripts.Count + 1;
            var script = _lsqlscripts[_doCount - 1];
            var luasql = script.Luasqls[tableName];
            DynValue srtn;
            lock (script) 
            {
                //调用lua函数生成sql
                //luaFuncParameters为sqlkey
                //使用Call对脚本中的函数进行调用的时候并不支持多线程，会抛异常，因此需要lock
                srtn = script.Script.Call(luasql.Sqls[sqlName], luaFuncParameters, tableName);    
            }
            
            var rtn = new LuaSqlConfigRunReturn();
            if (srtn.Type == DataType.String)
            {
                rtn.Sql = srtn.String;
                rtn.Type = ConfigSqlExecuteType.notsure;
            }
            else
            {
                rtn.Sql = srtn.Tuple[0].String;
                rtn.Type = (ConfigSqlExecuteType)Enum.Parse(typeof(ConfigSqlExecuteType), srtn.Tuple[1].String, true);
            }

            return rtn;
        }

        public void Init()
        {
            foreach (var script in _lsqlscripts)
            {
                var luasqls = script.Luasqls.Values.ToList();
                lock (script)
                {
                    //script的初始化设置
                    foreach (var luasql in luasqls)
                    {
                        DoLuaSqlInit(script, luasql);
                    }
                }
            }
        }

        /// <summary>
        /// lua脚本配置初始化
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        abstract protected void LuaConfigInit(LuaSqlScript script);

        /// <summary>
        /// 脚本初始化（合并布局页等等）
        /// </summary>
        /// <param name="luasql"></param>
        abstract protected void DoLuaSqlInit(LuaSqlScript script, LuaSqlModel luasql);

        /// <summary>
        /// 获取默认的属性标签名
        /// </summary>
        /// <returns></returns>
        abstract protected IReadOnlyList<string> DefaultCfgLabel();


    }
}
