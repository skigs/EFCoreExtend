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
    /// lua的SqlParameters相关的函数测试：each
    /// </summary>
    public class TestLuaForeach
    {
        //static DbContext db = new MSSqlDBContext();
        static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        LuaPersonEachBLL p = new LuaPersonEachBLL(db);

        static TestLuaForeach()
        {
            EFHelper.ServiceBuilder.AddLuaSqlDefault().BuildServices(); //添加lua sql模块
            //加载指定的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadFile(Directory.GetCurrentDirectory() + "/Datas/Lua/ParamFunc/PersonEach.lua");

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

        /// <summary>
        /// 遍历list
        /// </summary>
        [Fact]
        public void Test()
        {
            p.Delete();

            Assert.True(p.Add1() > 0);
            Assert.True(p.Add11() > 0);
            Assert.True(p.Add12() > 0);
            Assert.True(p.Add13() > 0);

            Assert.True(p.Update() > 0);
            Assert.True(p.Update1() > 0);
            Assert.True(p.Update2() > 0);
            Assert.True(p.Update3() > 0);

            Assert.True(p.Delete() > 0);
        }

        /// <summary>
        /// 遍历dictionary
        /// </summary>
        [Fact]
        public void Test1()
        {
            p.Delete();

            Assert.True(p.Add2() > 0);
            Assert.True(p.Add21() > 0);
            Assert.True(p.Add22() > 0);
            Assert.True(p.Add23() > 0);
            Assert.True(p.Add24() > 0);

            Assert.True(p.Delete() > 0);
        }

        /// <summary>
        /// 遍历model
        /// </summary>
        [Fact]
        public void Test2()
        {
            p.Delete();

            Assert.True(p.Add3() > 0);
            Assert.True(p.Add31() > 0);
            Assert.True(p.Add32() > 0);
            Assert.True(p.Add33() > 0);
            Assert.True(p.Add34() > 0);

            Assert.True(p.Delete() > 0);
        }

    }
}
