using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    /// <summary>
    /// lua的SqlParameters相关的函数测试：param
    /// </summary>
    public class TestLuaParam
    {
        //static DbContext db = new MSSqlDBContext();
        static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        LuaPersonParamBLL p = new LuaPersonParamBLL(db);

        static TestLuaParam()
        {
            EFHelper.ServiceBuilder.AddLuaSqlDefault().BuildServices(); //添加lua sql模块
            //加载指定的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadFile(Directory.GetCurrentDirectory() + "/Datas/Lua/ParamFunc/PersonParam.lua");

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

        string name = "小明Param";
        int addrid = 6574;
        void InitTest(LuaPersonParamBLL pbll = null)
        {
            p = pbll ?? p;
            p.Delete(name);
            p.Delete1(addrid);

            Assert.True(p.Add(name, null) > 0);
            Assert.True(p.Add(name, addrid) > 0);
        }

        void DelTest()
        {
            p.Delete(name);
            p.Delete1(addrid);
        }

        [Fact]
        public void Test()
        {
            InitTest();

            Assert.True(p.Get1(name, null)?.Count > 0);
            Assert.True(p.Get1(name, addrid)?.Count > 0);
            Assert.True(p.Get11(name, null)?.Count > 0);
            Assert.True(p.Get11(name, addrid)?.Count > 0);

            Assert.True(p.Get12(null, addrid)?.Count > 0);
            Assert.True(p.Get12("", addrid)?.Count > 0);
            Assert.True(p.Get12(name, addrid)?.Count > 0);

            Assert.True(p.Get13(null, addrid)?.Count > 0);
            Assert.True(p.Get13("", addrid)?.Count > 0);
            Assert.True(p.Get13(name, addrid)?.Count > 0);

            DelTest();
        }

        [Fact]
        public void Test1()
        {
            InitTest();

            Assert.True(p.Get2(name, null, null)?.Count > 0);
            Assert.True(p.Get2(name, addrid, DateTime.Now)?.Count > 0);

            DelTest();
        }

        [Fact]
        public void Test2()
        {
            InitTest();

            Assert.True(p.Get31(name, null)?.Count > 0);
            Assert.True(p.Get31(name, addrid)?.Count > 0);

            Assert.True(p.Get32(name, null)?.Count > 0);
            Assert.True(p.Get32(name, addrid)?.Count > 0);

            Assert.True(p.Get33(null)?.Count > 0);
            Assert.True(p.Get33("")?.Count > 0);
            Assert.True(p.Get33(name)?.Count > 0);

            DelTest();
        }

        /// <summary>
        /// 多线程测试
        /// </summary>
        [Fact]
        public void TestThread()
        {
            InitTest(new LuaPersonParamBLL(new MSSqlDBContext()));

            List<Task> ts = new List<Task>();
            int forcount = 100;
            for (int i = 0; i < forcount; i++)
            {
                ts.Add(Task.Run(() =>
                {
                    var p = new LuaPersonParamBLL(new MSSqlDBContext());
                    Assert.True(p.Get1(name, null)?.Count > 0);
                    Assert.True(p.Get1(name, addrid)?.Count > 0);
                    Assert.True(p.Get11(name, null)?.Count > 0);
                    Assert.True(p.Get11(name, addrid)?.Count > 0);

                    Assert.True(p.Get12(null, addrid)?.Count > 0);
                    Assert.True(p.Get12("", addrid)?.Count > 0);
                    Assert.True(p.Get12(name, addrid)?.Count > 0);

                    Assert.True(p.Get13(null, addrid)?.Count > 0);
                    Assert.True(p.Get13("", addrid)?.Count > 0);
                    Assert.True(p.Get13(name, addrid)?.Count > 0);
                }));
                ts.Add(Task.Run(() =>
                {
                    var p = new LuaPersonParamBLL(new MSSqlDBContext());
                    Assert.True(p.Get2(name, null, null)?.Count > 0);
                    Assert.True(p.Get2(name, addrid, DateTime.Now)?.Count > 0);
                }));
                ts.Add(Task.Run(() =>
                {
                    var p = new LuaPersonParamBLL(new MSSqlDBContext());
                    Assert.True(p.Get31(name, null)?.Count > 0);
                    Assert.True(p.Get31(name, addrid)?.Count > 0);

                    Assert.True(p.Get32(name, null)?.Count > 0);
                    Assert.True(p.Get32(name, addrid)?.Count > 0);

                    Assert.True(p.Get33(null)?.Count > 0);
                    Assert.True(p.Get33("")?.Count > 0);
                    Assert.True(p.Get33(name)?.Count > 0);
                }));
            }
            Task.WaitAll(ts.ToArray());

            DelTest();
        }


    }
}
