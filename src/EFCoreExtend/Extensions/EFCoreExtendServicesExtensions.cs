using EFCoreExtend.Commons;
using EFCoreExtend.Commons.Default;
using EFCoreExtend.EFCache;
using EFCoreExtend.EFCache.Default;
using EFCoreExtend.Evaluators;
using EFCoreExtend.Evaluators.Default;
using EFCoreExtend.Evaluators.Default.Printer;
using EFCoreExtend.Evaluators.Default.Printer.Default;
using EFCoreExtend.ExpressionParsers;
using EFCoreExtend.ExpressionParsers.Default;
using EFCoreExtend.ServicesConfiguration;
using EFCoreExtend.Sql;
using EFCoreExtend.Sql.Default;
using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Default;
using EFCoreExtend.Sql.SqlConfig.Executors;
using EFCoreExtend.Sql.SqlConfig.Executors.Default;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EFCoreExtend
{
    public static class EFCoreExtendServicesExtensions
    {
        /// <summary>
        /// 设置默认服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEFCoreExtendServiceBuilder AddDefaultServices(this IEFCoreExtendServiceBuilder builder)
        {
            builder.Services
                //EFCache
                .AddScoped<IEFQueryCache, EFQueryCache>()
                .AddScoped<IQueryCacheCreator, QueryCacheCreator>()
                .AddScoped<IQueryCacheContainerMgr, QueryCacheContainerMgr>()
                .AddScoped<IEFExpressionParser, EFExpressionParser>()
                .AddScoped<IEvaluator, Evaluator>()
                .AddScoped<IEnumerablePrinter, EnumerablePrinter>()
                .AddScoped<IExpressionParser, ExpressionParser>()

                //Sql
                .AddScoped<ISqlParamConverter, SqlParamConverter>()
                .AddScoped<ISqlExecutor, SqlExecutor>()

                //SqlConfig
                .AddScoped<ISqlConfigManager, SqlConfigManager>()
                //  SqlConfig的sql执行器对象创造器
                .AddScoped<ISqlConfigExecutorCreator, SqlConfigExecutorCreator>()
                //  SqlConfig的策略执行器管理
                .AddScoped<ISqlPolicyManager, SqlPolicyManager>()

                //Commons
                .AddScoped<IObjectReflector, ObjectReflector>()
                .AddScoped<IEFCoreExtendUtility, EFCoreExtendUtility>()
                ;

            return builder;
        }

        /// <summary>
        /// 设置查询缓存存储类(IQueryCache)的创建器，默认设置为：QueryCacheCreator，可以设置为RedisQueryCacheCreator（将缓存存储到Redis中）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEFCoreExtendServiceBuilder AddQueryCacheCreator<T>(this IEFCoreExtendServiceBuilder builder)
            where T : class, IQueryCacheCreator
        {
            builder.Services.AddScoped<IQueryCacheCreator, T>();
            return builder;
        }

        /// <summary>
        /// 设置查询缓存存储类(IQueryCache)的创建器，默认设置为：QueryCacheCreator，可以设置为RedisQueryCacheCreator（将缓存存储到Redis中）
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEFCoreExtendServiceBuilder AddQueryCacheCreator(this IEFCoreExtendServiceBuilder builder, 
            Func<IServiceProvider, IQueryCacheCreator> implFactory)
        {
            builder.Services.AddScoped<IQueryCacheCreator>(implFactory);
            return builder;
        }

        /// <summary>
        /// 设置Expression解析器，用于将Expression中的值提取出来，默认设置为：ExpressionParser; 如果解析存在问题可以设置为：ExpressionDynamicParser(使用了DynamicInvoke，因此性能慢)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEFCoreExtendServiceBuilder AddExpressionParser<T>(this IEFCoreExtendServiceBuilder builder)
            where T : class, IExpressionParser
        {
            builder.Services.AddScoped<IExpressionParser, T>();
            return builder;
        }

        /// <summary>
        /// 设置SqlParamConverter，用于将一些数据对象转换成合适的SqlParameter，默认设置为：SqlParamConverter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEFCoreExtendServiceBuilder AddSqlParamConverter<T>(this IEFCoreExtendServiceBuilder builder)
            where T : class, ISqlParamConverter
        {
            builder.Services.AddScoped<ISqlParamConverter, T>();
            return builder;
        }

        /// <summary>
        /// 设置sql执行器，默认设置为：SqlExecutor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEFCoreExtendServiceBuilder AddSqlExecutor<T>(this IEFCoreExtendServiceBuilder builder)
            where T : class, ISqlExecutor
        {
            builder.Services.AddScoped<ISqlExecutor, T>();
            return builder;
        }

        /// <summary>
        /// 设置ObjectReflector，反射帮助类，默认设置为：ObjectReflector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEFCoreExtendServiceBuilder AddObjectReflector<T>(this IEFCoreExtendServiceBuilder builder)
            where T : class, IObjectReflector
        {
            builder.Services.AddScoped<IObjectReflector, T>();
            return builder;
        }

        /// <summary>
        /// 设置EFCoreExtendUtility，EFCoreExtend的相关工具类，默认设置为：EFCoreExtendUtility
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEFCoreExtendServiceBuilder AddEFCoreExtendUtility<T>(this IEFCoreExtendServiceBuilder builder)
            where T : class, IEFCoreExtendUtility
        {
            builder.Services.AddScoped<IEFCoreExtendUtility, T>();
            return builder;
        }

    }
}
