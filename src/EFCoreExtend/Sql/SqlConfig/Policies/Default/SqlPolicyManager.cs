using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    public class SqlPolicyManager : ISqlPolicyManager
    {
        #region 数据对象

        IDictionary<string, ISqlConfigPolicy> _globalPolicies = new ConcurrentDictionary<string, ISqlConfigPolicy>();
        public IDictionary<string, ISqlConfigPolicy> GlobalPolicies => _globalPolicies;

        protected readonly IDictionary<string, Type> _policyTypes = new ConcurrentDictionary<string, Type>();
        public IDictionary<string, Type> PolicyTypes => _policyTypes;

        readonly protected IDictionary<string, Tuple<int, Func<ISqlInitPolicyExecutor>>> _sqlInitPolicyExecutors
            = new ConcurrentDictionary<string, Tuple<int, Func<ISqlInitPolicyExecutor>>>();
        IEnumerable<KeyValuePair<string, Tuple<int, Func<ISqlInitPolicyExecutor>>>> _sqlInitPolicyExecutorsOrder;
        readonly InitAction _sqlInitPolicyExecutorsOrderInit;

        readonly protected IDictionary<string, Tuple<int, Func<ISqlPreExecutePolicyExecutor>>> _sqlPreExecutePolicyExecutors
            = new ConcurrentDictionary<string, Tuple<int, Func<ISqlPreExecutePolicyExecutor>>>();
        IEnumerable<KeyValuePair<string, Tuple<int, Func<ISqlPreExecutePolicyExecutor>>>> _sqlPreExecutePolicyExecutorsOrder;
        readonly InitAction _sqlPreExecutePolicyExecutorsOrderInit;

        readonly protected IDictionary<string, Tuple<int, Func<ISqlExecutePolicyExecutor>>> _sqlExecutePolicyExecutors
            = new ConcurrentDictionary<string, Tuple<int, Func<ISqlExecutePolicyExecutor>>>();
        IEnumerable<KeyValuePair<string, Tuple<int, Func<ISqlExecutePolicyExecutor>>>> _sqlExecutePolicyExecutorsOrder;
        readonly InitAction _sqlExecutePolicyExecutorsOrderInit;


        readonly protected Type _tISqlInitPolicyExecutor = typeof(ISqlInitPolicyExecutor);
        readonly protected Type _tISqlPreExecutePolicyExecutor = typeof(ISqlPreExecutePolicyExecutor);
        readonly protected Type _tISqlExecutePolicyExecutor = typeof(ISqlExecutePolicyExecutor);

        readonly protected Type _tISqlInitPolicyExecutorInfo = typeof(ISqlInitPolicyExecutorInfo);
        readonly protected Type _tISqlPreExecutePolicyExecutorInfo = typeof(ISqlPreExecutePolicyExecutorInfo);
        readonly protected Type _tISqlExecutePolicyExecutorInfo = typeof(ISqlExecutePolicyExecutorInfo);
        #endregion

        public SqlPolicyManager()
        {
            //对执行器的执行优先级进行排序初始化
            _sqlInitPolicyExecutorsOrderInit = new InitAction(() => 
            {
                //优先级的值越大越先执行
                _sqlInitPolicyExecutorsOrder = _sqlInitPolicyExecutors.OrderByDescending(l => l.Value.Item1).ToList();
            });
            _sqlPreExecutePolicyExecutorsOrderInit = new InitAction(() =>
            {
                _sqlPreExecutePolicyExecutorsOrder = _sqlPreExecutePolicyExecutors.OrderByDescending(l => l.Value.Item1).ToList();
            });
            _sqlExecutePolicyExecutorsOrderInit = new InitAction(() =>
            {
                _sqlExecutePolicyExecutorsOrder = _sqlExecutePolicyExecutors.OrderByDescending(l => l.Value.Item1).ToList();
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

            //if (getExecutorFunc is Func<ISqlInitPolicyExecutor>)  //不使用is，因为类型T可能都继承了所有类型而造成都转换成功
            if(typeof(T) == _tISqlInitPolicyExecutor)
            {
                _sqlInitPolicyExecutors[policyName] = Tuple.Create(priority, getExecutorFunc as Func<ISqlInitPolicyExecutor>);
                //执行器有改动，那么重新初始化
                _sqlInitPolicyExecutorsOrderInit.Release();
            }
            else if(typeof(T) == _tISqlPreExecutePolicyExecutor)
            {
                _sqlPreExecutePolicyExecutors[policyName] = Tuple.Create(priority, getExecutorFunc as Func<ISqlPreExecutePolicyExecutor>);
                //执行器有改动，那么重新初始化
                _sqlPreExecutePolicyExecutorsOrderInit.Release();
            }
            else if(typeof(T) == _tISqlExecutePolicyExecutor)
            {
                _sqlExecutePolicyExecutors[policyName] = Tuple.Create(priority, getExecutorFunc as Func<ISqlExecutePolicyExecutor>);
                //执行器有改动，那么重新初始化
                _sqlExecutePolicyExecutorsOrderInit.Release();
            }
            else
            {
                throw new ArgumentException($"Invalid type [{typeof(T).FullName}]", nameof(getExecutorFunc));
            }
        }

        /// <summary>
        /// 执行相关的策略执行器
        /// </summary>
        public void InvokeExecutors<T>(IReadOnlyDictionary<string, ISqlConfigPolicy> policies, T info)
            where T : IPolicyExecutorInfoBase
        {
            var policyInfo = (IPolicyExecutorInfo)info;
            //if (info is ISqlInitPolicyExecutorInfo)   //不使用is，因为类型T可能都继承了所有类型而造成都转换成功
            if(typeof(T) == _tISqlInitPolicyExecutorInfo)
            {
                //在策略执行之前进行相应的初始化操作（优先级排序）
                _sqlInitPolicyExecutorsOrderInit.Invoke(-1);
                DoSqlInitPolicyExecutors(policies, policyInfo);
            }
            else if (typeof(T) == _tISqlPreExecutePolicyExecutorInfo)
            {
                _sqlPreExecutePolicyExecutorsOrderInit.Invoke(-1);
                DoSqlPreExecutePolicyExecutors(policies, policyInfo);
            }
            else if (typeof(T) == _tISqlExecutePolicyExecutorInfo)
            {
                _sqlExecutePolicyExecutorsOrderInit.Invoke(-1);
                DoSqlExecutePolicyExecutors(policies, policyInfo);
            }
            else
            {
                throw new ArgumentException($"Invalid type [{info.GetType().FullName}]", nameof(info));
            }

        }

        /// <summary>
        /// 用于初始化的策略执行器，就是在程序运行期间只执行一次，除非sql的配置数据发生了改变（例如：替换表名 / 合并分部sql等的执行器）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected void DoSqlInitPolicyExecutors(IReadOnlyDictionary<string, ISqlConfigPolicy> policies,
            IPolicyExecutorInfo info)
        {
            var pinfo = (ISqlInitPolicyExecutorInfo)info;
            ISqlConfigPolicy policy = null;

            if (policies?.Count > 0)
            {
                foreach (var pair in _sqlInitPolicyExecutorsOrder)
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
                foreach (var pair in _sqlInitPolicyExecutorsOrder)
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
        /// 在sql执行前的策略执行器（例如：foreach的执行器对某些数据类型（list/dict等等）进行生成字串替换到sql中）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected void DoSqlPreExecutePolicyExecutors(IReadOnlyDictionary<string, ISqlConfigPolicy> policies,
            IPolicyExecutorInfo info)
        {
            var pinfo = (ISqlPreExecutePolicyExecutorInfo)info;
            ISqlConfigPolicy policy = null;

            if (policies?.Count > 0)
            {
                foreach (var pair in _sqlPreExecutePolicyExecutorsOrder)
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
                foreach (var pair in _sqlPreExecutePolicyExecutorsOrder)
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
        /// 用于sql执行时的策略执行器（例如：查询缓存（一级/二级的），查询缓存清理（NonQuery的执行之后对二级缓存的清理）等等）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected void DoSqlExecutePolicyExecutors(IReadOnlyDictionary<string, ISqlConfigPolicy> policies,
            IPolicyExecutorInfo info)
        {
            var pinfo = (ISqlExecutePolicyExecutorInfo)info;
            ISqlConfigPolicy policy = null;

            if (policies?.Count > 0)
            {
                foreach (var pair in _sqlExecutePolicyExecutorsOrder)
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
                foreach (var pair in _sqlExecutePolicyExecutorsOrder)
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
