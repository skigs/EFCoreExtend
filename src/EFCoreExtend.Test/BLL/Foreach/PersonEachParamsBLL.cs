using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class PersonEachParamsBLL
    {
        DBConfigTable tinfo;
        string name = "小明_eachparams";
        public PersonEachParamsBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Person>();
        }

        public int AddPersonEachP()
        {
            //var model = new Person()
            //{
            //    name = name,
            //    addrid = null,
            //    birthday = null,
            //};
            //return tinfo.GetExecutor().NonQueryUseModel(model, nameof(model.id));

            var model = new Person()
            {
                name = name,
                addrid = null,
                birthday = null,
            };
            return tinfo.GetExecutor().NonQueryUseModel(model, new[] { nameof(model.id) },
                new SqlForeachParamsPolicy
                {
                    IsToSqlParam = true,
                    IsKVSplit = true,
                    KSeparator = ",",
                    VSeparator = ",",
                }.ToPolicies());
        }

        public int UpdatePersonEachP()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                name = name,
                birthday = DateTime.Now,
                addrid = 123,
            });
        }

        public int DeletePersonEachP()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                name = name,
            });
        }

    }
}
