using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class PersonEachModelBLL
    {
        DBConfigTable tinfo;
        string name = "小明_eachModel";
        public PersonEachModelBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }

        public int UpdatePersonEachM()
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
                    "setdatas", new
                    {
                        addrid = model.addrid,
                        birthday = model.birthday,
                    }
                },
            });
        }

        public int AddPersonEachM()
        {
            var model = new Person
            {
                name = name,
                addrid = 345,
                birthday = null,
            };
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                datas = model
            });
        }

        public int UpdatePersonEachM1()
        {
            var model = new Person
            {
                id = 123,
                name = name,
                birthday = DateTime.Now,
            };
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                setdatas = new
                {
                    birthday = model.birthday
                },
                wheredatas = new
                {
                    name = model.name,
                    id = model.id
                },
            });
        }

        public int DeletePersonEachM()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                name = name,
            });
        }

    }
}
