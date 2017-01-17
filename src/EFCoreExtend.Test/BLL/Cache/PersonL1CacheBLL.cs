using EFCoreExtend.Sql.SqlConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    public class PersonL1CacheBLL
    {
        DBConfigTable tinfo;
        string _name = "小明_l1cache";
        public PersonL1CacheBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }

        public void GetListL1Cache()
        {
            var exc = tinfo.GetExecutor();
            Assert.True(AddPersonL1Cache() > 0);
            var q1 = exc.QueryUseModel<Person>(new { name = _name });
            Assert.True(AddPersonL1Cache() > 0);
            var q2 = exc.QueryUseModel<Person>(new { name = _name });
            var q3 = exc.QueryUseModel<Person>(new { name = _name + "1" });
            //一级缓存作用于SqlConfigExecutor，因此设置了一级缓存策略，那么同一个SqlConfigExecutor对象
            //相同sql和SqlParameter下获取的数据是一样的
            Assert.True(q1 == q2);
            Assert.True(q1 != q3);
            Assert.True(DelPersonL1Cache() > 0);
        }

        public void CountL1Cache()
        {
            var exc = tinfo.GetExecutor();
            Assert.True(AddPersonL1Cache() > 0);
            var q1 = exc.ScalarUseModel(new { name = _name });
            Assert.True(AddPersonL1Cache() > 0);
            var q2 = exc.ScalarUseModel(new { name = _name });
            var q3 = exc.ScalarUseModel(new { name = _name + "1" });
            Assert.True(q1 == q2);
            Assert.True(q1 != q3);
            Assert.True(DelPersonL1Cache() > 0);
        }

        public int AddPersonL1Cache()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new { name = _name });
        }

        public int DelPersonL1Cache()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new { name = _name });
        }

    }
}
