using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class AddressCacheBLL
    {
        DBConfigTable tinfo;
        public AddressCacheBLL(DbContext db)
        {
            tinfo = db.GetConfigTable<Address>();
        }

        public IReadOnlyList<Address> GetAddrListCache()
        {
            return tinfo.GetExecutor().Query<Address>();
        }

        public IReadOnlyList<Address> GetAddrListCache1()
        {
            return tinfo.GetExecutor(nameof(GetAddrListCache))
                .Query<Address>(null, null, new SqlL2QueryCachePolicy
                {
                    Type = "query1",
                }.ToPolicies());
        }

        public int AddAddrCache()
        {
            return tinfo.GetExecutor().NonQuery();
        }

        public int AddAddrCacheNotClearCache()
        {
            return tinfo.GetExecutor().NonQuery();
        }

        public int DelAddrCache()
        {
            return tinfo.GetExecutor().NonQuery();
        }

    }
}
