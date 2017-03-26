using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class LuaPersonEachBLL : LuaBLLBase<Person>
    {
        string name = "小明each";

        public LuaPersonEachBLL(DbContext db) : base(db)
        {
        }

        public int Add1()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递List参数
                names = new[] { "name", "addrid", "birthday" },
                vals = new object[] { name, 123, null }
            });
        }

        public int Add11()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递List参数
                names = new[] { "name", "addrid", "birthday" },
                vals = new object[] { name, 123, DateTime.Now }
            });
        }

        public int Add12()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递List参数
                names = new[] { "name", "addrid" },
                vals = new object[] { name, 123, null }
            });
        }

        public int Add13()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递List参数
                names = new[] { "name", "addrid" },
                vals = new object[] { name, 123 }
            });
        }

        public int Add2()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Dictionary参数
                dparams = new Dictionary<string, object>
                {
                    { "name", name },
                    { "addrid", 123 },
                    { "birthday", null },
                },
            });
        }

        public int Add21()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Dictionary参数
                dparams = new Dictionary<string, object>
                {
                    { "name", name },
                    { "addrid", 123 },
                    { "birthday", DateTime.Now },
                },
            });
        }

        public int Add22()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Dictionary参数
                dparams = new Dictionary<string, object>
                {
                    { "name", name },
                    { "addrid", 123 },
                    { "birthday", null },
                },
            });
        }

        public int Add23()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Dictionary参数
                dparams = new Dictionary<string, object>
                {
                    { "name", name },
                    { "addrid", 123 },
                    { "birthday", null },
                },
            });
        }

        public int Add24()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Dictionary参数
                dparams = new Dictionary<string, object>
                {
                    { "id", 123 },
                    { "name", name },
                    { "addrid", 123 },
                    { "birthday", null },
                },
            });
        }

        public int Add3()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Model参数
                mparams = new 
                {
                    name = name,
                    addrid = 123,
                    birthday = (DateTime?)null,
                },
            });
        }

        public int Add31()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Model参数
                mparams = new 
                {
                    name = name,
                    addrid = 123,
                    birthday = DateTime.Now,
                },
            });
        }

        public int Add32()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Model参数
                mparams = new 
                {
                    name = name,
                    addrid = 123,
                    birthday = (DateTime?)null,
                },
            });
        }

        public int Add33()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Model参数
                mparams = new
                {
                    name = name,
                    addrid = 123,
                    birthday = (DateTime?)null,
                },
            });
        }

        public int Add34()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Model参数
                mparams = new
                {
                    id = 123,
                    name = name,
                    addrid = 123,
                    birthday = (DateTime?)null,
                },
            });
        }

        public int Update()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Dictionary参数
                dparams = new Dictionary<string, object>
                {
                    { "name", name },
                    { "addrid", 123 },
                    { "birthday", null },
                },
                name = name,
            });
        }

        public int Update1()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Dictionary参数
                dparams = new Dictionary<string, object>
                {
                    { "name", name },
                    { "addrid", 123 },
                    { "birthday", null },
                },
                name = name,
            });
        }

        public int Update2()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Model参数
                mparams = new
                {
                    name = name,
                    addrid = 123,
                    birthday = (DateTime?)null,
                },
                name = name,
            });
        }

        public int Update3()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                //传递Model参数
                mparams = new
                {
                    name = name,
                    addrid = 123,
                    birthday = (DateTime?)null,
                },
                name = name,
            });
        }

        public int Delete()
        {
            return config.GetLuaExecutor().NonQueryUseModel(new
            {
                name = name,
            });
        }

    }
}
