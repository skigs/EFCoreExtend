using EFCoreExtend.Lua;
using EFCoreExtend.Lua.SqlConfig;
using EFCoreExtend.Lua.SqlConfig.Policies;
using EFCoreExtend.Lua.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class LuaSqlPolicyMgrExtensions
    {
        #region 设置全局策略对象

        /// <summary>
        /// 设置全局策略对象
        /// </summary>
        public static void SetGlobalPolicy(this ILuaSqlConfigManager mgr, ISqlConfigPolicy policy)
        {
            policy.CheckNull(nameof(policy));
            var policyName = EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(policy.GetType());
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException($"Could not find {nameof(SqlConfigPolicyAttribute)} in {policy.GetType().Name} type");
            }
            mgr.PolicyMgr.GlobalPolicies[policyName] = policy;
        }

        /// <summary>
        /// 设置全局策略对象
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="policyName"></param>
        /// <param name="policy"></param>
        public static void SetGlobalPolicy(this ILuaSqlConfigManager mgr, string policyName, ISqlConfigPolicy policy)
        {
            policy.CheckNull(nameof(policy));
            policyName.CheckStringIsNullOrEmpty(nameof(policyName));
            mgr.PolicyMgr.GlobalPolicies[policyName] = policy;
        }

        #endregion

        #region 设置策略类型
        /// <summary>
        /// 设置策略类型
        /// </summary>
        public static void SetPolicyType<T>(this ILuaSqlPolicyManager mgr)
            where T : ISqlConfigPolicy
        {
            mgr.SetPolicyType(typeof(T));
        }

        /// <summary>
        /// 设置策略类型
        /// </summary>
        public static void SetPolicyType<T>(this ILuaSqlPolicyManager mgr, string policyName)
            where T : ISqlConfigPolicy
        {
            mgr.SetPolicyType(policyName, typeof(T));
        }

        /// <summary>
        /// 设置策略类型
        /// </summary>
        public static void SetPolicyType(this ILuaSqlPolicyManager mgr, Type type)
        {
            if (!typeof(ISqlConfigPolicy).IsAssignableFrom(type))
            {
                throw new ArgumentException($"The type [{type.Name}] must implement {nameof(ISqlConfigPolicy)}", nameof(type));
            }
            var policyName = EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(type);
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException($"Could not find {nameof(SqlConfigPolicyAttribute)} in type [{type.Name}]", nameof(type));
            }
            mgr.PolicyTypes[policyName] = type;
        }

        /// <summary>
        /// 设置策略类型
        /// </summary>
        public static void SetPolicyType(this ILuaSqlPolicyManager mgr, string policyName, Type type)
        {
            if (!typeof(ISqlConfigPolicy).IsAssignableFrom(type))
            {
                throw new ArgumentException($"The type [{type.Name}] must implement {nameof(ISqlConfigPolicy)}", nameof(type));
            }
            mgr.PolicyTypes[policyName] = type;
        }
        #endregion

        #region 设置策略执行器

        /// <summary>
        /// 设置用于初始化的策略执行器，就是在程序运行期间只执行一次，除非sql的配置数据发生了改变
        /// </summary>
        public static void SetInitPolicyExecutor(this ILuaSqlPolicyManager mgr, string policyName,
            Func<ILuaSqlInitPolicyExecutor> getExecutorFunc, int priority = 0)
        {
            mgr.SetExecutor(policyName, getExecutorFunc, priority);
        }

        /// <summary>
        /// 设置用于初始化的策略执行器，就是在程序运行期间只执行一次，除非sql的配置数据发生了改变
        /// </summary>
        public static void SetInitPolicyExecutor<T>(this ILuaSqlPolicyManager mgr,
            Func<T> getExecutorFunc, int priority = 0)
            where T : ILuaSqlInitPolicyExecutor
        {
            var policyName = EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(typeof(T));
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException($"Could not find {nameof(SqlConfigPolicyAttribute)} in {typeof(T).Name} type");
            }
            mgr.SetExecutor(policyName, 
                getExecutorFunc as Func<ILuaSqlInitPolicyExecutor>, priority);
        }

        /// <summary>
        /// 用于在lua sql执行前的策略执行器（例如：生成相关的lua函数的参数）
        /// </summary>
        public static void SetSqlPreExecutePolicyExecutor(this ILuaSqlPolicyManager mgr, string policyName,
            Func<ILuaSqlPreExecutePolicyExecutor> getExecutorFunc, int priority = 0)
        {
            mgr.SetExecutor(policyName, getExecutorFunc, priority);
        }

        /// <summary>
        /// 用于在lua sql执行前的策略执行器（例如：生成相关的lua函数的参数）
        /// </summary>
        public static void SetSqlPreExecutePolicyExecutor<T>(this ILuaSqlPolicyManager mgr,
            Func<T> getExecutorFunc, int priority = 0)
            where T : ILuaSqlPreExecutePolicyExecutor
        {
            var policyName = EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(typeof(T));
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException($"Could not find {nameof(SqlConfigPolicyAttribute)} in {typeof(T).Name} type");
            }
            mgr.SetExecutor(policyName, getExecutorFunc as Func<ILuaSqlPreExecutePolicyExecutor>, priority);
        }

        /// <summary>
        /// 设置用于sql执行时的策略执行器（例如：查询缓存，查询缓存清理 等等）
        /// </summary>
        public static void SetSqlExecutePolicyExecutor(this ILuaSqlPolicyManager mgr, string policyName,
            Func<ILuaSqlExecutePolicyExecutor> getExecutorFunc, int priority = 0)
        {
            mgr.SetExecutor(policyName, getExecutorFunc, priority);
        }

        /// <summary>
        /// 设置用于sql执行时的策略执行器（例如：查询缓存，查询缓存清理 等等）
        /// </summary>
        public static void SetSqlExecutePolicyExecutor<T>(this ILuaSqlPolicyManager mgr,
            Func<T> getExecutorFunc, int priority = 0)
            where T : ILuaSqlExecutePolicyExecutor
        {
            var policyName = EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(typeof(T));
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException($"Could not find {nameof(SqlConfigPolicyAttribute)} in {typeof(T).Name} type");
            }
            mgr.SetExecutor(policyName, getExecutorFunc as Func<ILuaSqlExecutePolicyExecutor>, priority);
        }

        /// <summary>
        /// 设置sql日志记录策略执行器
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="doLog">Action中的参数一：TableName；参数二：SqlName；参数三：Sql；参数四：SqlParameters</param>
        /// <param name="isAddLogGlobalPolicyObj">是否添加全局的策略对象</param>
        /// <param name="isAsync">是否异步记录</param>
        public static void SetLogPolicyExecutor(this ILuaSqlConfigManager mgr, Action<string, string,
            string, IReadOnlyList<IDataParameter>> doLog, bool isAddLogGlobalPolicyObj = true,
            bool isAsync = true)
        {
            var logExc = new LuaSqlExecuteLogPolicyExecutor(doLog);
            mgr.PolicyMgr.SetSqlExecutePolicyExecutor(() => logExc);
            if (isAddLogGlobalPolicyObj)
            {
                var policy = new SqlConfigExecuteLogPolicy()
                {
                    IsAsync = isAsync,
                };
                mgr.SetGlobalPolicy(policy);
            }
        }

        #endregion

        #region 策略执行器的调用

        /// <summary>
        /// 初始化的策略执行器的调用
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="policies"></param>
        /// <param name="info"></param>
        public static void InvokeInitPolicyExecutors<T>(this ILuaSqlPolicyManager mgr,
           IDictionary<string, ISqlConfigPolicy> policies, T info)
            where T : ILuaSqlPolicyExecutorInfo, ILuaSqlInitPolicyExecutorInfo
        {
            mgr.InvokeExecutors(policies, info, typeof(ILuaSqlInitPolicyExecutorInfo));
        }

        /// <summary>
        /// sql执行前的策略执行器的调用
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="policies"></param>
        /// <param name="info"></param>
        public static void InvokePreExecutePolicyExecutors<T>(this ILuaSqlPolicyManager mgr,
            IDictionary<string, ISqlConfigPolicy> policies, T info)
            where T : ILuaSqlPolicyExecutorInfo, ILuaSqlPreExecutePolicyExecutorInfo
        {
            mgr.InvokeExecutors(policies, info, typeof(ILuaSqlPreExecutePolicyExecutorInfo));
        }

        /// <summary>
        /// sql执行时的策略执行器的调用
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="policies"></param>
        /// <param name="info"></param>
        public static void InvokeExecutePolicyExecutors<T>(this ILuaSqlPolicyManager mgr,
            IDictionary<string, ISqlConfigPolicy> policies, T info)
            where T : ILuaSqlPolicyExecutorInfo, ILuaSqlExecutePolicyExecutorInfo
        {
            mgr.InvokeExecutors(policies, info, typeof(ILuaSqlExecutePolicyExecutorInfo));
        }

        #endregion

    }
}
