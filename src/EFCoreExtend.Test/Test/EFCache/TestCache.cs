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
    /// 查询缓存测试（EF查询缓存 和 SqlConfig的查询缓存（二级查询缓存）都是使用这个缓存器的：IEFQueryCache）
    /// </summary>
    public class TestCache
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

        static TestCache()
        {
            ////使用Redis进行缓存
            //EFHelper.ServiceBuilder.AddQueryCacheCreator(sp =>
            //    new Redis.EFCache.RedisQueryCacheCreator("127.0.0.1:6379,allowAdmin=true"))
            //    .BuildServices();   //重新编译服务

            ////移除缓存，以免影响测试
            //EFHelper.Services.Cache.Remove<Person>();
            //EFHelper.Services.Cache.Remove<Address>();
        }

        [Fact]
        public void Test()
        {
            Add();

            //查询缓存，不过期
            var rtn = EFHelper.Services.Cache.Cache<Person, int>("TestCache", queryable, 
                () => queryable.Count(), null);
            Add();
            //IQueryable扩展方法(和上面的一样的)
            var rtn1 = queryable.Cache<Person, int>("TestCache", 
                () => queryable.Count(), null);
            Assert.True(rtn == rtn1);
            var rtn2 = queryable.Cache(typeof(Person), "TestCache", 
                () => queryable.Count(), null);
            Assert.True(rtn == rtn2);
            var rtn3 = queryable.Cache(nameof(Person), "TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn == rtn3);

            Del();
        }

        [Fact]
        public void Test0()
        {
            Add();

            //查询缓存，不过期( 其中Func：() => queryable.Sum(l => l.id)是可以返回任意类型数据的，
            //  就是缓存是可以为任意类型的数据的 )
            var rtn = EFHelper.Services.Cache.Cache<Person, int>("MyCacheType", queryable,
                () => queryable.Sum(l => l.id), null);
            Add();
            //IQueryable扩展方法(和上面的一样的)
            var rtn1 = queryable.Cache<Person, int>("MyCacheType",
                () => queryable.Sum(l => l.id), null);
            Assert.True(rtn == rtn1);

            var rtn2 = EFHelper.Services.Cache.Cache<Person, object>("MyCacheType", "MyCacheKey",
                () => null, null);
            Add();
            var rtn21 = EFHelper.Services.Cache.Cache<Person, object>("MyCacheType", "MyCacheKey",
                () => null, null);
            Assert.True(rtn2 == rtn21);

            var rtn3 = EFHelper.Services.Cache.Cache<Person, Dictionary<string, int>>("MyCacheType", 
                "MyCacheKey123",
                () => new Dictionary<string, int>()
                {
                    { "asdf", 123 }
                }, null);
            Add();
            var rtn31 = EFHelper.Services.Cache.Cache<Person, Dictionary<string, int>>("MyCacheType", 
                "MyCacheKey123",
                () => new Dictionary<string, int>()
                {
                    { "asdf", 123 }
                }, null);
            Assert.True(rtn3["asdf"] == rtn31["asdf"]);

            Del();
        }

        [Fact]
        public void Test1()
        {
            Add();

            //查询缓存，不过期
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn == rtn1);

            //不同的IQueryable
            var rtn2 = queryable1.Cache(typeof(Person), "TestCache",
                () => queryable1.Count(), null);
            Add();
            Assert.True(rtn != rtn2);
            var rtn3 = queryable1.Cache(nameof(Person), "TestCache",
                () => queryable1.Count(), null);
            Assert.True(rtn2 == rtn3);

            Del();
        }

        /// <summary>
        /// 缓存过期
        /// </summary>
        [Fact]
        public void TestExpiry()
        {
            Add();

            //查询缓存，过期
            var expiry = new QueryCacheExpiryPolicy(TimeSpan.FromSeconds(3));
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), expiry);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), expiry);
            Assert.True(rtn == rtn1);

            Thread.Sleep(3000);
            var rtn2 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), expiry);
            Assert.True(rtn != rtn2);

            Del();
        }

        /// <summary>
        /// 缓存过期
        /// </summary>
        [Fact]
        public void TestExpiry1()
        {
            Add();

            //查询缓存，过期
            var span = TimeSpan.FromSeconds(3); //QueryCacheExpiryPolicy(TimeSpan.FromSeconds(3));
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), span);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), span);
            Assert.True(rtn == rtn1);

            Thread.Sleep(3000);
            var rtn2 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), span);
            Assert.True(rtn != rtn2);

            Del();
        }

        /// <summary>
        /// 缓存过期
        /// </summary>
        [Fact]
        public void TestExpiry2()
        {
            Add();

            //查询缓存，过期
            var time = DateTime.Now.AddSeconds(3);
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), time);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), time);
            Assert.True(rtn == rtn1);

            Thread.Sleep(3000);
            var rtn2 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), time);
            Assert.True(rtn != rtn2);

            Del();
        }

        /// <summary>
        /// 缓存过期与更新缓存时间
        /// </summary>
        [Fact]
        public void TestExpiry3()
        {
            Add();

            //查询缓存，过期与更新缓存时间
            var expiry = new QueryCacheExpiryPolicy(TimeSpan.FromSeconds(3), true);   //第二个参数设置是否每次调用都更新缓存时间
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), expiry);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), expiry);
            Assert.True(rtn == rtn1);

            Thread.Sleep(2000);
            var rtn2 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), expiry);
            Assert.True(rtn == rtn2);

            Thread.Sleep(2000);
            var rtn3 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), expiry);
            Assert.True(rtn == rtn3);

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
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn == rtn1);

            //清理缓存(只移除指定的IQueryable缓存：queryable)
            queryable.CacheRemove<Person>("TestCache");
            var rtn2 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
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
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn == rtn1);

            //不同的IQueryable
            var rtn2 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Add();
            var rtn3 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Assert.True(rtn != rtn2);
            Assert.True(rtn2 == rtn3);

            //清理缓存(只移除指定的IQueryable缓存：queryable)
            queryable.CacheRemove(typeof(Person), "TestCache");
            var rtn4 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn != rtn4);

            var rtn5 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Assert.True(rtn2 == rtn5);  //queryable移除了，但是queryable1并没有移除

            Del();
        }

        /// <summary>
        /// 移除指定CacheType（缓存类型）的缓存
        /// </summary>
        [Fact]
        public void TestRemove2()
        {
            Add();

            //查询缓存，不过期
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn == rtn1);

            //不同的IQueryable
            var rtn2 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Add();
            var rtn3 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Assert.True(rtn != rtn2);
            Assert.True(rtn2 == rtn3);

            //清理缓存(移除指定缓存类型的：TestCache)
            EFHelper.Services.Cache.Remove<Person>("TestCache");
            var rtn4 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn != rtn4);

            var rtn5 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Assert.True(rtn2 != rtn5);  //queryable和queryable1的CacheType都是：TestCache

            Del();
        }

        /// <summary>
        /// 移除指定CacheType（缓存类型）的缓存
        /// </summary>
        [Fact]
        public void TestRemove3()
        {
            Add();

            //查询缓存，不过期
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn == rtn1);

            //不同的IQueryable
            var rtn2 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Add();
            var rtn3 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Assert.True(rtn != rtn2);
            Assert.True(rtn2 == rtn3);

            //不同的CacheType
            var rtn21 = queryable1.Cache<Person, int>("TestCache123",
                () => queryable1.Count(), null);
            Add();
            var rtn31 = queryable1.Cache<Person, int>("TestCache123",
                () => queryable1.Count(), null);
            Assert.True(rtn != rtn21);
            Assert.True(rtn2 != rtn21);
            Assert.True(rtn21 == rtn31);

            //清理缓存(移除指定类型的：TestCache)
            EFHelper.Services.Cache.Remove(typeof(Person), "TestCache");
            var rtn4 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn != rtn4);

            var rtn5 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Assert.True(rtn2 != rtn5);  //queryable和queryable1的CacheType都是：TestCache

            var rtn51 = queryable1.Cache<Person, int>("TestCache123",
                () => queryable1.Count(), null);
            Assert.True(rtn21 == rtn51);  //CacheType：TestCache1

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
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn == rtn1);

            //不同的IQueryable
            var rtn2 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Add();
            var rtn3 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Assert.True(rtn != rtn2);
            Assert.True(rtn2 == rtn3);

            //不同的CacheType
            var rtn21 = queryable1.Cache<Person, int>("TestCache123",
                () => queryable1.Count(), null);
            Add();
            var rtn31 = queryable1.Cache<Person, int>("TestCache123",
                () => queryable1.Count(), null);
            Assert.True(rtn != rtn21);
            Assert.True(rtn2 != rtn21);
            Assert.True(rtn21 == rtn31);

            //清理缓存(移除整个表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn != rtn4);

            var rtn5 = queryable1.Cache<Person, int>("TestCache",
                () => queryable1.Count(), null);
            Assert.True(rtn2 != rtn5);  //queryable和queryable1的CacheType都是：TestCache

            var rtn51 = queryable1.Cache<Person, int>("TestCache123",
                () => queryable1.Count(), null);
            Assert.True(rtn21 != rtn51);  //CacheType：TestCache1

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
            var rtn = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Add();
            var rtn1 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn == rtn1);

            var artn = queryAddr.Cache<Address, int>("TestCache",
                () => queryAddr.Count(), null);
            AddAddr();
            var artn1 = queryAddr.Cache<Address, int>("TestCache",
                () => queryAddr.Count(), null);
            Assert.True(artn == artn1);

            //清理缓存(移除指定表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = queryable.Cache<Person, int>("TestCache",
                () => queryable.Count(), null);
            Assert.True(rtn != rtn4);
            var artn2 = queryAddr.Cache<Address, int>("TestCache",
                () => queryAddr.Count(), null);
            Assert.True(artn == artn2); //Address表的缓存还没有移除


            Del();
            DelAddr();
        }

    }
}
