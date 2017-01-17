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
    /// 策略扩展测试
    /// </summary>
    public class TestPolicyExtend
    {
        //static DbContext db = new MSSqlDBContext();
        static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        PersonPolicyExtendBLL p = new PersonPolicyExtendBLL(db);

        static TestPolicyExtend()
        {
            //添加扩展的策略执行器
            var exc = new InsertIntoPolicyExecutor();
            EFHelper.Services.SqlConfigMgr.PolicyMgr.SetInitPolicyExecutor(() => exc);
            //添加扩展的策略类型
            EFHelper.Services.SqlConfigMgr.PolicyMgr.SetPolicyType<InsertIntoPolicy>();
            EFHelper.Services.SqlConfigMgr.Config.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/PolicyExtend");
        }

        [Fact]
        public void Test()
        {
            Assert.True(p.AddPersonPolicyEx() > 0);
            Assert.True(p.DeletePersonPolicyEx() > 0);
        }

    }
}
