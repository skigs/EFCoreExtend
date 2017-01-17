using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.ServicesConfiguration
{
    public interface IEFCoreExtendServiceBuilder
    {
        IServiceCollection Services { get; }
        /// <summary>
        /// 服务编译
        /// </summary>
        /// <returns></returns>
        IEFCoreExtendServiceProvider BuildServices();
    }
}
