using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class LuaAddressCacheBLL : LuaBLLBase<Address>
    {
        string fullAddress = "Test Lua Address cache";
        public LuaAddressCacheBLL(DbContext db) : base(db)
        {
        }

        public IReadOnlyList<Address> Get()
        {
            return config.GetLuaExecutor().QueryUseModel<Address>(new
            {
                fullAddress = fullAddress,
            });
        }

        public IReadOnlyList<Address> Get1()
        {
            return config.GetLuaExecutor().QueryUseModel<Address>(new
            {
                fullAddress = fullAddress,
            });
        }

        public int Count()
        {
            return config.GetLuaExecutor().ScalarUseModel<int>(new
            {
                fullAddress = fullAddress,
            });
        }

        public int Add()
        {
            var model = new Address
            {
                lat = 123.123,
                lon = 345.345,
                fullAddress = fullAddress,
            };
            return config.GetLuaExecutor().NonQueryUseModel(model, nameof(model.id));
        }

        public int Update()
        {
            var model = new
            {
                lat = 56.567,
                lon = 675.345,
                fullAddress = fullAddress,
            };
            return config.GetLuaExecutor().NonQueryUseModel(model);
        }

        public int Delete()
        {
            var model = new
            {
                fullAddress = fullAddress,
            };
            return config.GetLuaExecutor().NonQueryUseModel(model);
        }

    }
}
