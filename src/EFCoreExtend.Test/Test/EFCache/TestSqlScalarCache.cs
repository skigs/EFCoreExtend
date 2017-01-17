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
    /// scalar查询缓存测试
    /// </summary>
    public class TestSqlScalarCache
    {
        static DbContext db = new MSSqlDBContext();
        #region 测试的参数
        static string name = "efcache";
        static string fullAddress = "efaddr";
        static string sql = $"select count(*) from {nameof(Person)} where name=@name";
        static string sql1 = $"select count(id) from {nameof(Person)} where name=@name";
        static string sqlAddr = $"select count(*) from {nameof(Address)} where fullAddress=@fullAddress";

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

        static TestSqlScalarCache()
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
            var val = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Add();
            var val1 = db.ScalarCacheUseDict(typeof(Person), sql, new Dictionary<string, object>() { { "name", name } }, null);
            var val2 = db.ScalarCache(nameof(Person), sql, new[] { new SqlParameter("name", name) }, null);
            Assert.NotNull(val);
            Assert.True(val.ToString() == val1.ToString());
            Assert.True(val.ToString() == val2.ToString());

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
            var val = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, expiry);
            Add();
            var val1 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, expiry);
            Assert.NotNull(val);
            Assert.True(val.ToString() == val1.ToString());

            Thread.Sleep(3100);
            var val2 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, expiry);
            Assert.True(val.ToString() != val2.ToString());

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
            var val = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, expiry);
            Add();
            var val1 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, expiry);
            Assert.NotNull(val);
            Assert.True(val.ToString() == val1.ToString());

            Thread.Sleep(2000);
            var val2 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, expiry);
            Assert.True(val.ToString() == val2.ToString());

            Thread.Sleep(2000);
            var val3 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, expiry);
            Assert.True(val.ToString() == val3.ToString());

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
            var rtn = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Add();
            var rtn1 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() == rtn1.ToString());

            //清理缓存
            db.ScalarCacheRemoveUseModel<Person>(sql, new { name = name }, null);
            var rtn2 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() != rtn2.ToString());

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
            var rtn = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Add();
            var rtn1 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() == rtn1.ToString());

            //不同的IQueryable
            var rtn2 = db.ScalarCacheUseModel<Person>(sql1, new { name = name }, null, null);
            Add();
            var rtn3 = db.ScalarCacheUseModel<Person>(sql1, new { name = name }, null, null);
            Assert.True(rtn.ToString() != rtn2.ToString());
            Assert.True(rtn2.ToString() == rtn3.ToString());

            //清理缓存
            db.ScalarCacheRemoveUseModel<Person>(sql, new { name = name }, null);    //移除queryable的缓存
            var rtn4 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() != rtn4.ToString());

            var rtn5 = db.ScalarCacheUseModel<Person>(sql1, new { name = name }, null, null);
            Assert.True(rtn2.ToString() == rtn5.ToString());  //queryable移除了，但是queryable1并没有移除

            Del();
        }

        /// <summary>
        /// 移除指定CacheType（缓存类型scalar）的缓存
        /// </summary>
        [Fact]
        public void TestRemove2()
        {
            Add();

            //查询缓存，不过期
            var rtn = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Add();
            var rtn1 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() == rtn1.ToString());

            //不同的IQueryable
            var rtn2 = db.ScalarCacheUseModel<Person>(sql1, new { name = name }, null, null);
            Add();
            var rtn3 = db.ScalarCacheUseModel<Person>(sql1, new { name = name }, null, null);
            Assert.True(rtn2.ToString() == rtn3.ToString());

            Assert.True(rtn.ToString() != rtn2.ToString());

            //清理缓存(移除指定缓存类型的：scalar)
            EFHelper.Services.Cache.ScalarRemove<Person>();
            var rtn4 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() != rtn4.ToString());

            var rtn5 = db.ScalarCacheUseModel<Person>(sql1, new { name = name }, null, null);
            Assert.True(rtn2.ToString() != rtn5.ToString());

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
            var rtn = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Add();
            var rtn1 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() == rtn1.ToString());

            //不同的IQueryable
            var rtn2 = db.ScalarCacheUseModel<Person>(sql1, new { name = name }, null, null);
            Add();
            var rtn3 = db.ScalarCacheUseModel<Person>(sql1, new { name = name }, null, null);
            Assert.True(rtn2.ToString() == rtn3.ToString());

            Assert.True(rtn.ToString() != rtn2.ToString());

            //清理缓存(移除整个表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() != rtn4.ToString());

            var rtn5 = db.ScalarCacheUseModel<Person>(sql1, new { name = name }, null, null);
            Assert.True(rtn2.ToString() != rtn5.ToString());

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
            var rtn = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Add();
            var rtn1 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() == rtn1.ToString());

            var artn = db.ScalarCacheUseModel<Address>(sqlAddr, new { fullAddress = fullAddress }, null, null);
            AddAddr();
            var artn1 = db.ScalarCacheUseModel<Address>(sqlAddr, new { fullAddress = fullAddress }, null, null);
            Assert.True(artn.ToString() == artn1.ToString());

            //清理缓存(移除指定表下的缓存：Person)
            EFHelper.Services.Cache.Remove<Person>();
            var rtn4 = db.ScalarCacheUseModel<Person>(sql, new { name = name }, null, null);
            Assert.True(rtn.ToString() != rtn4.ToString());

            var artn2 = db.ScalarCacheUseModel<Address>(sqlAddr, new { fullAddress = fullAddress }, null, null);
            Assert.True(artn.ToString() == artn2.ToString()); //Address表的缓存还没有移除


            Del();
            DelAddr();
        }


    }
}
