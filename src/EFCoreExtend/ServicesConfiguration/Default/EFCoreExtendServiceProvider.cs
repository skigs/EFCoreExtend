using EFCoreExtend.EFCache;
using EFCoreExtend.EFCache.Default;
using EFCoreExtend.Sql;
using EFCoreExtend.Sql.SqlConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using EFCoreExtend.Commons;

namespace EFCoreExtend.ServicesConfiguration.Default
{
    public class EFCoreExtendServiceProvider : IEFCoreExtendServiceProvider
    {
        public IServiceProvider Provider { get; }
        public EFCoreExtendServiceProvider(IServiceProvider provider)
        {
            provider.CheckNull(nameof(provider));

            Provider = provider;

            Cache = Provider.GetService<IEFQueryCache>();
            _queryCacheContainerMgr = Provider.GetService<IQueryCacheContainerMgr>();
            SqlExecutor = Provider.GetService<ISqlExecutor>();
            SqlParamConverter = Provider.GetService<ISqlParamConverter>();
            ObjReflector = Provider.GetService<IObjectReflector>();
            SqlConfigMgr = Provider.GetService<ISqlConfigManager>();
            EFCoreExUtility = Provider.GetService<IEFCoreExtendUtility>();
        }

        /// <summary>
        /// 查询缓存
        /// </summary>
        public IEFQueryCache Cache { get; }
        /// <summary>
        /// sql执行器
        /// </summary>
        public ISqlExecutor SqlExecutor { get; }
        /// <summary>
        /// sql配置管理器
        /// </summary>
        public ISqlConfigManager SqlConfigMgr { get; }
        /// <summary>
        /// sql参数转换器
        /// </summary>
        public ISqlParamConverter SqlParamConverter { get; }
        /// <summary>
        /// 反射帮助类
        /// </summary>
        public IObjectReflector ObjReflector { get; }
        /// <summary>
        /// 帮助工具
        /// </summary>
        public IEFCoreExtendUtility EFCoreExUtility { get; }

        IQueryCacheContainerMgr _queryCacheContainerMgr;
        /// <summary>
        /// 是否使用EF查询缓存服务
        /// </summary>
        public bool IsUseCache
        {
            get { return _queryCacheContainerMgr.IsUseCache; }
            set
            {
                _queryCacheContainerMgr.IsUseCache = value;
            }
        }

    }
}
