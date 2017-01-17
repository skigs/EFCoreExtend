using EFCoreExtend;
using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    public class TestSqlConfigEachList
    {
        //static DbContext db = new MSSqlDBContext();
        //static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        static DbContext db = new PostgreSqlDBContext();

        PersonEachListBLL p = new PersonEachListBLL(db);

        static TestSqlConfigEachList()
        {
            EFHelper.Services.SqlConfigMgr.Config.LoadFile(Directory.GetCurrentDirectory() + "/Datas/Foreach/PersonEachList.json");
            //sql日志记录策略执行器
            EFHelper.Services.SqlConfigMgr.PolicyMgr.SetSqlConfigExecuteLogPolicyExecutor(
                (tableName, sqlName, sql, sqlparam) =>
                {
                    Assert.NotEmpty(tableName);
                    Assert.NotEmpty(sqlName);
                    Assert.NotEmpty(sql);
                    Console.WriteLine(sql + ";      params:" + (sqlparam?.Count > 0 ? sqlparam.JoinToString(",", l => l.ParameterName  + "="+ l.Value) : "") + "\r\n");
                }, true, false);
        }

        [Fact]
        public void Test()
        {
            Assert.True(p.AddPersonEachL() > 0);
            Assert.True(p.GetListPersonEachL()?.Count > 0);
            Assert.True(p.GetListPersonEachL1()?.Count > 0);
            Assert.True(p.UpdatePersonEachL() > 0);
            Assert.True(p.DeletePersonEachL() > 0);
        }

    }
}
