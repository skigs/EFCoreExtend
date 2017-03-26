using EFCoreExtend;
using EFCoreExtend.Commons;
using EFCoreExtend.Test;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    /// <summary>
    /// 配置文件加载测试
    /// </summary>
    public class TestLuaLoad
    {
        static TestLuaLoad()
        {
            EFHelper.ServiceBuilder.AddLuaSqlDefault().BuildServices(); //添加lua sql模块
        }

        [Fact]
        public void Test()
        {
            //加载指定的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadFile(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad/Person.lua");
            luasql.LoadFile(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad/Person1.txt");
            luasql.LoadFile(new FileInfo(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad/Person2.lua"));
            luasql.LoadFile(new FileInfo(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad/Person3.txt"));
            Assert.True(luasql.Config.TableSqlInfos?.Count == 4);
        }

        [Fact]
        public void Test1()
        {
            //加载指定目录的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad");    //TestLoad目录下的所有lua文件(包括子目录)
            Assert.True(luasql.Config.TableSqlInfos?.Count == 4);
        }

        [Fact]
        public void Test10()
        {
            //加载指定目录的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(new DirectoryInfo(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad"));    //TestLoad目录下的所有lua文件(包括子目录)
            Assert.True(luasql.Config.TableSqlInfos?.Count == 4);
        }

        [Fact]
        public void Test11()
        {
            //加载指定目录的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad", false);
            Assert.True(luasql.Config.TableSqlInfos?.Count == 2);
        }

        [Fact]
        public void Test12()
        {
            //加载指定目录的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad", true, LuaSqlConfigFileExtType.txt);    //TestLoad目录下的所有lua文件(包括子目录)
            Assert.True(luasql.Config.TableSqlInfos?.Count == 4);
        }

        [Fact]
        public void Test13()
        {
            //加载指定目录的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad", false, LuaSqlConfigFileExtType.txt);
            Assert.True(luasql.Config.TableSqlInfos?.Count == 2);
        }

        [Fact]
        public void Test14()
        {
            //加载指定目录的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad", true, LuaSqlConfigFileExtType.all);    //TestLoad目录下的所有lua文件(包括子目录)
            Assert.True(luasql.Config.TableSqlInfos?.Count == 8);
        }

        [Fact]
        public void Test15()
        {
            //加载指定目录的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad", false, LuaSqlConfigFileExtType.all);
            Assert.True(luasql.Config.TableSqlInfos?.Count == 4);
        }

        /// <summary>
        /// 清理配置
        /// </summary>
        [Fact]
        public void Test2()
        {
            //加载指定目录的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad", true, LuaSqlConfigFileExtType.all);    //TestLoad目录下的所有lua文件(包括子目录)
            Assert.True(luasql.Config.TableSqlInfos?.Count == 8);
            luasql.Config.Remove("Person");
            luasql.Config.Remove("Person1");
            Assert.True(luasql.Config.TableSqlInfos?.Count == 6);
        }

        /// <summary>
        /// 清理配置
        /// </summary>
        [Fact]
        public void Test21()
        {
            //加载指定目录的配置文件
            var luasql = EFHelper.Services.GetLuaSqlMgr();
            luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/TestLoad", true, LuaSqlConfigFileExtType.all);    //TestLoad目录下的所有lua文件(包括子目录)
            Assert.True(luasql.Config.TableSqlInfos?.Count == 8);
            luasql.Config.Clear();
            Assert.True(luasql.Config.TableSqlInfos?.Count == 0);
        }

    }
}
