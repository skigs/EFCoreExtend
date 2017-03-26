using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend.Sql.SqlConfig.Policies;
using System.Collections.Concurrent;
using EFCoreExtend.Commons;

namespace EFCoreExtend.Lua.SqlConfig.Policies.Default
{
    public class LuaSqlPolicyManager : ILuaSqlPolicyManager
    {
        #region 数据对象

        IDictionary<string, ISqlConfigPolicy> _globalPolicies = new ConcurrentDictionary<string, ISqlConfigPolicy>();
        public IDictionary<string, ISqlConfigPolicy> GlobalPolicies => _globalPolicies;

        protected readonly IDictionary<string, Type> _policyTypes = new ConcurrentDictionary<string, Type>();
        public IDictionary<string, Type> PolicyTypes => _policyTypes;

        readonly protected IDictionary<string, Tuple<int, Func<ILuaSqlInitPolicyExecutor>>> _luasqlInitPolicyExecutors
            = new ConcurrentDictionary<string, Tuple<int, Func<ILuaSqlInitPolicyExecutor>>>();
        IEnumerable<KeyValuePair<string, Tuple<int, Func<ILuaSqlInitPolicyExecutor>>>> _luasqlInitPolicyExecutorsOrder;
        readonly InitAction _luasqlInitPolicyExecutorsOrderInit;

        readonly protected IDictionary<string, Tuple<int, Func<ILuaSqlPreExecutePolicyExecutor>>> _luasqlPreExecutePolicyExecutors
            = new ConcurrentDictionary<string, Tuple<int, Func<ILuaSqlPreExecutePolicyExecutor>>>();
        IEnumerable<KeyValuePair<string, Tuple<int, Func<ILuaSqlPreExecutePolicyExecutor>>>> _luasqlPreExecutePolicyExecutorsOrder;
        readonly InitAction _luasqlPreExecutePolicyExecutorsOrderInit;

        readonly protected IDictionary<string, Tuple<int, Func<ILuaSqlExecutePolicyExecutor>>> _luasqlExecutePolicyExecutors
            = new ConcurrentDictionary<string, Tuple<int, Func<ILuaSqlExecutePolicyExecutor>>>();
        IEnumerable<KeyValuePair<string, Tuple<int, Func<ILuaSqlExecutePolicyExecutor>>>> _luasqlExecutePolicyExecutorsOrder;
        readonly InitAction _luasqlExecutePolicyExecutorsOrderInit;


        readonly protected Type _tILuaSqlInitPolicyExecutor = typeof(ILuaSqlInitPolicyExecutor);
        readonly protected Type _tILuaSqlPreExecutePolicyExecutor = typeof(ILuaSqlPreExecutePolicyExecutor);
        readonly protected Type _tILuaSqlExecutePolicyExecutor = typeof(ILuaSqlExecutePolicyExecutor);

        readonly protected Type _tILuaSqlInitPolicyExecutorInfo = typeof(ILuaSqlInitPolicyExecutorInfo);
        readonly protected Type _tILuaSqlPreExecutePolicyExecutorInfo = typeof(ILuaSqlPreExecutePolicyExecutorInfo);
        readonly protected Type _tILuaSqlExecutePolicyExecutorInfo = typeof(ILuaSqlExecutePolicyExecutorInfo);

        #endregion

        public LuaSqlPolicyManager()
        {
            //对执行器的执行优先级进行排序初始化
            _luasqlInitPolicyExecutorsOrderInit = new InitAction(() =>
            {
                //优先级的值越大越先执行
                _luasqlInitPolicyExecutorsOrder = _luasqlInitPolicyExecutors.OrderByDescending(l => l.Value.Item1).ToList();
            });
            _luasqlPreExecutePolicyExecutorsOrderInit = new InitAction(() =>
            {
                _luasqlPreExecutePolicyExecutorsOrder = _luasqlPreExecutePolicyExecutors.OrderByDescending(l => l.Value.Item1).ToList();
            });
            _luasqlExecutePolicyExecutorsOrderInit = new InitAction(() =>
            {
                _luasqlExecutePolicyExecutorsOrder = _luasqlExecutePolicyExecutors.OrderByDescending(l => l.Value.Item1).ToList();
            });
        }

        /// <summary>
        /// 添加策略执行器
        /// </summary>
        /// <typeparam name="T">类型要为：</typeparam>
        /// <param name="policyName">策略名称</param>
        /// <param name="getExecutorFunc">获取执行器对象的Func</param>
        /// <param name="priority">策略执行器的执行优先级（值越大越先执行）</param>
        public void SetExecutor<T>(string policyName, Func<T> getExecutorFunc, int priority = 0)
        {
            policyName.CheckStringIsNullOrEmpty(nameof(policyName));
            getExecutorFunc.CheckNull(nameof(getExecutorFunc));
            var executorType = typeof(T);

            if (executorType == _tILuaSqlInitPolicyExecutor)
            {
                _luasqlInitPolicyExecutors[policyName] = Tuple.Create(priority, 
                    getExecutorFunc as Func<ILuaSqlInitPolicyExecutor>);
                //执行器有改动，那么重新初始化
                _luasqlInitPolicyExecutorsOrderInit.Release();
            }
            else if (typeof(T) == _tILuaSqlPreExecutePolicyExecutor)
            {
                _luasqlPreExecutePolicyExecutors[policyName] = Tuple.Create(priority, 
                    getExecutorFunc as Func<ILuaSqlPreExecutePolicyExecutor>);
                //执行器有改动，那么重新初始化
                _luasqlPreExecutePolicyExecutorsOrderInit.Release();
            }
            else if (executorType == _tILuaSqlExecutePolicyExecutor)
            {
                _luasqlExecutePolicyExecutors[policyName] = Tuple.Create(priority, 
                    getExecutorFunc as Func<ILuaSqlExecutePolicyExecutor>);
                //执行器有改动，那么重新初始化
                _luasqlExecutePolicyExecutorsOrderInit.Release();
            }
            else
            {
                throw new ArgumentException($"Invalid type [{executorType.FullName}]", nameof(executorType));
            }
        }

        /// <summary>
        /// 执行相关的策略执行器
        /// </summary>
        public void InvokeExecutors(IDictionary<string, ISqlConfigPolicy> policies, 
            ILuaSqlPolicyExecutorInfo info, Type executorInfoBaseType)
        {
            if (executorInfoBaseType == _tILuaSqlInitPolicyExecutorInfo)
            {
                //在策略执行之前进行相应的初始化操作（优先级排序）
                _luasqlInitPolicyExecutorsOrderInit.Invoke(-1);
                DoLuaSqlInitPolicyExecutors(policies, info);
            }
            else if (executorInfoBaseType == _tILuaSqlPreExecutePolicyExecutorInfo)
            {
                _luasqlPreExecutePolicyExecutorsOrderInit.Invoke(-1);
                DoLuaSqlPreExecutePolicyExecutors(policies, info);
            }
            else if (executorInfoBaseType == _tILuaSqlExecutePolicyExecutorInfo)
            {
                _luasqlExecutePolicyExecutorsOrderInit.Invoke(-1);
                DoLuaSqlExecutePolicyExecutors(policies, info);
            }
            else
            {
                throw new ArgumentException($"Invalid type [{executorInfoBaseType.FullName}]", nameof(executorInfoBaseType));
            }

        }

        /// <summary>
        /// 用于初始化的策略执行器，就是在程序运行期间只执行一次，除非sql的配置数据发生了改变
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected void DoLuaSqlInitPolicyExecutors(IDictionary<string, ISqlConfigPolicy> policies,
            ILuaSqlPolicyExecutorInfo info)
        {
            var pinfo = (ILuaSqlInitPolicyExecutorInfo)info;
            ISqlConfigPolicy policy = null;

            if (policies?.Count > 0)
            {
                foreach (var pair in _luasqlInitPolicyExecutorsOrder)
                {
                    info.PolicyName = pair.Key;
                    //全局的策略对象
                    _globalPolicies.TryGetValue(pair.Key, out policy);
                    info.GlobalPolicy = policy;

                    //获取通过方法形参传递过来的策略对象
                    policies.TryGetValue(pair.Key, out policy);
                    info.ParameterPolicy = policy;

                    pair.Value.Item2.Invoke().Execute(pinfo);
                }
            }
            else
            {
                info.ParameterPolicy = null;
                foreach (var pair in _luasqlInitPolicyExecutorsOrder)
                {
                    info.PolicyName = pair.Key;
                    //全局的策略对象
                    _globalPolicies.TryGetValue(pair.Key, out policy);
                    info.GlobalPolicy = policy;
                    pair.Value.Item2.Invoke().Execute(pinfo);
                }
            }
        }

        /// <summary>
        /// 在sql执行前的策略执行器（例如：lua函数参数的初始化）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected void DoLuaSqlPreExecutePolicyExecutors(IDictionary<string, ISqlConfigPolicy> policies,
            ILuaSqlPolicyExecutorInfo info)
        {
            var pinfo = (ILuaSqlPreExecutePolicyExecutorInfo)info;
            ISqlConfigPolicy policy = null;

            if (policies?.Count > 0)
            {
                foreach (var pair in _luasqlPreExecutePolicyExecutorsOrder)
                {
                    info.PolicyName = pair.Key;
                    //全局的策略对象
                    _globalPolicies.TryGetValue(pair.Key, out policy);
                    info.GlobalPolicy = policy;

                    //获取通过方法形参传递过来的策略对象
                    policies.TryGetValue(pair.Key, out policy);
                    info.ParameterPolicy = policy;

                    pair.Value.Item2.Invoke().Execute(pinfo);
                }
            }
            else
            {
                info.ParameterPolicy = null;
                foreach (var pair in _luasqlPreExecutePolicyExecutorsOrder)
                {
                    info.PolicyName = pair.Key;
                    //全局的策略对象
                    _globalPolicies.TryGetValue(pair.Key, out policy);
                    info.GlobalPolicy = policy;

                    pair.Value.Item2.Invoke().Execute(pinfo);
                }
            }

        }

        /// <summary>
        /// 用于lua sql执行时的策略执行器（例如：查询缓存，查询缓存清理，日志记录等等）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected void DoLuaSqlExecutePolicyExecutors(IDictionary<string, ISqlConfigPolicy> policies,
            ILuaSqlPolicyExecutorInfo info)
        {
            var pinfo = (ILuaSqlExecutePolicyExecutorInfo)info;
            ISqlConfigPolicy policy = null;

            if (policies?.Count > 0)
            {
                foreach (var pair in _luasqlExecutePolicyExecutorsOrder)
                {
                    info.PolicyName = pair.Key;
                    //全局的策略对象
                    _globalPolicies.TryGetValue(pair.Key, out policy);
                    info.GlobalPolicy = policy;

                    //获取通过方法形参传递过来的策略对象
                    policies.TryGetValue(pair.Key, out policy);
                    info.ParameterPolicy = policy;

                    pair.Value.Item2.Invoke().Execute(pinfo);
                    //如果是执行完毕，那么跳出循环不再继续执行
                    if (pinfo.IsEnd)
                    {
                        break;
                    }
                }
            }
            else
            {
                info.ParameterPolicy = null;
                foreach (var pair in _luasqlExecutePolicyExecutorsOrder)
                {
                    info.PolicyName = pair.Key;
                    //全局的策略对象
                    _globalPolicies.TryGetValue(pair.Key, out policy);
                    info.GlobalPolicy = policy;

                    pair.Value.Item2.Invoke().Execute(pinfo);
                    //如果是执行完毕，那么跳出循环不再继续执行
                    if (pinfo.IsEnd)
                    {
                        break;
                    }
                }
            }

        }

    }
}
