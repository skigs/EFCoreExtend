using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Data.SqlClient;
using EFCoreExtend.Sql.SqlConfig.Default;
using System.Threading;
using EFCoreExtend.EFCache.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies;
using Xunit;
using Microsoft.Data.Sqlite;
using EFCoreExtend.Commons;
using MySql.Data.MySqlClient;
using Npgsql;

namespace EFCoreExtend.Test
{
    public class TestSqlConfig
    {
        static DbContext db = new MSSqlDBContext();
        //static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();


        PersonBLL person = new PersonBLL(db);
        AddressBLL address = new AddressBLL(db);

        static TestSqlConfig()
        {
            EFHelper.Services.SqlConfigMgr.Config.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Simple");
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
        public void Simple()
        {
            Assert.True(address.AddAddr() > 0);
            var list = address.GetAddrList();
            Assert.True(list?.Count > 0);
            Assert.True(list.Where(l => l.fullAddress == "moon 1").Count() > 0);
            Assert.True(address.DelAddr() > 0);
        }

        [Fact]
        public void Simple1()
        {
            //增加
            Assert.True(person.AddPerson() > 0);
            //修改
            Assert.True(person.UpdatePerson() > 0);
            //查询Scalar
            Assert.True(person.Count() > 0);
            //删除
            Assert.True(person.DeletePerson() > 0);
        }

        [Fact]
        public void Simple11()
        {
            //增加
            Assert.True(person.AddPerson() > 0);
            //修改
            Assert.True(person.UpdatePerson() > 0);
            //查询Query
            //方式一
            var list = person.GetList();
            Assert.True(list?.Count > 0);
            Assert.True(list.Where(l => l.name != null).Count() <= 0);  //因为忽略获取name，因此那么应该为null
            //删除
            Assert.True(person.DeletePerson() > 0);
        }

        [Fact]
        public void Simple112()
        {
            //增加
            Assert.True(person.AddPerson() > 0);
            //修改
            Assert.True(person.UpdatePerson() > 0);
            //查询Query
            //方式二
            var list1 = person.GetList1();
            Assert.True(list1?.Count > 0);
            Assert.True(list1.Where(l => l.name != null).Count() <= 0);  //因为忽略获取name，因此那么应该为null
            //删除
            Assert.True(person.DeletePerson() > 0);
        }

        [Fact]
        public void Simple13()
        {
            //增加
            Assert.True(person.AddPerson() > 0);
            //修改
            Assert.True(person.UpdatePerson() > 0);

            //查询Query
            //方式三
            var list2 = person.GetList2<SqlParameter>();
            //var list2 = person.GetList2<SqliteParameter>();
            //var list2 = person.GetList2<MySqlParameter>();
            //var list2 = person.GetList2<NpgsqlParameter>();
            Assert.True(list2?.Count > 0);
            Assert.True(list2.Where(l => l.name != null).Count() <= 0);  //因为忽略获取name，因此那么应该为null

            Assert.True(person.GetPerson() != null);
            //删除
            Assert.True(person.DeletePerson() > 0);
        }

        [Fact]
        public void Trans1()
        {
            int oldAddrid = 123, newAddrid = 345;
            person.DeletePerson();
            Assert.True(person.AddPerson() > 0);
            Assert.True(person.UpdatePerson(oldAddrid) > 0);

            bool bRtn = true;   //模拟事物提交成功
            db.Database.BeginTransaction();
            Assert.True(person.GetPerson().addrid == oldAddrid);
            Assert.True(person.UpdatePerson(newAddrid) > 0);
            if (bRtn)
                db.Database.CommitTransaction();
            else
                db.Database.RollbackTransaction();

            Assert.True(person.GetPerson().addrid == newAddrid);
        }

        [Fact]
        public void Trans2()
        {
            int oldAddrid = 123, newAddrid = 345;
            person.DeletePerson();
            Assert.True(person.AddPerson() > 0);
            Assert.True(person.UpdatePerson(oldAddrid) > 0);

            bool bRtn = false;   //模拟事物提交失败
            db.Database.BeginTransaction();
            Assert.True(person.GetPerson().addrid == oldAddrid);
            Assert.True(person.UpdatePerson(newAddrid) > 0);
            if (bRtn)
                db.Database.CommitTransaction();
            else
                db.Database.RollbackTransaction();

            Assert.True(person.GetPerson().addrid == oldAddrid);
        }

        [Fact]
        public void Proc()
        {
            //存储过程
            person.ProcQuery();
            person.ProcUpdate();
        }

    }
}
