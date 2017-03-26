using EFCoreExtend.ServicesConfiguration;
using EFCoreExtend.ServicesConfiguration.Default;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EFCoreExtend
{
    public static class EFHelper
    {
        static EFHelper()
        {
            ServiceBuilder = new EFCoreExtendServiceBuilder(p =>
            {
                _serviceProvider = p;
                ServiceBuiltCallback?.Invoke(p);
            });
            ServiceBuilder.AddDefaultServices().BuildServices();
        }

        /// <summary>
        /// EFCoreExtend的扩展服务生成器
        /// </summary>
        public static IEFCoreExtendServiceBuilder ServiceBuilder { get; }

        static IEFCoreExtendServiceProvider _serviceProvider;
        /// <summary>
        /// EFCoreExtend的扩展服务
        /// </summary>
        public static IEFCoreExtendServiceProvider Services => _serviceProvider;

        /// <summary>
        /// 扩展服务重新编译之后的回调
        /// </summary>
        public static event Action<IEFCoreExtendServiceProvider> ServiceBuiltCallback;

    }

}
