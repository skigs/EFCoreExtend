using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class LuaPersonParamBLL : LuaBLLBase<Person>
    {
        public LuaPersonParamBLL(DbContext db) : base(db)
        {
        }

        public IReadOnlyList<Person> Get1(string name, int? addrid)
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
                addrid = addrid,
            });
        }

        public IReadOnlyList<Person> Get11(string name, int? addrid)
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
                addrid = addrid,
            });
        }

        public IReadOnlyList<Person> Get12(string name, int addrid)
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
                addrid = addrid,
            });
        }

        public IReadOnlyList<Person> Get13(string name, int addrid)
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
                addrid = addrid,
            });
        }

        public IReadOnlyList<Person> Get2(string name, int? addrid, DateTime? birthday)
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
                addrid = addrid,
                birthday = birthday,
            });
        }

        public IReadOnlyList<Person> Get31(string name, int? addrid)
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
                addrid = addrid,
            });
        }

        public IReadOnlyList<Person> Get32(string name, int? addrid)
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
                addrid = addrid,
            });
        }

        public IReadOnlyList<Person> Get33(string name)
        {
            return config.GetLuaExecutor().QueryUseModel<Person>(new
            {
                name = name,
            });
        }

        public int Add(string name, int? addrid)
        {
            return config.GetLuaExecutor().NonQueryUseModel(new Person
            {
                name = name,
                addrid = addrid,
                birthday = DateTime.Now
            }, "id");
        }

        public int Delete(string name)
        {
            return config.GetLuaExecutor().NonQueryUseDict(
                new Dictionary<string, object>
                {
                    { nameof(name), name },
                });
        }

        public int Delete1(int? addrid)
        {
            return config.GetLuaExecutor().NonQueryUseDict(
                new Dictionary<string, object>
                {
                    { nameof(addrid), addrid },
                });
        }


    }
}
