using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class PersonSectionBLL
    {
        DBConfigTable tinfo;
        string name = "tom_section";
        public PersonSectionBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }

        public IReadOnlyList<Person> GetListSection()
        {
            return tinfo.GetExecutor().QueryUseModel<Person>(new
            {
                name = name,
                addrid = 123,
            });
        }

        public int UpdatePersonSection()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                name = name,
                birthday = (DateTime?)null,
            });
        }

        public int AddPersonSection()
        {
            var model = new Person
            {
                addrid = 123,
                birthday = null,
                name = name,
            };
            return tinfo.GetExecutor().NonQueryUseModel(model, new[] { nameof(model.id) });
        }

        public int DeletePersonSection()
        {
            return tinfo.GetExecutor().NonQueryUseDict(
                new Dictionary<string, object>()
                {
                    { nameof(name), name},
                });
        }

    }
}
