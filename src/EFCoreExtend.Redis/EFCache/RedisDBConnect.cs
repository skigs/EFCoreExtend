using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Redis.EFCache
{
    public class RedisDBConnect : IRedisDBConnect
    {
        ConnectionMultiplexer _redis;
        public RedisDBConnect(ConfigurationOptions configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration);
        }
        public RedisDBConnect(string configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration);
        }
        public IDatabase GetDatabase()
        {
            return _redis.GetDatabase();
        }
        public IConnectionMultiplexer GetConnection()
        {
            return _redis;
        }
    }
}
