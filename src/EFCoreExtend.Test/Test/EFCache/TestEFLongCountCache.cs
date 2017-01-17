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
    /// IQueryable.LongCount查询缓存测试
    /// </summary>
    public class TestEFLongCountCache
    {
        static DbContext db = new MSSqlDBContext();
        #region 测试的参数
        static string name = "efcache";
        static string fullAddress = "efaddr";
        static IQueryable<Person> queryable = db.Set<Person>().Where(l => l.name == name);
        static IQueryable<Person> queryable1 = db.Set<Person>().Where(l => true);
        static IQueryable<Address> queryAddr = db.Set<Address>().Where(l => l.fullAddress == fullAddress);

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

        static TestEFLongCountCache()
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
            var val = queryable.LongCountCache<Person, Person>(null);
            Add();
            var val1 = queryable.LongCountCache(typeof(Person), null);
            var val2 = queryable.LongCountCache(nameof(Person), null);
            Assert.True(val > 0);
            Assert.True(val == val1);
            Assert.True(val == val2);

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
            var val = queryable.LongCountCache<Person, Person>(expiry);
            Add();
            var val1 = queryable.LongCountCache<Person, Person>(expiry);
            Assert.True(val > 0);
            Assert.True(val == val1);

            Thread.Sleep(3100);
            var val2 = queryable.LongCountCache<Person, Person>(expiry);
            Assert.True(val != val2);

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
            var val = queryable.LongCountCache<Person, Person>(expiry);
            Add();
            var val1 = queryable.LongCountCache<Person, Person>(expiry);
            Assert.True(val > 0);
            Assert.True(val == val1);

            Thread.Sleep(2000);
            var val2 = queryable.LongCountCache<Person, Person>(expiry);
            Assert.True(val == val2);

            Thread.Sleep(2000);
            var val3 = queryable.LongCountCache<Person, Person>(expiry);
            Assert.True(val == val3);

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
            var rtn = queryable.LongCountCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn == rtn1);

            //清理缓存
            queryable.LongCountCacheRemove<Person>();
            var rtn2 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn != rtn2);

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
            var rtn = queryable.LongCountCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn == rtn1);

            //不同的IQueryable
            var rtn2 = queryable1.LongCountCache<Person, Person>(null);
            Add();
            var rtn3 = queryable1.LongCountCache<Person, Person>(null);
            Assert.True(rtn != rtn2);
            Assert.True(rtn2 == rtn3);

            //清理缓存
            queryable.LongCountCacheRemove<Person>();    //移除queryable的缓存
            var rtn4 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn != rtn4);

            var rtn5 = queryable1.LongCountCache<Person, Person>(null);
            Assert.True(rtn2 == rtn5);  //queryable移除了，但是queryable1并没有移除

            Del();
        }

        /// <summary>
        /// 移除指定CacheType（缓存类型LongCount）的缓存
        /// </summary>
        [Fact]
        public void TestRemove2()
        {
            Add();

            //查询缓存，不过期
            var rtn = queryable.LongCountCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn == rtn1);

            //不同的IQueryable
            var rtn2 = queryable1.LongCountCache<Person, Person>(null);
            Add();
            var rtn3 = queryable1.LongCountCache<Person, Person>(null);
            Assert.True(rtn2 == rtn3);

            Assert.True(rtn != rtn2);

            //清理缓存(移除指定缓存类型的：LongCount)
            EFHelper.Services.Cache.LongCountRemove<Person>();
            var rtn4 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn != rtn4);

            var rtn5 = queryable1.LongCountCache<Person, Person>(null);
            Assert.True(rtn2 != rtn5);

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
            var rtn = queryable.LongCountCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn == rtn1);

            //不同的IQueryable
            var rtn2 = queryable1.LongCountCache<Person, Person>(null);
            Add();
            var rtn3 = queryable1.LongCountCache<Person, Person>(null);
            Assert.True(rtn2 == rtn3);

            Assert.True(rtn != rtn2);

            //清理缓存(移除整个表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn != rtn4);

            var rtn5 = queryable1.LongCountCache<Person, Person>(null);
            Assert.True(rtn2 != rtn5);

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
            var rtn = queryable.LongCountCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn == rtn1);

            var artn = queryAddr.LongCountCache<Address, Address>(null);
            AddAddr();
            var artn1 = queryAddr.LongCountCache<Address, Address>(null);
            Assert.True(artn == artn1);

            //清理缓存(移除指定表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = queryable.LongCountCache<Person, Person>(null);
            Assert.True(rtn != rtn4);

            var artn2 = queryAddr.LongCountCache<Address, Address>(null);
            Assert.True(artn == artn2); //Address表的缓存还没有移除


            Del();
            DelAddr();
        }


    }
}
