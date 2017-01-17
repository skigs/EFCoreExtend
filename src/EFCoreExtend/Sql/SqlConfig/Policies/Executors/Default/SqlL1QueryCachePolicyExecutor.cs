using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    /// <summary>
    /// 一级查询缓存策略执行器(只作用与SqlConfigExecutor对象中，默认配置使用，缓存策略：如果一二级查询缓存都有配置，那么应该二级缓存优先调用获取)
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.SqlL1QueryCachePolicyName)]
    public class SqlL1QueryCachePolicyExecutor : PolicyExecutorBase, ISqlExecutePolicyExecutor
    {
        protected readonly IEFCoreExtendUtility _util;

        public SqlL1QueryCachePolicyExecutor(IEFCoreExtendUtility util)
        {
            util.CheckNull(nameof(util));
            _util = util;
        }

        public void Execute(ISqlExecutePolicyExecutorInfo info)
        {
            //query和scalar才进行缓存
            if (info.ExecuteType == ConfigSqlExecuteType.query || info.ExecuteType == ConfigSqlExecuteType.scalar)
            {
                //执行缓存策略
                var policy = info.GetPolicy() as SqlL1QueryCachePolicy;
                //是否配置了查询缓存
                if (IsUsePolicy(policy))
                {
                    var key = _util.CombineSqlAndParamsToString(info.Sql, info.SqlParams);
                    object obj;
                    //查看存不存在缓存数据
                    if (!info.SqlExecutorTempDatas.TryGetValue(key, out obj))
                    {
                        //到数据库中获取
                        obj = info.ToDBExecutor();
                        //缓存
                        info.SqlExecutorTempDatas[key] = obj;
                    }

                    info.ReturnValue = obj;
                    //执行缓存成功，那么结束
                    info.IsEnd = true;
                }
            }
        }
    }
}
