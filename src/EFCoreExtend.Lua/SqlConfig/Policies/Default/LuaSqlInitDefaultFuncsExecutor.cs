using EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs;
using EFCoreExtend.Sql.SqlConfig.Policies;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.Default
{
    /// <summary>
    /// 用于处理SqlParameter相关的函数初始化，
    /// 以保证默认全局参数的初始化，这个策略执行器没有策略类(默认优先级为：int.MaxValue - 1)
    /// </summary>
    [SqlConfigPolicy("LuaSqlDefaultFuncsInit")]
    public class LuaSqlInitDefaultFuncsExecutor : ILuaSqlInitPolicyExecutor
    {
        protected readonly ILuaFuncManager _luafuncs;

        public LuaSqlInitDefaultFuncsExecutor(ILuaFuncManager luafuncs)
        {
            luafuncs.CheckNull(nameof(luafuncs));
            _luafuncs = luafuncs;
        }

        public void Execute(ILuaSqlInitPolicyExecutorInfo info)
        {
            var funcs =  _luafuncs.GetFuncs();
            foreach (var func in funcs)
            {
                var finfo = func.GetFunc(info);
                SetFunc2Config(info, finfo.Type, finfo.Func);
            }
        }

        protected void SetFunc2Config(ILuaSqlInitPolicyExecutorInfo info, string parameterName, object func)
        {
            var ps = info.Config.GetGlobalLuaParam(LuaSqlConfigConst.SqlConfigLabel);
            foreach (var p in ps)
            {
                var cfg = (Table)p;
                cfg[parameterName] = func;
            }
        }

    }
}
