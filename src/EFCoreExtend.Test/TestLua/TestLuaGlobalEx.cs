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
    /// 全局配置测试
    /// </summary>
    public class TestLuaGlobalEx
    {
        //static DbContext db = new MSSqlDBContext();
        static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        LuaPersonGlobalExBLL p = new LuaPersonGlobalExBLL(db);

        static TestLuaGlobalEx()
        {
            EFHelper.ServiceBuilder.AddLuaSqlDefault().BuildServices(); //添加lua sql模块
            //加载指定的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/Global");

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

        void TestInit()
        {
            //先清理旧数据 和 添加新数据
            p.Delete();
            Assert.True(p.Add() > 0);
        }

        void TestDelete()
        {
            //先清理旧数据 和 添加新数据
            p.Delete();
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
            var p3 = p.Get2();
            Assert.True(p1?.Count > 0);
            Assert.True(p3?.Count > 0);

            var count = p.Count();
            Assert.True(count > 0);
            //再添加
            Assert.True(p.Add1() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(p1?.Count == p.Get()?.Count);
            Assert.True(p3?.Count == p.Get2()?.Count);
            Assert.True(count == p.Count());

            TestDelete();
        }

        /// <summary>
        /// 查询缓存
        /// </summary>
        [Fact]
        public void TestException()
        {
            TestInit();

            bool hasEx = false;
            try
            {
                var p2 = p.Get1();
            }
            catch
            {
                hasEx = true;
            }
            Assert.True(hasEx);

            hasEx = false;
            try
            {
                //查询缓存测试
                var p1 = p.Get();
                Assert.True(p1?.Count > 0);
                //再添加
                Assert.True(p.Add() > 0);
                //因为配置了查询缓存，而且Add会进行清理缓存
                Assert.True(p1?.Count == p.Get()?.Count);
            }
            catch
            {
                hasEx = true;
            }
            Assert.True(hasEx);

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
            Assert.True(p.Add1() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(count == p.Count());

            Thread.Sleep(3000);
            //Count 5秒之后缓存失效
            Assert.True(count != p.Count());

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
            Assert.True(p.Add1() > 0);
            //因为配置了查询缓存，而且Add并没有进行清理缓存
            Assert.True(p1?.Count == p.Get()?.Count);
            Assert.True(count == p.Count());

            //Update 会进行缓存清理
            Assert.True(p.Update() > 0);
            Assert.True(p1?.Count != p.Get()?.Count);
            Assert.True(count != p.Count());

            TestDelete();
        }
        
    }
}
