using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs
{
    /// <summary>
    /// lua func类管理器
    /// </summary>
    public interface ILuaFuncManager
    {
        IReadOnlyList<ILuaFunc> GetFuncs();
    }
}
