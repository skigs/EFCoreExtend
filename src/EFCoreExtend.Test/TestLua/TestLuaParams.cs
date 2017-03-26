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
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    /// <summary>
    /// lua的SqlParameters相关的函数测试：params
    /// </summary>
    public class TestLuaParams
    {
        static DbContext db = new MSSqlDBContext();
        //static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        LuaPersonParamsBLL p = new LuaPersonParamsBLL(db);

        static TestLuaParams()
        {
            EFHelper.ServiceBuilder.AddLuaSqlDefault().BuildServices(); //添加lua sql模块
            //加载指定的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadFile(Directory.GetCurrentDirectory() + "/Datas/Lua/ParamFunc/PersonParams.lua");

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
            Assert.True(p.Add1() > 0);
            Assert.True(p.Add11() > 0);
            Assert.True(p.Add12() > 0);
            Assert.True(p.Add13() > 0);
            Assert.True(p.Add2() > 0);
            Assert.True(p.Add21() > 0);
            Assert.True(p.Add3() > 0);

            Assert.True(p.Update(null) > 0);
            Assert.True(p.Update(123) > 0);
            Assert.True(p.Update1(null) > 0);
            Assert.True(p.Update1(123) > 0);
            Assert.True(p.Update2() > 0);
            Assert.True(p.Update21() > 0);

            Assert.True(p.Delete() > 0);
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

            List<Task> ts = new List<Task>();
            int forcount = 100;
            for (int i = 0; i < forcount; i++)
            {
                ts.Add(Task.Run(() =>
                {
                    var pt = new LuaPersonParamsBLL(new MSSqlDBContext());
                    Assert.True(pt.Add() > 0);
                    Assert.True(pt.Add1() > 0);
                    Assert.True(pt.Add11() > 0);
                    Assert.True(pt.Add12() > 0);
                    Assert.True(pt.Add13() > 0);
                }));
                ts.Add(Task.Run(() =>
                {
                    var pt = new LuaPersonParamsBLL(new MSSqlDBContext());
                    Assert.True(pt.Add2() > 0);
                    Assert.True(pt.Add21() > 0);
                    Assert.True(pt.Add3() > 0);
                }));
            }
            Task.WaitAll(ts.ToArray());

            Assert.True(p.Delete() > 0);
        }

    }
}

