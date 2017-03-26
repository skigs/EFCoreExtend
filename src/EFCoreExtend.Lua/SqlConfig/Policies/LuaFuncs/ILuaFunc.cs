using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs
{
    /// <summary>
    /// lua func
    /// </summary>
    public interface ILuaFunc
    {
        /// <summary>
        /// 获取Lua Func
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        LuaFuncInfo GetFunc(ILuaSqlInitPolicyExecutorInfo info);

        /// <summary>
        /// 设置Lua Func
        /// </summary>
        /// <param name="luaparams"></param>
        /// <param name="sqlparams"></param>
        /// <param name="premove"></param>
        /// <param name="pnew"></param>
        void SetFunc(IDictionary<string, object> luaparams, IDictionary<string, object> sqlparams,
            ICollection<string> premove, IDictionary<string, object> pnew);
    }

    public class LuaFuncInfo
    {
        public object Func { get; set; }
        public string Type { get; set; }
    }

}
