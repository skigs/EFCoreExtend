using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class PersonEachListBLL
    {
        DBConfigTable tinfo;
        string name = "小明_eachList";
        public PersonEachListBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }

        public IReadOnlyList<Person> GetListPersonEachL()
        {
            return tinfo.GetExecutor().QueryUseModel<Person>(new
            {
                idList = new[] { 123, 5456, 456 },
                nameList = new[] { name, name + "123" },
            });
        }

        public IReadOnlyList<Person> GetListPersonEachL1()
        {
            return tinfo.GetExecutor().QueryUseModel<Person>(new
            {
                idList = new[] { 123, 5456, 456 },
                nameList = new[] { name, name + "123" },
                addrid = (int?)null,
            });
        }

        public int UpdatePersonEachL()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                birthday = DateTime.Now,
                nameList = new[] { name, name + "123" },
            });
        }

        public int AddPersonEachL()
        {
            var model = new Person
            {
                name = name,
                addrid = 345345,
                birthday = DateTime.Now,
            };
            return tinfo.GetExecutor().NonQueryUseModel(model, nameof(model.id));
        }

        public int DeletePersonEachL()
        {
            return tinfo.GetExecutor().NonQueryUseDict(new Dictionary<string, object>
            {
                { nameof(name), name }
            });
        }

    }
}
