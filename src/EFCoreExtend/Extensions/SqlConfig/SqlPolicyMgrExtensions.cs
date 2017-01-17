using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class SqlPolicyMgrExtensions
    {
        #region 设置全局策略对象

        /// <summary>
        /// 设置全局策略对象
        /// </summary>
        public static void SetGlobalPolicy(this ISqlPolicyManager mgr, ISqlConfigPolicy policy)
        {
            policy.CheckNull(nameof(policy));
            var policyName = EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(policy.GetType());
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException($"Could not find {nameof(SqlConfigPolicyAttribute)} in {policy.GetType().Name} type");
            }
            mgr.GlobalPolicies[policyName] = policy;
        }

        /// <summary>
        /// 设置全局策略对象
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="policyName"></param>
        /// <param name="policy"></param>
        public static void SetGlobalPolicy(this ISqlPolicyManager mgr, string policyName, ISqlConfigPolicy policy)
        {
            policy.CheckNull(nameof(policy));
            policyName.CheckStringIsNullOrEmpty(nameof(policyName));
            mgr.GlobalPolicies[policyName] = policy;
        } 

        #endregion

        #region 设置策略类型
        /// <summary>
        /// 设置策略类型
        /// </summary>
        public static void SetPolicyType<T>(this ISqlPolicyManager mgr)
            where T : ISqlConfigPolicy
        {
            mgr.SetPolicyType(typeof(T));
        }

        /// <summary>
        /// 设置策略类型
        /// </summary>
        public static void SetPolicyType<T>(this ISqlPolicyManager mgr, string policyName)
            where T : ISqlConfigPolicy
        {
            mgr.SetPolicyType(policyName, typeof(T));
        }

        /// <summary>
        /// 设置策略类型
        /// </summary>
        public static void SetPolicyType(this ISqlPolicyManager mgr, Type type)
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
        public static void SetPolicyType(this ISqlPolicyManager mgr, string policyName, Type type)
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
        /// 设置用于初始化的策略执行器，就是在程序运行期间只执行一次，除非sql的配置数据发生了改变（例如：替换表名 / 合并分部sql等的执行器）
        /// </summary>
        public static void SetInitPolicyExecutor(this ISqlPolicyManager mgr, string policyName,
            Func<ISqlInitPolicyExecutor> getExecutorFunc, int priority = 0)
        {
            mgr.SetExecutor(policyName, getExecutorFunc, priority);
        }

        /// <summary>
        /// 设置用于初始化的策略执行器，就是在程序运行期间只执行一次，除非sql的配置数据发生了改变（例如：替换表名 / 合并分部sql等的执行器）
        /// </summary>
        public static void SetInitPolicyExecutor<T>(this ISqlPolicyManager mgr,
            Func<T> getExecutorFunc, int priority = 0)
            where T : ISqlInitPolicyExecutor
        {
            var policyName = EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(typeof(T));
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException($"Could not find {nameof(SqlConfigPolicyAttribute)} in {typeof(T).Name} type");
            }
            mgr.SetExecutor(policyName, getExecutorFunc as Func<ISqlInitPolicyExecutor>, priority);
        }

        /// <summary>
        /// 设置用于在sql执行前的策略执行器（例如：foreach执行器对某些数据类型（list/dict等等）进行生成字串替换到sql中）
        /// </summary>
        public static void SetPreExecutePolicyExecutor(this ISqlPolicyManager mgr, string policyName,
            Func<ISqlPreExecutePolicyExecutor> getExecutorFunc, int priority = 0)
        {
            mgr.SetExecutor(policyName, getExecutorFunc, priority);
        }

        /// <summary>
        /// 设置用于在sql执行前的策略执行器（例如：foreach执行器对某些数据类型（list/dict等等）进行生成字串替换到sql中）
        /// </summary>
        public static void SetPreExecutePolicyExecutor<T>(this ISqlPolicyManager mgr,
            Func<T> getExecutorFunc, int priority = 0)
            where T : ISqlPreExecutePolicyExecutor
        {
            var policyName = EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(typeof(T));
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException($"Could not find {nameof(SqlConfigPolicyAttribute)} in {typeof(T).Name} type");
            }
            mgr.SetExecutor(policyName, getExecutorFunc as Func<ISqlPreExecutePolicyExecutor>, priority);
        }

        /// <summary>
        /// 设置用于sql执行时的策略执行器（例如：查询缓存（一级/二级的），查询缓存清理（NonQuery的执行之后对二级缓存的清理）等等）
        /// </summary>
        public static void SetExecutePolicyExecutor(this ISqlPolicyManager mgr, string policyName,
            Func<ISqlExecutePolicyExecutor> getExecutorFunc, int priority = 0)
        {
            mgr.SetExecutor(policyName, getExecutorFunc, priority);
        }

        /// <summary>
        /// 设置用于sql执行时的策略执行器（例如：查询缓存（一级/二级的），查询缓存清理（NonQuery的执行之后对二级缓存的清理）等等）
        /// </summary>
        public static void SetExecutePolicyExecutor<T>(this ISqlPolicyManager mgr,
            Func<T> getExecutorFunc, int priority = 0)
            where T : ISqlExecutePolicyExecutor
        {
            var policyName = EFHelper.Services.EFCoreExUtility.GetSqlConfigPolicyName(typeof(T));
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException($"Could not find {nameof(SqlConfigPolicyAttribute)} in {typeof(T).Name} type");
            }
            mgr.SetExecutor(policyName, getExecutorFunc as Func<ISqlExecutePolicyExecutor>, priority);
        }

        /// <summary>
        /// 设置sql日志记录策略执行器
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="doLog">Action中的参数一：TableName；参数二：SqlName；参数三：Sql；参数四：SqlParameters</param>
        /// <param name="isAddLogGlobalPolicyObj">是否添加全局的策略对象</param>
        /// <param name="isAsync">是否异步记录</param>
        public static void SetSqlConfigExecuteLogPolicyExecutor(this ISqlPolicyManager mgr, Action<string, string,
            string, IReadOnlyList<IDataParameter>> doLog, bool isAddLogGlobalPolicyObj = true, 
            bool isAsync = true)
        {
            var logExc = new SqlConfigExecuteLogPolicyExecutor(doLog);
            mgr.SetExecutePolicyExecutor(() => logExc);
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
        public static void InvokeInitPolicyExecutors(this ISqlPolicyManager mgr,
           IReadOnlyDictionary<string, ISqlConfigPolicy> policies, ISqlInitPolicyExecutorInfo info)
        {
            mgr.InvokeExecutors(policies, info);
        }

        /// <summary>
        /// 在sql执行前的策略执行器的调用
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="policies"></param>
        /// <param name="info"></param>
        public static void InvokePreExecutePolicyExecutors(this ISqlPolicyManager mgr,
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies, ISqlPreExecutePolicyExecutorInfo info)
        {
            mgr.InvokeExecutors(policies, info);
        }

        /// <summary>
        /// sql执行时的策略执行器的调用
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="policies"></param>
        /// <param name="info"></param>
        public static void InvokeExecutePolicyExecutors(this ISqlPolicyManager mgr,
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies, ISqlExecutePolicyExecutorInfo info)
        {
            mgr.InvokeExecutors(policies, info);
        } 
        #endregion


    }
}
