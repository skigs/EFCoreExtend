using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    public class TestSqlConfigComplex
    {
        static DbContext db = new MSSqlDBContext();
        //static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        PersonComplexBLL p = new PersonComplexBLL(db);

        static TestSqlConfigComplex()
        {
            EFHelper.Services.SqlConfigMgr.Config.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Complex");
            //sql日志记录策略执行器
            EFHelper.Services.SqlConfigMgr.PolicyMgr.SetSqlConfigExecuteLogPolicyExecutor(
                (tableName, sqlName, sql, sqlparam) =>
                {
                    Assert.NotEmpty(tableName);
                    Assert.NotEmpty(sqlName);
                    Assert.NotEmpty(sql);
                    Console.WriteLine(sql + ";      params:" + (sqlparam?.Count > 0 ? sqlparam.JoinToString(",", l => l.ParameterName + "=" + l.Value) : "") + "\r\n");
                }, true, false);
        }

        [Fact]
        public void Test()
        {
            Assert.True(p.AddPersonCpx() > 0);
            var list = p.GetListPersonCpx();
            Assert.True(list?.Count > 0);
            Assert.True(p.UpdatePersonCpx() > 0);
            Assert.True(p.DeletePersonCpx() > 0);
        }

        [Fact]
        public void Test1()
        {
            Assert.True(p.AddPersonCpx() > 0);

            var list = p.GetListPersonCpx();
            Assert.True(list?.Count > 0);
            var list1 = p.GetListPersonCpx();
            Assert.True(list == list1);

            Assert.True(p.UpdatePersonCpx() > 0);
            var list2 = p.GetListPersonCpx();
            Assert.True(list != list2);

            Assert.True(p.DeletePersonCpx() > 0);
        }

        [Fact]
        public void Test2()
        {
            Assert.True(p.AddPersonCpx() > 0);

            var list = p.GetListPersonCpx();
            Assert.True(list?.Count > 0);
            var list1 = p.GetListPersonCpx();
            Assert.True(list == list1);

            Thread.Sleep(3000);
            var list2 = p.GetListPersonCpx();
            Assert.True(list != list2);

            Assert.True(p.DeletePersonCpx() > 0);
        }

    }
}
