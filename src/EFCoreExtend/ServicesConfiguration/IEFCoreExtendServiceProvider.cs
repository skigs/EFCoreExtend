using EFCoreExtend.Commons;
using EFCoreExtend.EFCache;
using EFCoreExtend.Sql;
using EFCoreExtend.Sql.SqlConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.ServicesConfiguration
{
    public interface IEFCoreExtendServiceProvider
    {
        IServiceProvider Provider { get; }

        /// <summary>
        /// EF缓存器(查询缓存：IQueryable的查询缓存、配置SQL的二级查询缓存都是使用这个接口进行缓存的)
        /// </summary>
        IEFQueryCache Cache { get; }
        /// <summary>
        /// sql执行器
        /// </summary>
        ISqlExecutor SqlExecutor { get; }
        /// <summary>
        /// sql配置管理
        /// </summary>
        ISqlConfigManager SqlConfigMgr { get; }
        /// <summary>
        /// sql的参数转换器
        /// </summary>
        ISqlParamConverter SqlParamConverter { get; }
        /// <summary>
        /// 反射帮助类
        /// </summary>
        IObjectReflector ObjReflector { get; }
        /// <summary>
        /// EFCoreExtend的工具类
        /// </summary>
        IEFCoreExtendUtility EFCoreExUtility { get; }

        /// <summary>
        /// 是否使用EFQueryCache
        /// </summary>
        bool IsUseCache { get; set; }

    }
}
