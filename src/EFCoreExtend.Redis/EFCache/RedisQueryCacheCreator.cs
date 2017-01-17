using EFCoreExtend.EFCache;
using EFCoreExtend.EFCache.BaseCache;
using StackExchange.Redis;
using System;

namespace EFCoreExtend.Redis.EFCache
{
    public class RedisQueryCacheCreator : IQueryCacheCreator
    {
        readonly IRedisDBConnect _redisDBConnect;
        public IRedisDBConnect RedisDBConnect => _redisDBConnect;

        /// <summary>
        /// StackExchange.Redis的配置
        /// </summary>
        /// <param name="configuration"></param>
        public RedisQueryCacheCreator(ConfigurationOptions configuration)
        {
            configuration.CheckNull(nameof(configuration));

            _redisDBConnect = new RedisDBConnect(configuration);
        }

        /// <summary>
        /// StackExchange.Redis的连接字串
        /// </summary>
        /// <param name="configuration"></param>
        public RedisQueryCacheCreator(string configuration)
        {
            configuration.CheckStringIsNullOrEmpty(nameof(configuration));

            _redisDBConnect = new RedisDBConnect(configuration);
        }

        /// <summary>
        /// IRedisDBConnect的实例(用于获取StackExchange.Redis的连接对象)
        /// </summary>
        /// <param name="redisDBConnect"></param>
        public RedisQueryCacheCreator(IRedisDBConnect redisDBConnect)
        {
            redisDBConnect.CheckNull(nameof(redisDBConnect));

            _redisDBConnect = redisDBConnect;
        }

        public IQueryCache Create(string tableName)
        {
            return new RedisQueryCache(tableName, RedisDBConnect);
        }

    }
}
