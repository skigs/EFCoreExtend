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
    /// IQueryable.ToList查询缓存测试
    /// </summary>
    public class TestEFListCache
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

        static TestEFListCache()
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
            var list = queryable.ListCache<Person, Person>(null);
            Add();
            var list1 = queryable.ListCache(typeof(Person), null);
            var list2 = queryable.ListCache(nameof(Person), null);
            Assert.True(list?.Count > 0);
            Assert.True(list?.Count == list1?.Count);
            Assert.True(list?.Count == list2?.Count);

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
            var list = queryable.ListCache<Person, Person>(expiry);
            Add();
            var list1 = queryable.ListCache<Person, Person>(expiry);
            Assert.True(list?.Count > 0);
            Assert.True(list?.Count == list1?.Count);

            Thread.Sleep(3100);
            var list2 = queryable.ListCache<Person, Person>(expiry);
            Assert.True(list?.Count != list2?.Count);

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
            var list = queryable.ListCache<Person, Person>(expiry);
            Add();
            var list1 = queryable.ListCache<Person, Person>(expiry);
            Assert.True(list?.Count > 0);
            Assert.True(list?.Count == list1?.Count);

            Thread.Sleep(2000);
            var list2 = queryable.ListCache<Person, Person>(expiry);
            Assert.True(list?.Count == list2?.Count);

            Thread.Sleep(2000);
            var list3 = queryable.ListCache<Person, Person>(expiry);
            Assert.True(list?.Count == list3?.Count);

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
            var rtn = queryable.ListCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count == rtn1?.Count);

            //清理缓存
            queryable.ListCacheRemove<Person>();
            var rtn2 = queryable.ListCache<Person, Person>(null);
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
            var rtn = queryable.ListCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count == rtn1?.Count);

            //不同的IQueryable
            var rtn2 = queryable1.ListCache<Person, Person>(null);
            Add();
            var rtn3 = queryable1.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count != rtn2?.Count);
            Assert.True(rtn2?.Count == rtn3?.Count);

            //清理缓存
            queryable.ListCacheRemove<Person>();    //移除queryable的缓存
            var rtn4 = queryable.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count != rtn4?.Count);

            var rtn5 = queryable1.ListCache<Person, Person>(null);
            Assert.True(rtn2?.Count == rtn5?.Count);  //queryable移除了，但是queryable1并没有移除

            Del();
        }

        /// <summary>
        /// 移除指定CacheType（缓存类型List）的缓存
        /// </summary>
        [Fact]
        public void TestRemove2()
        {
            Add();

            //查询缓存，不过期
            var rtn = queryable.ListCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count == rtn1?.Count);

            //不同的IQueryable
            var rtn2 = queryable1.ListCache<Person, Person>(null);
            Add();
            var rtn3 = queryable1.ListCache<Person, Person>(null);
            Assert.True(rtn2?.Count == rtn3?.Count);

            Assert.True(rtn?.Count != rtn2?.Count);

            //清理缓存(移除指定缓存类型的：List)
            EFHelper.Services.Cache.ListRemove<Person>();
            var rtn4 = queryable.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count != rtn4?.Count);

            var rtn5 = queryable1.ListCache<Person, Person>(null);
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
            var rtn = queryable.ListCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count == rtn1?.Count);

            //不同的IQueryable
            var rtn2 = queryable1.ListCache<Person, Person>(null);
            Add();
            var rtn3 = queryable1.ListCache<Person, Person>(null);
            Assert.True(rtn2?.Count == rtn3?.Count);

            Assert.True(rtn?.Count != rtn2?.Count);

            //清理缓存(移除整个表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = queryable.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count != rtn4?.Count);

            var rtn5 = queryable1.ListCache<Person, Person>(null);
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
            var rtn = queryable.ListCache<Person, Person>(null);
            Add();
            var rtn1 = queryable.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count == rtn1?.Count);

            var artn = queryAddr.ListCache<Address, Address>(null);
            AddAddr();
            var artn1 = queryAddr.ListCache<Address, Address>(null);
            Assert.True(artn?.Count == artn1?.Count);

            //清理缓存(移除指定表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = queryable.ListCache<Person, Person>(null);
            Assert.True(rtn?.Count != rtn4?.Count);

            var artn2 = queryAddr.ListCache<Address, Address>(null);
            Assert.True(artn?.Count == artn2?.Count); //Address表的缓存还没有移除


            Del();
            DelAddr();
        }


    }
}
