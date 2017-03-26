using EFCoreExtend.Lua.SqlConfig;
using EFCoreExtend.Lua.SqlConfig.Default;
using EFCoreExtend.Lua.SqlConfig.Policies;
using EFCoreExtend.Lua.SqlConfig.Policies.Default;
using EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs;
using EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs.Default;
using EFCoreExtend.ServicesConfiguration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class EFCoreExtendLuaServicesExtensions
    {
        static volatile bool _isAddBuildCallback = false;
        static ILuaSqlConfigManager _luamgr;

        /// <summary>
        /// 设置Lua Sql的相关服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="luasqlScriptCount">Lua脚本解析器的实例对象个数(因为Lua脚本解析器实例对象不支持多线程，因此实例只能被一个线程操作，因此多线程使用就得多个实例对象，但也不能设置过多，设置过多占内存就越多)</param>
        /// <returns></returns>
        public static IEFCoreExtendServiceBuilder AddLuaSqlDefault(this IEFCoreExtendServiceBuilder builder,
            int luasqlScriptCount = 10)
        {
            builder.Services.AddScoped<ILuaSqlConfigManager, LuaSqlConfigManager>();
            builder.Services.AddScoped<ILuaSqlPolicyManager, LuaSqlPolicyManager>();
            builder.Services.AddScoped<ILuaFuncManager, LuaFuncManager>();
            var config = new LuaSqlConfig(luasqlScriptCount);
            builder.Services.AddScoped<ILuaSqlConfig>(sp => config);

            if (!_isAddBuildCallback)
            {
                _isAddBuildCallback = true;
                EFHelper.ServiceBuiltCallback += p =>
                {
                    _luamgr = p.Provider.GetService<ILuaSqlConfigManager>();
                };
            }

            return builder;
        }

        /// <summary>
        /// 获取lua sql管理器
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static ILuaSqlConfigManager GetLuaSqlMgr(this IEFCoreExtendServiceProvider service)
        {
            //return service.Provider.GetService<ILuaSqlConfigManager>();
            //在扩展服务重新编译之后进行了赋值，不需要每次都调用GetService获取，提高些许性能，因为GetLuaSqlMgr可能会被大量调用的
            return _luamgr;
        }

    }
}
