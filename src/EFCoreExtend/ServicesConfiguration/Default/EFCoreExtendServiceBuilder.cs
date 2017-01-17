using Microsoft.Extensions.DependencyInjection;
using System;

namespace EFCoreExtend.ServicesConfiguration.Default
{
    public class EFCoreExtendServiceBuilder : IEFCoreExtendServiceBuilder
    {
        protected IServiceScope _scope;
        protected IEFCoreExtendServiceProvider _serviceProvider;
        readonly protected Action<IEFCoreExtendServiceProvider> _buildCallback;
        public IServiceCollection Services { get; }

        public EFCoreExtendServiceBuilder(Action<IEFCoreExtendServiceProvider> buildCallback = null)
        {
            _buildCallback = buildCallback;
            Services = new ServiceCollection();
        }

        public EFCoreExtendServiceBuilder(IServiceCollection services, Action<IEFCoreExtendServiceProvider> buildCallback = null)
        {
            services.CheckNull(nameof(services));

            _buildCallback = buildCallback;
            Services = services;
        }

        /// <summary>
        /// EFCoreExtend的相关服务生成 / 重新生成
        /// </summary>
        public IEFCoreExtendServiceProvider BuildServices()
        {
            _scope?.Dispose();  //先释放资源，防止被多次编译了而造成旧的Services没进行内存释放

            _scope = Services.BuildServiceProvider().CreateScope();
            _serviceProvider = new EFCoreExtendServiceProvider(_scope.ServiceProvider);

            _buildCallback?.Invoke(_serviceProvider);

            return _serviceProvider;
        }

    }
}
