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
    /// IQueryable.FirstOrDefault查询缓存测试
    /// </summary>
    public class TestEFFirstCache
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

        void DelAll()
        {
            var rtn = db.NonQuery($"delete from {nameof(Person)}");
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

        static TestEFFirstCache()
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
            var val = queryable.FirstOrDefaultCache<Person, Person>(null);
            Del();
            Add();
            var val1 = queryable.FirstOrDefaultCache(typeof(Person), null);
            var val2 = queryable.FirstOrDefaultCache(nameof(Person), null);
            Assert.NotNull(val);
            Assert.True(val.id == val1.id);
            Assert.True(val.id == val2.id);

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
            var val = queryable.FirstOrDefaultCache<Person, Person>(expiry);
            Del();
            Add();
            var val1 = queryable.FirstOrDefaultCache<Person, Person>(expiry);
            Assert.NotNull(val);
            Assert.True(val.id == val1.id);

            Thread.Sleep(3100);
            var val2 = queryable.FirstOrDefaultCache<Person, Person>(expiry);
            Assert.True(val.id != val2.id);

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
            var val = queryable.FirstOrDefaultCache<Person, Person>(expiry);
            Del();
            Add();
            var val1 = queryable.FirstOrDefaultCache<Person, Person>(expiry);
            Assert.NotNull(val);
            Assert.True(val.id == val1.id);

            Thread.Sleep(2000);
            var val2 = queryable.FirstOrDefaultCache<Person, Person>(expiry);
            Assert.True(val.id == val2.id);

            Thread.Sleep(2000);
            var val3 = queryable.FirstOrDefaultCache<Person, Person>(expiry);
            Assert.True(val.id == val3.id);

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
            var rtn = queryable.FirstOrDefaultCache<Person, Person>(null);
            Del();
            Add();
            var rtn1 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id == rtn1.id);

            //清理缓存
            queryable.FirstOrDefaultCacheRemove<Person>();
            var rtn2 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id != rtn2.id);

            Del();
        }

        /// <summary>
        /// 移除指定key（IQueryable）的缓存
        /// </summary>
        [Fact]
        public void TestRemove1()
        {
            DelAll();   //先删除所有的数据，避免影响测试（因为FirstOrDefault只会获取第一个数据）
            Add();

            //查询缓存，不过期
            var rtn = queryable.FirstOrDefaultCache<Person, Person>(null);
            Del();
            Add();
            var rtn1 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id == rtn1.id);

            //不同的IQueryable
            var rtn2 = queryable1.FirstOrDefaultCache<Person, Person>(null);
            Del();
            Add();
            var rtn3 = queryable1.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id != rtn2.id);
            Assert.True(rtn2.id == rtn3.id);

            //清理缓存
            queryable.FirstOrDefaultCacheRemove<Person>();    //移除queryable的缓存
            var rtn4 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id != rtn4.id);

            var rtn5 = queryable1.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn2.id == rtn5.id);  //queryable移除了，但是queryable1并没有移除

            Del();
        }

        /// <summary>
        /// 移除指定CacheType（缓存类型FirstOrDefault）的缓存
        /// </summary>
        [Fact]
        public void TestRemove2()
        {
            DelAll();   //先删除所有的数据，避免影响测试（因为FirstOrDefault只会获取第一个数据）
            Add();

            //查询缓存，不过期
            var rtn = queryable.FirstOrDefaultCache<Person, Person>(null);
            Del();
            Add();
            var rtn1 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id == rtn1.id);

            //不同的IQueryable
            var rtn2 = queryable1.FirstOrDefaultCache<Person, Person>(null);
            Del();
            Add();
            var rtn3 = queryable1.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn2.id == rtn3.id);

            Assert.True(rtn.id != rtn2.id);

            //清理缓存(移除指定缓存类型的：FirstOrDefault)
            EFHelper.Services.Cache.FirstOrDefaultRemove<Person>();
            var rtn4 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id != rtn4.id);

            var rtn5 = queryable1.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn2.id != rtn5.id);

            Del();
        }

        /// <summary>
        /// 移除指定表的缓存
        /// </summary>
        [Fact]
        public void TestRemove4()
        {
            DelAll();   //先删除所有的数据，避免影响测试（因为FirstOrDefault只会获取第一个数据）
            Add();

            //查询缓存，不过期
            var rtn = queryable.FirstOrDefaultCache<Person, Person>(null);
            Del();
            Add();
            var rtn1 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id == rtn1.id);

            //不同的IQueryable
            var rtn2 = queryable1.FirstOrDefaultCache<Person, Person>(null);
            Del();
            Add();
            var rtn3 = queryable1.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn2.id == rtn3.id);

            Assert.True(rtn.id != rtn2.id);

            //清理缓存(移除整个表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id != rtn4.id);

            var rtn5 = queryable1.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn2.id != rtn5.id);

            Del();
        }

        /// <summary>
        /// 移除指定表的缓存
        /// </summary>
        [Fact]
        public void TestRemove5()
        {
            DelAll();   //先删除所有的数据，避免影响测试（因为FirstOrDefault只会获取第一个数据）
            Add();
            AddAddr();

            //查询缓存，不过期
            var rtn = queryable.FirstOrDefaultCache<Person, Person>(null);
            Del();
            Add();
            var rtn1 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id == rtn1.id);

            var artn = queryAddr.FirstOrDefaultCache<Address, Address>(null);
            DelAddr();
            AddAddr();
            var artn1 = queryAddr.FirstOrDefaultCache<Address, Address>(null);
            Assert.True(artn.id == artn1.id);

            //清理缓存(移除指定表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = queryable.FirstOrDefaultCache<Person, Person>(null);
            Assert.True(rtn.id != rtn4.id);

            var artn2 = queryAddr.FirstOrDefaultCache<Address, Address>(null);
            Assert.True(artn.id == artn2.id); //Address表的缓存还没有移除


            Del();
            DelAddr();
        }

    }
}
