using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class LuaPersonBLL : LuaBLLBase<Person>
    {
        string name = "小明";

        public LuaPersonBLL(DbContext db) : base(db)
        {
        }

        public IReadOnlyList<Person> Get()
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
            });
        }

        public int GetException()
        {
            //query类型的sql被执行了scalar，那么抛异常
            return config.GetLuaExecutor("Get").ScalarUseModel<int>(new
            {
                name = name,
            });
        }

        public IReadOnlyList<Person> Get1<T>()
            where T : IDataParameter, new()
        {
            var param = (IDataParameter)new T();
            param.ParameterName = nameof(name);
            param.Value = name;

            //和上面的Get一样的，只是传递参数的形式不一样
            return config.GetLuaExecutor("Get").Query<Person>(new[] { param });
        }

        public IReadOnlyList<Person> Get2()
        {
            //和上面的Get一样的，只是传递参数的形式不一样
            return config.GetLuaExecutor("Get").QueryUseDict<Person>(new Dictionary<string, object>
            {
                { nameof(name), name },
            });
        }

        public int Count()
        {
            return config.GetLuaExecutor().ScalarUseModel<int>(new
            {
                name = name,
            });
        }

        public int CountException()
        {
            //scalar类型的sql被执行了nonquery，那么抛异常
            return config.GetLuaExecutor("Count").NonQueryUseModel(new
            {
                name = name,
            });
        }

        public int Count1<T>()
            where T : IDataParameter, new()
        {
            var param = (IDataParameter)new T();
            param.ParameterName = nameof(name);
            param.Value = name;
            return config.GetLuaExecutor("Count").Scalar<int>(param);
        }

        public int Count2()
        {
            return config.GetLuaExecutor("Count").ScalarUseDict<int>(new Dictionary<string, object>
            {
                { nameof(name), name },
            });
        }

        public int Add()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = 123,
                birthday = DateTime.Now,
            }, "id");
        }

        public int AddException()
        {
            return config.GetLuaExecutor("Add").ScalarUseModel<int>(new Person
            {
                name = name,
                addrid = 123,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Update(int? addrid = 345)
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                name = name,
                addrid = addrid,
            });
        }

        public int Update1<T>()
            where T : IDataParameter, new()
        {
            var pname = (IDataParameter)new T();
            pname.ParameterName = nameof(name);
            pname.Value = name;

            var paddrid = (IDataParameter)new T();
            paddrid.ParameterName = "addrid";
            paddrid.Value = 345;

            return config.GetLuaExecutor("Update").NonQuery(pname, paddrid);
        }

        public int Update2()
        {
            return config.GetLuaExecutor("Update").NonQueryUseDict(new Dictionary<string, object>
            {
                { "name", name },
                { "addrid", 345 },
            });
        }

        public int Delete()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                name = name
            });
        }

    }
}
