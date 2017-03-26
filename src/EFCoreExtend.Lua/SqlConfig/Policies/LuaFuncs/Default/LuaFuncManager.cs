using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs.Default
{
    public class LuaFuncManager : ILuaFuncManager
    {
        protected readonly List<ILuaFunc> _funcs = new List<ILuaFunc>();
        protected readonly IObjectReflector _reflec;
        public LuaFuncManager(IObjectReflector reflec)
        {
            reflec.CheckNull(nameof(reflec));
            _reflec = reflec;

            _funcs.Add(new LuaParamsFunc());
            _funcs.Add(new LuaParamFunc());
            _funcs.Add(new LuaParamForeachFunc(reflec));
        }

        public IReadOnlyList<ILuaFunc> GetFuncs()
        {
            return _funcs;
        }
    }
}
