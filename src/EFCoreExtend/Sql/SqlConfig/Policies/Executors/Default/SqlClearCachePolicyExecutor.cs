using EFCoreExtend.EFCache;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    /// <summary>
    /// 清理二级查询缓存策略 执行器（用于非查询(NonQuery)中）
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlClearCachePolicyName)]
    public class SqlClearCachePolicyExecutor : PolicyExecutorBase, ISqlExecutePolicyExecutor
    {
        protected readonly IEFQueryCache _cache;
        public SqlClearCachePolicyExecutor(IEFQueryCache cache)
        {
            cache.CheckNull(nameof(cache));

            _cache = cache;
        }

        public void Execute(ISqlExecutePolicyExecutorInfo info)
        {
            //缓存清理
            if (info.ExecuteType == ConfigSqlExecuteType.nonquery
                //scalar类型也是可能执行nonquery语句的，例如：获取新增后的id：
                //  insert into TName(...) output inserted.id values(...)
                || info.ExecuteType == ConfigSqlExecuteType.scalar)
            {
                var policy = info.GetPolicy() as SqlClearCachePolicy;
                //而且配置了缓存清理，那么进行清理缓存操作
                if (IsUsePolicy(policy))
                {
                    //到数据库中执行sql
                    var result = (int)info.ToDBExecutor();
                    //返回值大于0，就是NonQuery有处理的数据才进行清理缓存
                    if (result > 0)
                    {
                        //是否使用异步
                        if (policy.IsAsync)
                        {
                            Task.Run(() =>
                            {
                                ClearCache(info.TableName, policy);
                            });
                        }
                        else
                        {
                            ClearCache(info.TableName, policy);
                        }
                    }
                    info.ReturnValue = result;
                    //执行之后，那么结束
                    info.IsEnd = true;
                }
            }
        }

        protected void ClearCache(string tableName, SqlClearCachePolicy policy)
        {
            //清理所在表下的所有缓存
            if (policy.IsSelfAll)
            {
                _cache.Remove(tableName);
            }
            else
            {
                //清理所在表下的CacheType
                if (policy.CacheTypes?.Count > 0)
                {
                    foreach (var ct in policy.CacheTypes)
                    {
                        _cache.Remove(tableName, ct);
                    }
                }
            }

            //清理其他的Table
            if (policy.Tables?.Count > 0)
            {
                foreach (var t in policy.Tables)
                {
                    _cache.Remove(t);
                }
            }

            //清理其他的Table下的CachcType
            if (policy.TableCacheTypes?.Count > 0)
            {
                foreach (var pair in policy.TableCacheTypes)
                {
                    _cache.Remove(pair.Key, pair.Value);
                }
            }
        }

    }
}
