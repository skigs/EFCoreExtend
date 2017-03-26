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
    /// 分部页（布局页）测试
    /// </summary>
    public class TestLuaLayout
    {
        //static DbContext db = new MSSqlDBContext();
        static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        LuaPersonLayoutBLL p = new LuaPersonLayoutBLL(db);

        static TestLuaLayout()
        {
            EFHelper.ServiceBuilder.AddLuaSqlDefault().BuildServices(); //添加lua sql模块
            //加载指定的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/Layout");

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

        [Fact]
        public void Test()
        {
            p.Delete();
            Assert.True(p.Add() > 0);

            var count = p.Count();
            Assert.True(count > 0);
            Assert.True(p.Get()?.Count > 0);
            Assert.True(p.Get1()?.Count > 0);
            Assert.True(p.Get2()?.Count > 0);
            Assert.True(p.Update() > 0);
            Assert.True(p.Update1() > 0);
            Assert.True(p.Update2() > 0);

            Assert.True(p.Delete() > 0);
        }

        [Fact]
        public void Test1()
        {
            p.Delete();
            Assert.True(p.Add() > 0);

            var count = p.Count();
            var p1 = p.Get();
            var p2 = p.Get1();
            var p3 = p.Get2();

            Assert.True(p.Add() > 0);
            //缓存
            Assert.True(count == p.Count());
            Assert.True(p1?.Count == p.Get()?.Count);
            Assert.True(p2?.Count == p.Get1()?.Count);
            Assert.True(p3?.Count == p.Get2()?.Count);

            Assert.True(p.Delete() > 0);
        }

        [Fact]
        public void Test2()
        {
            p.Delete();
            Assert.True(p.Add() > 0);

            var count = p.Count();
            var p1 = p.Get();
            var p2 = p.Get1();
            var p3 = p.Get2();
            Assert.True(p.Add() > 0);
            Assert.True(count == p.Count());
            Assert.True(p1?.Count == p.Get()?.Count);
            Assert.True(p2?.Count == p.Get1()?.Count);
            Assert.True(p3?.Count == p.Get2()?.Count);

            //缓存过期
            Thread.Sleep(6000);
            Assert.True(count != p.Count());
            Assert.True(p1?.Count != p.Get()?.Count);
            Assert.True(p2?.Count != p.Get1()?.Count);
            Assert.True(p3?.Count != p.Get2()?.Count);

            Assert.True(p.Delete() > 0);
        }

        [Fact]
        public void Test3()
        {
            p.Delete();
            Assert.True(p.Add() > 0);

            var count = p.Count();
            var p1 = p.Get();
            var p2 = p.Get1();
            var p3 = p.Get2();
            Assert.True(p.Add() > 0);
            Assert.True(count == p.Count());
            Assert.True(p1?.Count == p.Get()?.Count);
            Assert.True(p2?.Count == p.Get1()?.Count);
            Assert.True(p3?.Count == p.Get2()?.Count);

            //缓存清理
            p.Update();     //只清理query类型的
            Assert.True(p1?.Count != p.Get()?.Count);
            Assert.True(p2?.Count == p.Get1()?.Count);
            Assert.True(p3?.Count == p.Get2()?.Count);

            p.Update1();     //只清理query1类型的
            Assert.True(p2?.Count != p.Get1()?.Count);
            Assert.True(p3?.Count == p.Get2()?.Count);

            p.Update2();     //只清理query2类型的
            Assert.True(p3?.Count != p.Get2()?.Count);

            Assert.True(p.Delete() > 0);
        }

    }
}
