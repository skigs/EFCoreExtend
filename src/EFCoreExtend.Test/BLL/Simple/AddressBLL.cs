using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;

namespace EFCoreExtend.Test
{
    public class AddressBLL
    {
        string _fullAddress = "moon 1";
        DBConfigTable tinfo;
        public AddressBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Address>();
        }

        public IReadOnlyList<Address> GetAddrList()
        {
            return tinfo.GetExecutor().QueryUseModel<Address>(new
            {
                fullAddress = _fullAddress,
            });
        }

        public int AddAddr()
        {
            var model = new Address
            {
                fullAddress = _fullAddress,
                lat = 123.123,
                lon = 345.345,
            };
            return tinfo.GetExecutor().NonQueryUseModel(model, new[] { nameof(model.id) });
        }

        public int DelAddr()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                fullAddress = _fullAddress,
            }, null);
        }

    }
}
