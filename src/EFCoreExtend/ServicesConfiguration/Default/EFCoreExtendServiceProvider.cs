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

        public IEFQueryCache Cache { get; }
        public ISqlExecutor SqlExecutor { get; }
        public ISqlConfigManager SqlConfigMgr { get; }
        public ISqlParamConverter SqlParamConverter { get; }
        public IObjectReflector ObjReflector { get; }
        public IEFCoreExtendUtility EFCoreExUtility { get; }

        IQueryCacheContainerMgr _queryCacheContainerMgr;
        /// <summary>
        /// 是否使用EFQueryCache
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
