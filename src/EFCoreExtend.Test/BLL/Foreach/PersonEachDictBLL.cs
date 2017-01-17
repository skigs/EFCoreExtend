using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class PersonEachDictBLL
    {
        DBConfigTable tinfo;
        string name = "小明_eachDict";
        public PersonEachDictBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }

        public int UpdatePersonEachD()
        {
            var model = new Person
            {
                name = name,
                addrid = null,
                birthday = DateTime.Now,
            };
            return tinfo.GetExecutor().NonQueryUseDict(new Dictionary<string, object>
            {
                { nameof(model.name), model.name },
                {
                    "setdatas", new Dictionary<string, object>
                    {
                        { nameof(model.addrid), model.addrid },
                        { nameof(model.birthday), model.birthday },
                    }
                },
            });
        }

        public int AddPersonEachD()
        {
            var model = new Person
            {
                name = name,
                addrid = 345,
                birthday = null,
            };
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                datas = new Dictionary<string, object>
                {
                    { nameof(model.addrid), model.addrid },
                    { nameof(model.birthday), model.birthday },
                    { nameof(model.name), model.name },
                    { nameof(model.id), model.id }, //配置中设置了忽略
                }
            });
        }

        public int UpdatePersonEachD1()
        {
            var model = new Person
            {
                id = 123,
                name = name,
                birthday = DateTime.Now,
            };
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                setdatas = new Dictionary<string, object>
                    {
                        { nameof(model.birthday), model.birthday },
                    },
                wheredatas = new Dictionary<string, object>
                    {
                        { nameof(model.name), model.name },
                        { nameof(model.id), model.id },
                    }
            });
        }

        public int DeletePersonEachD()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                name = name,
            });
        }

    }
}
