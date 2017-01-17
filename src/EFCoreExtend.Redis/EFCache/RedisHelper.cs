using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Redis.EFCache.Helper
{
    public static class RedisHelper
    {
        public static void Set(this IRedisDBConnect redis, string key, string val)
        {
            var db = redis.GetDatabase();
            db.StringSet(key, val);
        }

        public static void Set(this IRedisDBConnect redis, string key, string val, TimeSpan? expiry)
        {
            var db = redis.GetDatabase();
            db.StringSet(key, val, expiry);
        }

        public static void Set(this IRedisDBConnect redis, string key, string val, DateTime? expiry)
        {
            var db = redis.GetDatabase();
            db.StringSet(key, val);
            db.KeyExpire(key, expiry);
        }

        public static void SetJson(this IRedisDBConnect redis, string key, object val)
        {
            Set(redis, key, JsonConvert.SerializeObject(val));
        }

        public static void SetJson(this IRedisDBConnect redis, string key, object val, TimeSpan expiry)
        {
            Set(redis, key, JsonConvert.SerializeObject(val), expiry);
        }

        public static void SetJson(this IRedisDBConnect redis, string key, object val, DateTime expiry)
        {
            Set(redis, key, JsonConvert.SerializeObject(val), expiry);
        }

        public static bool TryGet(this IRedisDBConnect redis, string key, out string val)
        {
            val = null;
            var db = redis.GetDatabase();
            var strVal = db.StringGet(key);
            if (strVal.HasValue)
            {
                val = strVal.ToString();
                return true;
            }
            return false;
        }

        public static bool TryGet<T>(this IRedisDBConnect redis, string key, out T val)
        {
            val = default(T);
            string strVal;
            if (TryGet(redis, key, out strVal))
            {
                try
                {
                    val = JsonConvert.DeserializeObject<T>(strVal);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public static bool TryGet(this IRedisDBConnect redis, string key, TimeSpan? expiry, out string val)
        {
            val = null;
            var db = redis.GetDatabase();
            var strVal = db.StringGet(key);
            if (strVal.HasValue)
            {
                val = strVal.ToString();
                db.KeyExpireAsync(key, expiry);
                return true;
            }
            return false;
        }

        public static bool TryGet<T>(this IRedisDBConnect redis, string key, TimeSpan? expiry, out T val)
        {
            val = default(T);
            string strVal;
            if (TryGet(redis, key, expiry, out strVal))
            {
                try
                {
                    val = JsonConvert.DeserializeObject<T>(strVal);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public static bool TryGet(this IRedisDBConnect redis, string key, DateTime? expiry, out string val)
        {
            val = null;
            var db = redis.GetDatabase();
            var strVal = db.StringGet(key);
            if (strVal.HasValue)
            {
                val = strVal.ToString();
                db.KeyExpireAsync(key, expiry);
                return true;
            }
            return false;
        }

        public static bool TryGet<T>(this IRedisDBConnect redis, string key, DateTime? expiry, out T val)
        {
            val = default(T);
            string strVal;
            if (TryGet(redis, key, expiry, out strVal))
            {
                try
                {
                    val = JsonConvert.DeserializeObject<T>(strVal);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public static bool DeleteKey(this IRedisDBConnect redis, string key)
        {
            return redis.GetDatabase().KeyDelete(key);
        }

        public static void ClearKeys(this IRedisDBConnect redis, string keypattern)
        {
            redis.GetDatabase().ScriptEvaluate(LuaScript.Prepare(
                " local ks = redis.call('KEYS', @keypattern) " +
                " for i=1,#ks,5000 do " +
                "     redis.call('del', unpack(ks, i, math.min(i+4999, #ks))) " +
                " end " +
                " return true "
                ), new { keypattern = keypattern });
        }

    }
}
