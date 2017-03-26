using EFCoreExtend;
using EFCoreExtend.Commons;
using EFCoreExtend.Test;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    /// <summary>
    /// 查询缓存与清理测试
    /// </summary>
    public class TestLuaCacheAndClear
    {
        static DbContext db = new MSSqlDBContext();
        //static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        LuaPersonCacheBLL p = new LuaPersonCacheBLL(db);
        LuaAddressCacheBLL a = new LuaAddressCacheBLL(db);

        static TestLuaCacheAndClear()
        {
            ////使用Redis进行缓存
            //EFHelper.ServiceBuilder.AddQueryCacheCreator(sp =>
            //    new Redis.EFCache.RedisQueryCacheCreator("127.0.0.1:6379,allowAdmin=true"))
            //    .BuildServices();    //重新编译服务;
            ////移除缓存，以免影响测试
            //EFHelper.Services.Cache.Remove<Person>();
            //EFHelper.Services.Cache.Remove<Address>();

            EFHelper.ServiceBuilder.AddLuaSqlDefault()  //添加lua sql模块
                .BuildServices();    //重新编译服务
            //加载指定的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/Cache");

            //lua sql日志记录策略执行器
            luasql.SetLogPolicyExecutor(
                (tableName, sqlName, sql, sqlparam) =>
                {
                    Assert.NotEmpty(tableName);
                    Assert.NotEmpty(sqlName);
                    Assert.NotEmpty(sql);
                    Console.WriteLine(sql + ";  params:" +
                        (sqlparam?.Count > 0 ? sqlparam.JoinToString(",", l => l.ParameterName + "=" + l.Value) : "") + "\r\n");
                }, true, false);
        }

        void TestInit(LuaPersonCacheBLL p = null, LuaAddressCacheBLL a = null)
        {
            p = p ?? this.p;
            a = a ?? this.a;
            //先清理旧数据 和 添加新数据
            p.Delete();
            a.Delete();
            Assert.True(p.Add() > 0);
            Assert.True(a.Add() > 0);
        }

        void TestDelete(LuaPersonCacheBLL p = null, LuaAddressCacheBLL a = null)
        {
            p = p ?? this.p;
            a = a ?? this.a;
            //先清理旧数据 和 添加新数据
            p.Delete();
            a.Delete();
        }

        /// <summary>
        /// 查询缓存
        /// </summary>
        [Fact]
        public void Test()
        {
            TestInit();

            //查询缓存测试
            var p1 = p.Get();
            var count = p.Count();
            Assert.True(p1?.Count > 0);
            Assert.True(count > 0);
            //再添加
            Assert.True(p.Add() > 0);

            //因为配置了查询缓存，而且Add并没有进行清理缓存
            var get = p.Get();
            Assert.True(p1?.Count == get?.Count);
            var count1 = p.Count();
            Assert.True(count == count1);

            TestDelete();
        }

        /// <summary>
        /// 查询缓存过期期限
        /// </summary>
        [Fact]
        public void Test1()
        {
            TestInit();

            var count = p.Count();
            Assert.True(count == p.Count());
            //再添加
            Assert.True(p.Add() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(count == p.Count());

            Thread.Sleep(5000);
            //Count 5秒之后缓存失效
            Assert.True(count != p.Count());

            TestDelete();
        }

        /// <summary>
        /// 查询缓存缓存期限更新
        /// </summary>
        [Fact]
        public void Test11()
        {
            TestInit();

            //查询缓存测试
            var a1 = a.Get();
            Assert.True(a1?.Count > 0);
            //再添加
            Assert.True(a.Add() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(a1?.Count == a.Get()?.Count);

            Thread.Sleep(3000);
            //每次获取之后都会更新缓存期限
            Assert.True(a1?.Count == a.Get()?.Count);

            Thread.Sleep(3000);
            Assert.True(a1?.Count == a.Get()?.Count);

            //5秒后过期
            Thread.Sleep(5000);
            Assert.True(a1?.Count != a.Get()?.Count);

            TestDelete();
        }

        /// <summary>
        /// 查询缓存清理
        /// </summary>
        [Fact]
        public void Test2()
        {
            TestInit();

            //查询缓存测试
            var p1 = p.Get();
            var count = p.Count();
            Assert.True(p1?.Count > 0);
            Assert.True(count > 0);
            //再添加
            Assert.True(p.Add() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(p1?.Count == p.Get()?.Count);
            Assert.True(count == p.Count());

            //Update 会进行缓存清理
            Assert.True(p.Update() > 0);
            Assert.True(p1?.Count != p.Get()?.Count);
            Assert.True(count != p.Count());

            TestDelete();
        }

        /// <summary>
        /// 查询缓存清理（使用Scalar进行新增操作的，用于获取自增id）
        /// </summary>
        [Fact]
        public void Test21()
        {
            DbContext db = new MSSqlDBContext();
            var p = new LuaPersonCacheBLL(db);
            var a = new LuaAddressCacheBLL(db);
            TestInit(p, a);

            //查询缓存测试
            var p1 = p.Get();
            var count = p.Count();
            Assert.True(p1?.Count > 0);
            Assert.True(count > 0);
            //再添加
            Assert.True(p.Add() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(p1?.Count == p.Get()?.Count);
            Assert.True(count == p.Count());

            //AddEx 会进行缓存清理(AddEx是使用Scalar进行新增操作的，用于获取自增id)
            Assert.True(p.AddEx() > 0);
            Assert.True(p1?.Count != p.Get()?.Count);
            Assert.True(count != p.Count());

            TestDelete();
        }

        /// <summary>
        /// 查询缓存清理指定的缓存类型
        /// </summary>
        [Fact]
        public void Test3()
        {
            TestInit();

            //查询缓存测试
            var p1 = p.Get();
            var count = p.Count();
            Assert.True(p1?.Count > 0);
            Assert.True(count > 0);
            //再添加
            Assert.True(p.Add() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(p1?.Count == p.Get()?.Count);
            Assert.True(count == p.Count());

            //Delete 会清理query类型的缓存(Get())，但是不会清理scalar类型的缓存(Count())
            Assert.True(p.Delete() > 0);
            Assert.True(p1?.Count != p.Get()?.Count);
            Assert.True(count == p.Count());

            TestDelete();
        }

        /// <summary>
        /// 查询缓存跨表清理
        /// </summary>
        [Fact]
        public void Test4()
        {
            TestInit();

            //查询缓存测试
            var a1 = a.Get();
            var acount1 = a.Count();
            Assert.True(a1?.Count > 0);
            Assert.True(acount1 > 0);
            //再添加
            Assert.True(a.Add() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(a1?.Count == a.Get()?.Count);
            Assert.True(acount1 == a.Count());

            //p.Update() 会清理 Address表下的所有缓存
            Assert.True(p.Update() > 0);
            Assert.True(a1?.Count != a.Get()?.Count);
            Assert.True(acount1 != a.Count());

            TestDelete();
        }

        /// <summary>
        /// 查询缓存跨表清理指定的缓存类型
        /// </summary>
        [Fact]
        public void Test5()
        {
            TestInit();

            //查询缓存测试
            var a1 = a.Get();
            var a11 = a.Get1();
            var acount1 = a.Count();
            Assert.True(a1?.Count > 0);
            Assert.True(a11?.Count > 0);
            Assert.True(acount1 > 0);
            //再添加
            Assert.True(a.Add() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(a1?.Count == a.Get()?.Count);
            Assert.True(a11?.Count == a.Get1()?.Count);
            Assert.True(acount1 == a.Count());

            //p.Delete() 会清理 Address表下的query类型的缓存(Get())
            Assert.True(p.Delete() > 0);
            Assert.True(a1?.Count != a.Get()?.Count);
            Assert.True(a11?.Count != a.Get1()?.Count);
            Assert.True(acount1 == a.Count());

            TestDelete();
        }

        /// <summary>
        /// 多线程测试
        /// </summary>
        [Fact]
        public void TestThread()
        {
            DbContext db = new MSSqlDBContext();
            var p = new LuaPersonCacheBLL(db);
            var a = new LuaAddressCacheBLL(db);
            TestInit(p, a);

            List<Task> ts = new List<Task>();
            int forcount = 100;
            for (int i = 0; i < forcount; i++)
            {
                ts.Add(Task.Run(() =>
                {
                    DbContext db1 = new MSSqlDBContext();
                    var pt = new LuaPersonCacheBLL(db1);
                    var at = new LuaAddressCacheBLL(db1);
                    pt.Add();
                    pt.Count();
                    pt.Update();
                    pt.Delete();

                    at.Add();
                    at.Count();
                    at.Update();
                    at.Delete();
                }));
            }
            Task.WaitAll(ts.ToArray());

            TestDelete(p, a);
        }

    }
}
