using EFCoreExtend.Sql.SqlConfig;
using MoonSharp.Interpreter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Default
{
    public class LuaSqlConfig : LuaSqlConfigBase
    {
        public LuaSqlConfig(int luasqlScriptCount = 10)
            :base(luasqlScriptCount)
        {

        }

        override protected void LuaConfigInit(LuaSqlScript script)
        {
            script.AddOrGetLuaSql += AddOrGetLuaSql;
        }

        override protected void DoLuaSqlInit(LuaSqlScript script, LuaSqlModel luasql)
        {
            //初始化布局页
            ScriptLayoutInit(script, luasql);
        }

        protected void ScriptLayoutInit(LuaSqlScript script, LuaSqlModel luasql)
        {
            var lname = GetLuaLayoutName(script, luasql);
            if (!string.IsNullOrEmpty(lname))
            {
                LuaSqlModel layoutlsql;
                if (script.Luasqls.TryGetValue(lname, out layoutlsql))
                {
                    ScriptLayoutInit(script, layoutlsql);  //先合并分部页中的layout(分布页也是可以设置layout的)
                    CombineGlobalConfig(luasql, layoutlsql);//脚本合并
                }
                else
                {
                    throw new KeyNotFoundException($"Can not find the layout [{ lname }]");
                }
            }
        }

        void CombineGlobalConfig(LuaSqlModel luasql, LuaSqlModel layoutlsql)
        {
            //配置对象的合并
            var cfg = luasql.Config;
            var lcfg = layoutlsql.Config;
            var deflabels = DefaultCfgLabel();  //默认的标签数据对象不需要在这里合并（策略对象在下面合并）
            var lothercfgs = lcfg.Pairs.Where(l => !deflabels.Contains(l.Key.String)).ToList();
            if (lothercfgs?.Count > 0)
            {
                var othercfgs = cfg.Pairs.Where(l => !deflabels.Contains(l.Key.String)).Select(l => l.Key.String).ToList();
                foreach (var pair in lothercfgs)
                {
                    if (!othercfgs.Contains(pair.Key.String))
                    {
                        //如果脚本中不存在该key，那么设置
                        cfg[pair.Key] = pair.Value;
                    }
                }
            }

            //策略对象合并
            CombineTables((Table)cfg[LuaSqlConfigConst.TablePoliciesLabel], (Table)lcfg[LuaSqlConfigConst.TablePoliciesLabel]);
            CombineTables((Table)cfg[LuaSqlConfigConst.SqlPoliciesLabel], (Table)lcfg[LuaSqlConfigConst.SqlPoliciesLabel]);
            //合并sql
            CombineTables((Table)cfg[LuaSqlConfigConst.SqlConfigSqlsLabel], (Table)lcfg[LuaSqlConfigConst.SqlConfigSqlsLabel]);

        }

        override protected IReadOnlyList<string> DefaultCfgLabel()
        {
            var labels = new List<string>();
            labels.Add(LuaSqlConfigConst.SqlConfigLabel);
            labels.Add(LuaSqlConfigConst.SqlConfigLayoutLabel);
            labels.Add(LuaSqlConfigConst.SqlTableNameLabel);
            labels.Add(LuaSqlConfigConst.SqlPoliciesLabel);
            labels.Add(LuaSqlConfigConst.TablePoliciesLabel);
            labels.Add(LuaSqlConfigConst.SqlConfigSqlsLabel);
            labels.Add(LuaSqlConfigConst.SqlConfigGlobalLabel);

            //labels.Add(LuaSqlConfigConst.SqlParamForeachFuncLabel);
            //labels.Add(LuaSqlConfigConst.SqlParamFuncLabel);
            //labels.Add(LuaSqlConfigConst.SqlParamsFuncLabel);

            return labels;
        }

        void CombineTables(Table table, Table ltable)
        {
            var tkeys = table.Keys.Select(l => l.String).ToList();
            foreach (var pair in ltable.Pairs)
            {
                if (!tkeys.Contains(pair.Key.String))
                {
                    table[pair.Key] = pair.Value;
                }
            }
        }

        protected string GetLuaLayoutName(LuaSqlScript script, LuaSqlModel luasql)
        {
            var layout = luasql.Config[LuaSqlConfigConst.SqlConfigLayoutLabel];
            var sqllay = layout?.ToString();
            if (sqllay == null)
            {
                //如果表配置没有配置layout，那么获取全局配置的
                layout = script.ConfigGlobal[LuaSqlConfigConst.SqlConfigLayoutLabel];
                sqllay = layout?.ToString();
            }
            return sqllay;
        }

        /// <summary>
        /// 获取或新增LuaSqlModel
        /// </summary>
        protected LuaSqlModel AddOrGetLuaSql(LuaSqlScript script, string tableName)
        {
            LuaSqlModel templuasql = new LuaSqlModel();
            var luasql = script.Luasqls.GetOrAdd(tableName, templuasql);

            if (luasql == templuasql)
            {
                //初始化
                luasql.TableName = tableName;
                InitLuaSql(script, luasql);
            }

            return luasql;
        }

        /// <summary>
        /// 初始化luasql对象数据
        /// </summary>
        /// <param name="luasql"></param>
        protected void InitLuaSql(LuaSqlScript script, LuaSqlModel luasql)
        {
            lock (script)
            {
                //设置表对象
                script.SqlTables[luasql.TableName] = new Dictionary<string, object>();
                //初始化表信息
                var t = (Table)script.SqlTables[luasql.TableName];
                t[LuaSqlConfigConst.SqlTableNameLabel] = luasql.TableName;
                t[LuaSqlConfigConst.SqlConfigSqlsLabel] = new Dictionary<string, object>();
                t[LuaSqlConfigConst.SqlTableNameLabel] = luasql.TableName;
                t[LuaSqlConfigConst.TablePoliciesLabel] = new Dictionary<string, object>();
                t[LuaSqlConfigConst.SqlPoliciesLabel] = new Dictionary<string, object>();

                luasql.Config = t;
                luasql.Sqls = (Table)luasql.Config[LuaSqlConfigConst.SqlConfigSqlsLabel];

                AddTableInfo(luasql);
            }
        }

        /// <summary>
        /// 添加表的配置信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="luasql"></param>
        protected void AddTableInfo(LuaSqlModel luasql)
        {
            //添加表的配置信息
            var tinfo = new LuaConfigTableInfo(luasql.TableName);
            //((IDictionary<string, ILuaConfigTableInfo>)_tableSqlInfos).Add(luasql.TableName, tinfo);
            _tableSqlInfos[luasql.TableName] = tinfo;
        }

    }
}
