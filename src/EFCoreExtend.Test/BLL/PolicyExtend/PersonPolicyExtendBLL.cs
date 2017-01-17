using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class PersonPolicyExtendBLL
    {
        DBConfigTable tinfo;
        string name = "tom_PolicyExtend";
        public PersonPolicyExtendBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }
        
        public int AddPersonPolicyEx()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new Person
            {
                addrid = 123,
                birthday = DateTime.Now,
                name = name
            }, new[] { "id" });
        }

        public int DeletePersonPolicyEx()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                name = name
            });
        }

    }
}
