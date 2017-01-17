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
    public class TestSqlConfigEachModel
    {
        //static DbContext db = new MSSqlDBContext();
        //static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        static DbContext db = new PostgreSqlDBContext();

        PersonEachModelBLL p = new PersonEachModelBLL(db);

        static TestSqlConfigEachModel()
        {
            EFHelper.Services.SqlConfigMgr.Config.LoadFile(Directory.GetCurrentDirectory() + "/Datas/Foreach/PersonEachModel.json");
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
            Assert.True(p.AddPersonEachM() > 0);
            Assert.True(p.UpdatePersonEachM() > 0);
            Assert.True(p.UpdatePersonEachM1() > 0);
            Assert.True(p.DeletePersonEachM() > 0);
        }

    }
}
