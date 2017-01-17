using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    public class TestExecuteSql
    {
        static DbContext db = new MSSqlDBContext();
        //static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        [Fact]
        public void TestExcSql()
        {
            var nRtn = db.NonQueryUseModel($"insert into {nameof(Person)}(name, birthday, addrid) values(@name, @birthday, @addrid)",
                new Person
                {
                    name = "tom1",
                    birthday = DateTime.Now,
                    addrid = 123,
                },
                //Model => SqlParams时需要忽略的属性
                new[] { "id" });
            Assert.True(nRtn > 0);

            var qRtn = db.QueryUseModel<Person>($"select name, birthday, addrid from {nameof(Person)} where name=@name", new
            {
                name = "tom1"
            }, null,
            //返回值类型需要忽略的属性(select中并没有获取id，如果不去掉id属性，那么Reader到Person对象的时候会抛异常)
            new[] { "id" });
            Assert.True(qRtn?.Count > 0);

            var sRtn = db.ScalarUseModel($"select count(id) from {nameof(Person)} where name=@name", new
            {
                name = "tom1"
            }, null);
            Assert.True((int)typeof(int).ChangeValueType(sRtn) > 0);

            var nRtn1 = db.NonQueryUseDict($"delete from {nameof(Person)} where name=@name",
                //可以使用SqlParameter / Dictionary作为sql的参数(使用Model对象时通过反射转换成SqlParameter的，因此性能会慢些)
                new Dictionary<string, object>
                {
                    {"name", "tom1"}
                });
            Assert.True(nRtn1 > 0);
        }

        /// <summary>
        /// 测试GUID
        /// </summary>
        [Fact]
        public void TestGUID()
        {
            var db = new MSSqlDBContext();
            //var model = new TestID { id = Guid.NewGuid().ToString(), name = "tom" };
            var model = new TestID { id = Guid.NewGuid(), name = "tom" };
            Assert.True(db.NonQueryUseModel(
                $"insert into {nameof(TestID)}(id, name) values(@id, @name)", model) > 0);

            var list = db.Query<TestID>($"select * from {nameof(TestID)}");
            Assert.NotEmpty(list.FirstOrDefault().id.ToString());

            var whereModel = new
            {
                id = model.id,
            };
            Assert.NotEmpty(db.QueryUseModel<TestID>($"select * from {nameof(TestID)} where id=@id", whereModel)
                .FirstOrDefault().id.ToString());

            Assert.True((int)typeof(int).ChangeValueType(
                db.Scalar($"select count(id) from {nameof(TestID)}")) > 0);
            Assert.True((int)typeof(int).ChangeValueType(
                db.ScalarUseModel($"select count(id) from {nameof(TestID)}", whereModel)) > 0);

            Assert.True(db.NonQueryUseModel(
                $"update {nameof(TestID)} set name=@name where id=@id", new { id = model.id, name = "tom123" }) > 0);

            Assert.True(db.NonQueryUseModel(
                $"delete from {nameof(TestID)} where id=@id", new { id = model.id }) > 0);
        }

    }
}
