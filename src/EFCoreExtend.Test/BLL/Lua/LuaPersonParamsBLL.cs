using EFCoreExtend;
using EFCoreExtend.Test;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class LuaPersonParamsBLL : LuaBLLBase<Person>
    {
        string name = "小明Params";

        public LuaPersonParamsBLL(DbContext db) : base(db)
        {
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

        public int Add1()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = null,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Add11()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = null,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Add12()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = null,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Add13()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = null,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Add2()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = 123,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Add21()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = null,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Add3()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                name = name,
                addrid = 123,
            }, "id", "birthday");
        }

        public int Update(int? addrid)
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = addrid,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Update1(int? addrid)
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = addrid,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Update2()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = 3453,
                birthday = DateTime.Now,
            }, "id");
        }

        public int Update21()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = 123,
                birthday = null,
            }, "id");
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
