using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Redis.EFCache
{
    public interface IRedisDBConnect
    {
        IDatabase GetDatabase();
        IConnectionMultiplexer GetConnection();
    }
}
