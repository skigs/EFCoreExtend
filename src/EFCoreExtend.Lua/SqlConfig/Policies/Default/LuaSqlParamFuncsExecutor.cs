using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs;

namespace EFCoreExtend.Lua.SqlConfig.Policies.Default
{
    /// <summary>
    /// 用于设置处理SqlParameter相关的函数
    /// </summary>
    [SqlConfigPolicy("LuaSqlParamFuncs")]
    public class LuaSqlParamFuncsExecutor : ILuaSqlPreExecutePolicyExecutor
    {
        protected readonly ILuaFuncManager _luafuncs;

        public LuaSqlParamFuncsExecutor(ILuaFuncManager luafuncs)
        {
            luafuncs.CheckNull(nameof(luafuncs));
            _luafuncs = luafuncs;
        }

        public void Execute(ILuaSqlPreExecutePolicyExecutorInfo info)
        {
            info.PreSqlParams = info.PreSqlParams ?? new Dictionary<string, object>();

            ICollection<string> removeSqlparams;
            IDictionary<string, object> newSqlparams;
            info.LuaSqlParamFuncs = GetParams(info.PreSqlParams, out removeSqlparams, out newSqlparams);

            //lua脚本运行之后触发的事件
            info.LuaRan += () => 
            {
                //移除不需要的sqlparams
                if (removeSqlparams?.Count > 0)
                {
                    foreach (var p in removeSqlparams)
                    {
                        info.PreSqlParams.Remove(p);
                    }
                }
                //添加新增加的sqlparams
                if (newSqlparams?.Count > 0)
                {
                    foreach (var p in newSqlparams)
                    {
                        info.PreSqlParams.Add(p.Key, p.Value);
                    }
                }
            };
        }

        protected Dictionary<string, object> GetParams(IDictionary<string, object> sqlparams,
            out ICollection<string> removeSqlparams, out IDictionary<string, object> newSqlparams)
        {
            var premove = new HashSet<string>();
            var pnew = new Dictionary<string, object>();
            removeSqlparams = premove;
            newSqlparams = pnew;

            var luaparams = new Dictionary<string, object>();
            var funcs = _luafuncs.GetFuncs();
            foreach (var f in funcs)
            {
                f.SetFunc(luaparams, sqlparams, removeSqlparams, newSqlparams);
            }

            return luaparams;
        }

    }
}
