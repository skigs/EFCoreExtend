using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend;
using System.Threading;
using EFCoreExtend.EFCache.Default;
using System.Data.SqlClient;
using System.Data;
using Xunit;

namespace EFCoreExtend.Test
{
    /// <summary>
    /// query查询缓存测试
    /// </summary>
    public class TestSqlQueryCache
    {
        static DbContext db = new MSSqlDBContext();
        #region 测试的参数
        static string name = "efcache";
        static string fullAddress = "efaddr";
        static string sql = $"select * from {nameof(Person)} where name=@name";
        static string sql1 = $"select * from {nameof(Person)} where name=@name and 1=1";
        static string sqlAddr = $"select * from {nameof(Address)} where fullAddress=@fullAddress";

        void Add()
        {
            var rtn = db.NonQueryUseModel(
                $"insert into {nameof(Person)}(name, birthday, addrid) values(@name, @birthday, @addrid)",
                new Person { name = name }, new[] { "id" });
            Assert.True(rtn > 0);
        }

        void Del()
        {
            var rtn = db.NonQueryUseModel($"delete from {nameof(Person)} where name=@name", new { name = name });
            Assert.True(rtn > 0);
        }

        void AddAddr()
        {
            var rtn = db.NonQueryUseModel(
                $"insert into {nameof(Address)}(fullAddress, lat, lon) values(@fullAddress, @lat, @lon)",
                new Address { fullAddress = fullAddress }, new[] { "id" });
            Assert.True(rtn > 0);
        }

        void DelAddr()
        {
            var rtn = db.NonQueryUseModel($"delete from {nameof(Address)} where fullAddress=@fullAddress",
                new { fullAddress = fullAddress });
            Assert.True(rtn > 0);
        }
        #endregion

        static TestSqlQueryCache()
        {
            ////使用Redis进行缓存
            //EFHelper.ServiceBuilder.AddQueryCacheCreator(sp =>
            //    new Redis.EFCache.RedisQueryCacheCreator("127.0.0.1:6379,allowAdmin=true"))
            //    .BuildServices();   //重新编译服务

            ////移除缓存，以免影响测试
            //EFHelper.Services.Cache.Remove<Person>();
            //EFHelper.Services.Cache.Remove<Address>();
        }

        /// <summary>
        /// 缓存不过期
        /// </summary>
        [Fact]
        public void Test()
        {
            Add();

            //缓存不过期
            var val = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Add();
            var val1 = db.QueryCacheUseDict<Person>(typeof(Person), sql, new Dictionary<string, object>() { { "name", name } }, null, null);
            var val2 = db.QueryCache<Person>(nameof(Person), sql, new[] { new SqlParameter("name", name) }, null, null);
            Assert.NotNull(val);
            Assert.True(val?.Count == val1?.Count);
            Assert.True(val?.Count == val2?.Count);
            
            Del();
        }

        /// <summary>
        /// 缓存过期
        /// </summary>
        [Fact]
        public void Test1()
        {
            Add();

            var expiry = new QueryCacheExpiryPolicy(TimeSpan.FromSeconds(3));
            //缓存过期
            var val = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, expiry);
            Add();
            var val1 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, expiry);
            Assert.NotNull(val);
            Assert.True(val?.Count == val1?.Count);

            Thread.Sleep(3100);
            var val2 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, expiry);
            Assert.True(val?.Count != val2?.Count);

            Del();
        }

        /// <summary>
        /// 缓存过期与更新
        /// </summary>
        [Fact]
        public void Test2()
        {
            Add();

            var expiry = new QueryCacheExpiryPolicy(TimeSpan.FromSeconds(3), true);
            //缓存过期
            var val = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, expiry);
            Add();
            var val1 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, expiry);
            Assert.NotNull(val);
            Assert.True(val?.Count == val1?.Count);

            Thread.Sleep(2000);
            var val2 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, expiry);
            Assert.True(val?.Count == val2?.Count);

            Thread.Sleep(2000);
            var val3 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, expiry);
            Assert.True(val?.Count == val3?.Count);

            Del();
        }

        /// <summary>
        /// 移除指定key（IQueryable）的缓存
        /// </summary>
        [Fact]
        public void TestRemove()
        {
            Add();

            //查询缓存，不过期
            var rtn = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Add();
            var rtn1 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count == rtn1?.Count);

            //清理缓存
            db.QueryCacheRemoveUseModel<Person>(sql, new { name = name }, null);
            var rtn2 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count != rtn2?.Count);

            Del();
        }

        /// <summary>
        /// 移除指定key（IQueryable）的缓存
        /// </summary>
        [Fact]
        public void TestRemove1()
        {
            Add();

            //查询缓存，不过期
            var rtn = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Add();
            var rtn1 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count == rtn1?.Count);

            //不同的IQueryable
            var rtn2 = db.QueryCacheUseModel<Person, Person>(sql1, new { name = name }, null, null, null);
            Add();
            var rtn3 = db.QueryCacheUseModel<Person, Person>(sql1, new { name = name }, null, null, null);
            Assert.True(rtn?.Count != rtn2?.Count);
            Assert.True(rtn2?.Count == rtn3?.Count);

            //清理缓存
            db.QueryCacheRemoveUseModel<Person>(sql, new { name = name }, null);    //移除queryable的缓存
            var rtn4 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count != rtn4?.Count);

            var rtn5 = db.QueryCacheUseModel<Person, Person>(sql1, new { name = name }, null, null, null);
            Assert.True(rtn2?.Count == rtn5?.Count);  //queryable移除了，但是queryable1并没有移除

            Del();
        }

        /// <summary>
        /// 移除指定CacheType（缓存类型query）的缓存
        /// </summary>
        [Fact]
        public void TestRemove2()
        {
            Add();

            //查询缓存，不过期
            var rtn = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Add();
            var rtn1 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count == rtn1?.Count);

            //不同的IQueryable
            var rtn2 = db.QueryCacheUseModel<Person, Person>(sql1, new { name = name }, null, null, null);
            Add();
            var rtn3 = db.QueryCacheUseModel<Person, Person>(sql1, new { name = name }, null, null, null);
            Assert.True(rtn2?.Count == rtn3?.Count);

            Assert.True(rtn?.Count != rtn2?.Count);

            //清理缓存(移除指定缓存类型的：query)
            EFHelper.Services.Cache.QueryRemove<Person>();
            var rtn4 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count != rtn4?.Count);

            var rtn5 = db.QueryCacheUseModel<Person, Person>(sql1, new { name = name }, null, null, null);
            Assert.True(rtn2?.Count != rtn5?.Count);

            Del();
        }

        /// <summary>
        /// 移除指定表的缓存
        /// </summary>
        [Fact]
        public void TestRemove4()
        {
            Add();

            //查询缓存，不过期
            var rtn = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Add();
            var rtn1 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count == rtn1?.Count);

            //不同的IQueryable
            var rtn2 = db.QueryCacheUseModel<Person, Person>(sql1, new { name = name }, null, null, null);
            Add();
            var rtn3 = db.QueryCacheUseModel<Person, Person>(sql1, new { name = name }, null, null, null);
            Assert.True(rtn2?.Count == rtn3?.Count);

            Assert.True(rtn?.Count != rtn2?.Count);

            //清理缓存(移除整个表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count != rtn4?.Count);

            var rtn5 = db.QueryCacheUseModel<Person, Person>(sql1, new { name = name }, null, null, null);
            Assert.True(rtn2?.Count != rtn5?.Count);

            Del();
        }

        /// <summary>
        /// 移除指定表的缓存
        /// </summary>
        [Fact]
        public void TestRemove5()
        {
            Add();
            AddAddr();

            //查询缓存，不过期
            var rtn = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Add();
            var rtn1 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count == rtn1?.Count);

            var artn = db.QueryCacheUseModel<Address, Address>(sqlAddr, new { fullAddress = fullAddress }, null, null, null);
            AddAddr();
            var artn1 = db.QueryCacheUseModel<Address, Address>(sqlAddr, new { fullAddress = fullAddress }, null, null, null);
            Assert.True(artn?.Count == artn1?.Count);

            //清理缓存(移除指定表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = db.QueryCacheUseModel<Person, Person>(sql, new { name = name }, null, null, null);
            Assert.True(rtn?.Count != rtn4?.Count);

            var artn2 = db.QueryCacheUseModel<Address, Address>(sqlAddr, new { fullAddress = fullAddress }, null, null, null);
            Assert.True(artn?.Count == artn2?.Count); //Address表的缓存还没有移除


            Del();
            DelAddr();
        }


    }
}
