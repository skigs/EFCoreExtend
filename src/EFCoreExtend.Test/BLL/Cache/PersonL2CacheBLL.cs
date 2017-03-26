using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class PersonL2CacheBLL
    {
        DBConfigTable tinfo;
        public PersonL2CacheBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }

        /// <summary>
        /// 二级查询缓存，不设置缓存过期时间(缓存不过期)
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Person> GetListL2Cache()
        {
            return tinfo.GetExecutor().Query<Person>();
        }

        /// <summary>
        /// 指定了CacheType（query1），默认为query
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Person> GetListL2Cache1()
        {
            return tinfo.GetExecutor().Query<Person>();
        }

        /// <summary>
        /// "date": "2018-01-01" //指定缓存的过期日期
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Person> GetListL2Cache2()
        {
            return tinfo.GetExecutor().Query<Person>();
        }

        /// <summary>
        /// "span": "0:0:3" //指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔），这里设置为3s（方便测试）
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Person> GetListL2Cache3()
        {
            return tinfo.GetExecutor(nameof(GetListL2Cache2)).QueryUseDict<Person>(null, null, new SqlL2QueryCachePolicy
            {
                Expiry = new EFCache.Default.QueryCacheExpiryPolicy(TimeSpan.FromSeconds(3))
            });
        }

        /// <summary>
        ///  "span": "0:0:3", //指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔），这里设置为3s（方便测试）
        ///  "isUpdateEach": true //是否每次获取缓存之后更新过期时间(这个属性 + span属性来进行模拟session访问更新过期时间)
        /// </summary>
        /// <returns></returns>
        public int CountL2Cache(TimeSpan? span = null)
        {
            if (span.HasValue)
            {
                //更改为不自动更新时间的
                return (int)typeof(int).ChangeValueType(
                tinfo.GetExecutor().ScalarUseModel(null, null, new SqlL2QueryCachePolicy
                {
                    Expiry = new EFCache.Default.QueryCacheExpiryPolicy(span.Value, false)
                }));
            }
            else
            {
                //MSSqlServer返回值会为int，而Sqlite会为long，转换就会出错，因此需要ChangeValueType
                return (int)typeof(int).ChangeValueType(tinfo.GetExecutor().Scalar());
            }
        }

        /// <summary>
        ///  "isSelfAll": true //是否清理 所在表下 的所有缓存
        /// </summary>
        /// <returns></returns>
        public int AddPersonL2Cache()
        {
            return tinfo.GetExecutor().NonQuery();
        }

        /// <summary>
        ///  output inserted.id
        ///  "isSelfAll": true //是否清理 所在表下 的所有缓存
        /// </summary>
        /// <returns></returns>
        public int AddPersonL2Cache1()
        {
            return (int)tinfo.GetExecutor().Scalar();   // output inserted.id 因此使用Scalar
        }

        /// <summary>
        /// 不清理缓存
        /// </summary>
        /// <returns></returns>
        public int AddPersonL2NotClearCache()
        {
            return tinfo.GetExecutor(nameof(AddPersonL2Cache))
                .NonQueryUseDict(null, new SqlClearCachePolicy
                {
                    IsUse = false,  //不使用清理缓存策略
                });
        }

        /// <summary>
        /// "cacheTypes": [ "query" ] //需要进行缓存清理的类型（用于清理 所在表下 的CacheType查询缓存）
        /// </summary>
        /// <returns></returns>
        public int UpdatePersonL2Cache()
        {
            return tinfo.GetExecutor().NonQuery();
        }

        /// <summary>
        /// "cacheTypes": [ "query1" ] //需要进行缓存清理的类型（用于清理 所在表下 的CacheType查询缓存）
        /// </summary>
        /// <returns></returns>
        public int UpdatePersonL2Cache0()
        {
            return tinfo.GetExecutor().NonQuery();
        }

        /// <summary>
        /// "tables": [ "Address" ] //需要进行缓存清理的表的名称（一般用于清理 其他表下 的所有查询缓存）
        /// </summary>
        /// <returns></returns>
        public int UpdatePersonL2Cache1()
        {
            return tinfo.GetExecutor().NonQuery();
        }

        /// <summary>
        /// "tableCacheTypes": { //需要进行缓存清理的类型(key为TableName，value为CacheType，一般用于清理 其他表下 的CacheType)
        ///     "Address": [ "query" ]}
        /// </summary>
        /// <returns></returns>
        public int UpdatePersonL2Cache2()
        {
            return tinfo.GetExecutor().NonQuery();
        }

        /// <summary>
        /// "isSelfAll": true //是否清理 所在表下 的所有缓存
        /// </summary>
        /// <returns></returns>
        public int DeletePersonL2Cache()
        {
            return tinfo.GetExecutor().NonQuery();
        }

    }
}
