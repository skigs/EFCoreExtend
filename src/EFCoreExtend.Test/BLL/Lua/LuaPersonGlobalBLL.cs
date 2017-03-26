using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class LuaPersonGlobalBLL : LuaBLLBase<Person>
    {
        string name = "小明LuaPersonGlobal";
        public LuaPersonGlobalBLL(DbContext db) : base(db)
        {
        }

        public IReadOnlyList<Person> Get()
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
            });
        }

        public IReadOnlyList<Person> Get1()
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
            });
        }

        public IReadOnlyList<Person> Get2()
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
            });
        }

        public int Count()
        {
            return config.GetLuaExecutor().ScalarUseModel<int>(new
            {
                name = name,
            });
        }

        public int Add()
        {
            var model = new Person
            {
                addrid = 123,
                name = name,
                birthday = DateTime.Now,
            };
            return config.GetLuaExecutor().NonQueryUseModel(model, nameof(model.id));
        }

        public int Update()
        {
            var model = new
            {
                addrid = 123,
                name = name,
            };
            return config.GetLuaExecutor().NonQueryUseModel(model);
        }

        public int Delete()
        {
            var model = new
            {
                name = name,
            };
            return config.GetLuaExecutor().NonQueryUseModel(model);
        }

    }
}
