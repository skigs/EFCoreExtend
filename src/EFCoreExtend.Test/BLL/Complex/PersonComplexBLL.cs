using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class PersonComplexBLL
    {
        DBConfigTable tinfo;
        string name = "tom_cpx";
        public PersonComplexBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }

        static IEnumerable<string> pcpxNames = typeof(PersonCpx).GetTypeInfo().GetProperties().Select(l => l.Name).ToList();
        public IReadOnlyList<PersonCpx> GetListPersonCpx()
        {
            return tinfo.GetExecutor().QueryUseModel<PersonCpx>(new
            {
                name = name,
                cols = pcpxNames,
            });
        }

        public int AddPersonCpx()
        {
            var model = new Person
            {
                name = name,
                addrid = 234,
                birthday = DateTime.Now,
            };
            return tinfo.GetExecutor().NonQueryUseModel(model, new[] { "id" });
        }

        public int UpdatePersonCpx()
        {
            var model = new Person
            {
                name = name,
                addrid = 677,
                birthday = null,
            };
            return tinfo.GetExecutor().NonQueryUseModel(model);
        }

        public int DeletePersonCpx()
        {
            return tinfo.GetExecutor().NonQueryUseDict(new Dictionary<string, object>
            {
                {nameof(name), name }
            });
        }

    }

    public class PersonCpx
    {
        public int id { get; set; }
        public DateTime? birthday { get; set; }
        public int? addrid { get; set; }
    }

}
