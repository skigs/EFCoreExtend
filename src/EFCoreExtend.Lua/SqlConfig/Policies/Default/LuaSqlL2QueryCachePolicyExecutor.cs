using EFCoreExtend.Commons;
using EFCoreExtend.EFCache;
using EFCoreExtend.EFCache.Default;
using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.Default
{
    /// <summary>
    /// 查询缓存策略执行器
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlL2QueryCachePolicyName)]
    public class LuaSqlL2QueryCachePolicyExecutor : PolicyExecutorBase, ILuaSqlExecutePolicyExecutor
    {
        protected readonly IEFQueryCache _cache;
        protected readonly IEFCoreExtendUtility _util;

        public LuaSqlL2QueryCachePolicyExecutor(IEFQueryCache cache, IEFCoreExtendUtility util)
        {
            cache.CheckNull(nameof(cache));
            util.CheckNull(nameof(util));

            _cache = cache;
            _util = util;
        }

        public void Execute(ILuaSqlExecutePolicyExecutorInfo info)
        {
            string defCacheType = null;
            //query和scalar才进行缓存
            if (info.ExecuteType == ConfigSqlExecuteType.query)
            {
                defCacheType = QueryCacheExtensions.CacheType;
            }
            else if (info.ExecuteType == ConfigSqlExecuteType.scalar)
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
                    var rtn = _cache.Cache(info.TableName, cacheType,
                            _util.CombineSqlAndParamsToString(info.Sql, info.SqlParams),
                            info.ToDBExecutor, policy.Expiry, info.ReturnType);
                    info.ReturnValue = rtn;

                    //执行缓存成功，那么结束
                    info.IsEnd = true;
                }
            }
        }

    }
}
