using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class LuaPersonLayoutBLL : LuaBLLBase<Person>
    {
        string name = "小明";

        public LuaPersonLayoutBLL(DbContext db) : base(db)
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
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = 123,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Update()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                name = name,
                addrid = 1234,
            });
        }

        public int Update1()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                name = name,
                addrid = 1234,
            });
        }

        public int Update2()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                name = name,
                addrid = 1234,
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
