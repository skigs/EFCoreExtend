using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend.Commons;

namespace EFCoreExtend.Test
{
    public class PersonBLL
    {
        string _name = "tom";
        DBConfigTable tinfo;
        public PersonBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }

        public int AddPerson()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new Person
            {
                addrid = 1,
                birthday = DateTime.Now,
                name = _name,
            }, null);
        }

        public int UpdatePerson(int? addrid = 123)
        {
            var exc = tinfo.GetExecutor();
            return exc.NonQueryUseModel(new { name = _name, birthday = DateTime.Now, addrid = addrid }, null);
        }

        public int DeletePerson()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                name = _name
            }, null);
        }

        public int Count()
        {
            var exc = tinfo.GetExecutor();
            var rtn = exc.ScalarUseModel(new { name = _name }, null);
            //MSSqlServer返回值会为int，而Sqlite会为long，转换就会出错，因此需要ChangeValueType
            return (int)typeof(int).ChangeValueType(rtn);
        }

        public IReadOnlyList<Person> GetList()
        {
            //方式一
            return tinfo.GetExecutor().QueryUseModel<Person>(
                //Model => SqlParams
                new { name = _name, id = 123 },
                //不需要的SqlParams
                new[] { "id" },
                //返回值类型需要忽略的属性
                new[] { "name" });

        }

        public IReadOnlyList<Person> GetList1()
        {
            //方式二
            return tinfo.GetExecutor(nameof(GetList))
                .QueryUseDict<Person>(
                //Dictionary => SqlParams
                new Dictionary<string, object>
                {
                    {"name", _name }
                },
                //返回值类型需要忽略的属性
                new[] { "name" });
        }

        public IReadOnlyList<Person> GetList2<T>()
            where T : IDataParameter, new()
        {
            var p1 = new T();
            p1.ParameterName = "name";
            p1.Value = _name;

            var p2 = new T();
            p2.ParameterName = "id";
            p2.Value = 123;

            //方式三
            return tinfo.GetExecutor(nameof(GetList))
                .Query<Person>(
                //SqlParams
                new IDataParameter[] { p1, p2 },
                //返回值类型需要忽略的属性
                new[] { "name" });
        }

        public Person GetPerson()
        {
            return tinfo.GetExecutor().QueryUseModel<Person>(new
            {
                name = _name
            }, null)?.FirstOrDefault();
        }

        public IReadOnlyList<Person> ProcQuery()
        {
            ////Stored procedure sql：
            //create proc TestQuery
            //@name varchar(256) = null
            //as
            //begin
            //    select * from person where [name] = @name
            //end

            return tinfo.GetExecutor().QueryUseModel<Person>(new { name = "tom" }, null);
        }

        public int ProcUpdate()
        {
            ////Stored procedure sql：
            //create proc TestUpdate
            //@addrid int = 0,
            //@name varchar(256)
            //as
            //begin

            //    update person set addrid = @addrid where[name] = @name
            //end

            return tinfo.GetExecutor().NonQueryUseModel(new { addrid = 3, name = "tom" }, null);
        }

    }
}
