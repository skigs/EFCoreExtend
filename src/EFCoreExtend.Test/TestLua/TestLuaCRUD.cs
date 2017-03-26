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
    /// 普通的增删改查测试
    /// </summary>
    public class TestLuaCRUD
    {
        static DbContext db = new MSSqlDBContext();
        //static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        LuaPersonBLL p = new LuaPersonBLL(db);

        static TestLuaCRUD()
        {
            EFHelper.ServiceBuilder.AddLuaSqlDefault().BuildServices(); //添加lua sql模块
            //加载指定的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadFile(Directory.GetCurrentDirectory() + "/Datas/Lua/Person.lua");

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
            Assert.True(p.Count() == 0);

            Assert.True(p.Add() > 0);

            Assert.True(p.Get()?.Count > 0);
            //Assert.True(p.Get1<SqlParameter>()?.Count > 0);
            Assert.True(p.Get1<SqliteParameter>()?.Count > 0);
            //Assert.True(p.Get1<MySqlParameter>()?.Count > 0);
            //Assert.True(p.Get1<NpgsqlParameter>()?.Count > 0);
            Assert.True(p.Get2()?.Count > 0);

            Assert.True(p.Count() > 0);
            //Assert.True(p.Count1<SqlParameter>() > 0);
            Assert.True(p.Count1<SqliteParameter>() > 0);
            //Assert.True(p.Count1<MySqlParameter>() > 0);
            //Assert.True(p.Count1<NpgsqlParameter>() > 0);
            Assert.True(p.Count2() > 0);

            Assert.True(p.Update() > 0);
            //Assert.True(p.Update1<SqlParameter>() > 0);
            Assert.True(p.Update1<SqliteParameter>() > 0);
            //Assert.True(p.Update1<MySqlParameter>() > 0);
            //Assert.True(p.Update1<NpgsqlParameter>() > 0);
            Assert.True(p.Update2() > 0);

            Assert.True(p.Delete() > 0);
        }

        [Fact]
        public void Test1()
        {
            try
            {
                //设置的类型和执行的类型不一样，那么抛异常
                p.GetException();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message.StartsWith("The sql type"));
            }
            try
            {
                p.CountException();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message.StartsWith("The sql type"));
            }
            try
            {
                p.AddException();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message.StartsWith("The sql type"));
            }
        }

        /// <summary>
        /// 多线程测试
        /// </summary>
        [Fact]
        public void TestThread()
        {
            LuaPersonBLL p = new LuaPersonBLL(new MSSqlDBContext());
            p.Delete();
            Assert.True(p.Add() > 0);

            int count = p.Get().Count;
            Assert.True(count > 0);
            int ic = 0;
            List<Task> ts = new List<Task>();
            int forcount = 90;
            for (int i = 0; i < forcount; i++)
            {
                ts.Add(Task.Run(() =>
                {
                    LuaPersonBLL pt = new LuaPersonBLL(new MSSqlDBContext());
                    var pcount = pt.Get().Count;
                    if (count == pcount)
                    {
                        Interlocked.Increment(ref ic);
                    }
                }));
            }
            Task.WaitAll(ts.ToArray());
            Assert.True(ic == forcount);

            Assert.True(p.Delete() > 0);
        }

        /// <summary>
        /// 事物测试
        /// </summary>
        [Fact]
        public void Trans1()
        {
            int oldAddrid = 123, newAddrid = 345;
            p.Delete();
            Assert.True(p.Add() > 0);
            Assert.True(p.Update(oldAddrid) > 0);

            bool bRtn = true;   //模拟事物提交成功
            db.Database.BeginTransaction();
            Assert.True(p.Get()[0].addrid == oldAddrid);
            Assert.True(p.Update(newAddrid) > 0);
            if (bRtn)
                db.Database.CommitTransaction();
            else
                db.Database.RollbackTransaction();

            Assert.True(p.Get()[0].addrid == newAddrid);
        }

        /// <summary>
        /// 事物测试
        /// </summary>
        [Fact]
        public void Trans2()
        {
            int oldAddrid = 123, newAddrid = 345;
            p.Delete();
            Assert.True(p.Add() > 0);
            Assert.True(p.Update(oldAddrid) > 0);

            bool bRtn = false;   //模拟事物提交失败
            db.Database.BeginTransaction();
            Assert.True(p.Get()[0].addrid == oldAddrid);
            Assert.True(p.Update(newAddrid) > 0);
            if (bRtn)
                db.Database.CommitTransaction();
            else
                db.Database.RollbackTransaction();

            Assert.True(p.Get()[0].addrid == oldAddrid);
        }

    }
}

