using EFCoreExtend.Commons;
using EFCoreExtend.EFCache;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    /// <summary>
    /// 二级查询缓存策略执行器
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlL2QueryCachePolicyName)]
    public class SqlL2QueryCachePolicyExecutor : PolicyExecutorBase, ISqlExecutePolicyExecutor
    {
        protected readonly IEFQueryCache _cache;
        protected readonly IEFCoreExtendUtility _util;

        public SqlL2QueryCachePolicyExecutor(IEFQueryCache cache, IEFCoreExtendUtility util)
        {
            cache.CheckNull(nameof(cache));
            util.CheckNull(nameof(util));

            _cache = cache;
            _util = util;
        }

        public void Execute(ISqlExecutePolicyExecutorInfo info)
        {
            string defCacheType = null;
            //query和scalar才进行缓存
            if (info.ExecuteType == ConfigSqlExecuteType.query)
            {
                defCacheType = QueryCacheExtensions.CacheType;
            }
            else if(info.ExecuteType == ConfigSqlExecuteType.scalar)
            {
                defCacheType = ScalarCacheExtensions.CacheType;
            }

            if (defCacheType != null)
            {
                //执行缓存策略
                var policy = info.GetPolicy() as SqlL2QueryCachePolicy;
                //是否配置了查询缓存
                if (IsUsePolicy(policy))
                {
                    var cacheType = string.IsNullOrEmpty(policy.Type) ? defCacheType : policy.Type;
                    info.ReturnValue = _cache.Cache(info.TableName, cacheType,
                        _util.CombineSqlAndParamsToString(info.Sql, info.SqlParams),
                        info.ToDBExecutor, policy.Expiry, info.ReturnType);

                    //执行缓存成功，那么结束
                    info.IsEnd = true;
                } 
            }
        }

    }
}
