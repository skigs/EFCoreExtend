using EFCoreExtend;
using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    public class TestSqlConfigCache
    {
        static DbContext db = new MSSqlDBContext();
        //static DbContext db = new SqlieteDBContext();
        //static DbContext db = new MysqlDBContext();
        //static DbContext db = new PostgreSqlDBContext();

        PersonL1CacheBLL p1 = new PersonL1CacheBLL(db);
        PersonL2CacheBLL p2 = new PersonL2CacheBLL(db);
        AddressCacheBLL a1 = new AddressCacheBLL(db);

        static TestSqlConfigCache()
        {
            ////使用Redis进行缓存
            //EFHelper.ServiceBuilder.AddQueryCacheCreator(sp =>
            //    new Redis.EFCache.RedisQueryCacheCreator("127.0.0.1:6379,allowAdmin=true"))
            //    .BuildServices();    //重新编译服务;
            ////移除缓存，以免影响测试
            //EFHelper.Services.Cache.Remove<Person>();
            //EFHelper.Services.Cache.Remove<Address>();


            EFHelper.Services.SqlConfigMgr.Config.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Cache");
            //sql日志记录策略执行器
            EFHelper.Services.SqlConfigMgr.PolicyMgr.SetSqlConfigExecuteLogPolicyExecutor(
                (tableName, sqlName, sql, sqlparam) =>
                {
                    Assert.NotEmpty(tableName);
                    Assert.NotEmpty(sqlName);
                    Assert.NotEmpty(sql);
                    Console.WriteLine(sql + ";      params:" + (sqlparam?.Count > 0 ? sqlparam.JoinToString(",", l => l.ParameterName  + "="+ l.Value) : "") + "\r\n");
                }, true, false);
        }

        /// <summary>
        /// 一级查询缓存
        /// </summary>
        [Fact]
        public void L1Cache()
        {
            p1.GetListL1Cache();
            p1.CountL1Cache();
        }

        /// <summary>
        /// 二级查询缓存，不过期与清理(清理对应的表下的所有缓存)
        /// </summary>
        [Fact]
        public void L2Cache()
        {
            Assert.True(p2.AddPersonL2Cache() > 0);

            var list = p2.GetListL2Cache();  //二级查询缓存，不设置过期时间的
            Assert.True(p2.AddPersonL2NotClearCache() > 0);
            var list1 = p2.GetListL2Cache();
            Assert.True(list?.Count > 0);
            Assert.True(list?.Count == list1?.Count);

            p2.DeletePersonL2Cache();   //清理缓存
            var list2 = p2.GetListL2Cache();
            Assert.True(list?.Count != list2?.Count);
        }

        /// <summary>
        /// 二级查询缓存，不过期与清理(清理对应的表下的所有缓存)
        /// </summary>
        [Fact]
        public void L2Cache0()
        {
            var db = new MSSqlDBContext();
            var p2 = new PersonL2CacheBLL(db);
            //使用了 output inserted.id 的，因此测试时选择支持的数据库(MSSqlServer)
            Assert.True(p2.AddPersonL2Cache1() > 0);    

            var list = p2.GetListL2Cache();  //二级查询缓存，不设置过期时间的
            Assert.True(p2.AddPersonL2NotClearCache() > 0);
            var list1 = p2.GetListL2Cache();
            Assert.True(list?.Count > 0);
            Assert.True(list?.Count == list1?.Count);

            Assert.True(p2.AddPersonL2Cache1() > 0);   //清理缓存
            var list2 = p2.GetListL2Cache();
            Assert.True(list?.Count != list2?.Count);

            p2.DeletePersonL2Cache();
        }

        /// <summary>
        /// 二级查询缓存，不过期与清理(清理对应的表下指定CacheType(类型)的缓存)
        /// </summary>
        [Fact]
        public void L2Cache1()
        {
            Assert.True(p2.AddPersonL2Cache() > 0);

            var list = p2.GetListL2Cache1();    //指定了CacheType: query1，默认为query
            Assert.True(p2.AddPersonL2NotClearCache() > 0);
            var list1 = p2.GetListL2Cache1();
            Assert.True(list?.Count > 0);
            Assert.True(list?.Count == list1?.Count);

            var list2 = p2.GetListL2Cache();   //CacheType: query
            Assert.True(p2.AddPersonL2NotClearCache() > 0);
            var list3 = p2.GetListL2Cache();
            Assert.True(list2?.Count > 0);
            Assert.True(list2?.Count == list3?.Count);

            Assert.True(list?.Count != list2?.Count);

            Assert.True(p2.UpdatePersonL2Cache() > 0);  //清理CacheType: query
            var list4 = p2.GetListL2Cache();
            Assert.True(list4?.Count > 0);
            Assert.True(list2?.Count != list4?.Count);

            var list5 = p2.GetListL2Cache1();   //CacheType: query1
            Assert.True(list4?.Count != list5?.Count);
            Assert.True(list?.Count == list5?.Count);

            Assert.True(p2.DeletePersonL2Cache() > 0);  //清理同表下的所有缓存
            var list6 = p2.GetListL2Cache1();
            Assert.True(list?.Count != list6?.Count);
        }

        /// <summary>
        /// 二级查询缓存，不过期与清理
        /// </summary>
        [Fact]
        public void L2Cache11()
        {
            Assert.True(p2.AddPersonL2Cache() > 0);

            var list = p2.GetListL2Cache1();    //指定了CacheType: query1，默认为query
            Assert.True(p2.AddPersonL2NotClearCache() > 0);
            var list1 = p2.GetListL2Cache1();
            Assert.True(list?.Count > 0);
            Assert.True(list?.Count == list1?.Count);

            Assert.True(p2.UpdatePersonL2Cache0() > 0);  //清理CacheType: query1
            var list2 = p2.GetListL2Cache1();
            Assert.True(list?.Count != list2?.Count);

            Assert.True(p2.DeletePersonL2Cache() > 0);
        }

        /// <summary>
        /// 缓存过期
        /// </summary>
        [Fact]
        public void L2Cache2()
        {
            Assert.True(p2.AddPersonL2Cache() > 0);

            var list = p2.GetListL2Cache2();    //"date": "2018-01-01" //指定缓存的过期日期
            Assert.True(p2.AddPersonL2NotClearCache() > 0);
            var list1 = p2.GetListL2Cache2();
            Assert.True(list?.Count > 0);
            Assert.True(list?.Count == list1?.Count);

            Assert.True(p2.DeletePersonL2Cache() > 0);  //清理同表下的所有缓存

            var list2 = p2.GetListL2Cache2();
            Assert.True(list?.Count != list2?.Count);
        }

        /// <summary>
        /// 缓存过期
        /// </summary>
        [Fact]
        public void L2Cache3()
        {
            Assert.True(p2.AddPersonL2Cache() > 0);

            var list = p2.GetListL2Cache3(); //"span": "0:0:3" //指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔），这里设置为3s（方便测试）
            Assert.True(p2.AddPersonL2NotClearCache() > 0);
            var list1 = p2.GetListL2Cache3();
            Assert.True(list?.Count > 0);
            Assert.True(list?.Count == list1?.Count);

            Thread.Sleep(3500); //3秒后过期
            var list2 = p2.GetListL2Cache3();
            Assert.True(list?.Count != list2?.Count);

            Assert.True(p2.DeletePersonL2Cache() > 0);  //清理同表下的所有缓存
        }

        /// <summary>
        /// 缓存过期与更新缓存时间 与 更改缓存策略
        /// </summary>
        [Fact]
        public void L2Cache4()
        {
            Assert.True(p2.AddPersonL2Cache() > 0);

            var c1 = p2.CountL2Cache(); //"span": "0:0:3"，并且"isUpdateEach": true
            Assert.True(p2.AddPersonL2NotClearCache() > 0);
            var c2 = p2.CountL2Cache();
            Assert.True(c1 > 0);
            Assert.True(c1 == c2);

            Thread.Sleep(2000); 
            var c3 = p2.CountL2Cache(); //会自动更新缓存时间
            Assert.True(c1 == c3);

            Thread.Sleep(2000); 
            var c4 = p2.CountL2Cache(); //会自动更新缓存时间
            Assert.True(c1 == c4);

            Thread.Sleep(2000);
            var c6 = p2.CountL2Cache(TimeSpan.FromSeconds(3)); //更改为不自动更新时间的
            Assert.True(c1 == c6);
            Thread.Sleep(2000);
            var c61 = p2.CountL2Cache(TimeSpan.FromSeconds(3)); //更改为不自动更新时间的
            Assert.True(c1 != c61);

            Assert.True(p2.DeletePersonL2Cache() > 0);  //清理同表下的所有缓存

            var c7 = p2.CountL2Cache();
            Assert.True(c1 != c7);
        }

        /// <summary>
        /// 清理其他表的缓存
        /// </summary>
        [Fact]
        public void L2Cache5()
        {
            Assert.True(p2.AddPersonL2Cache() > 0);
            Assert.True(a1.AddAddrCache() > 0);

            var list1 = a1.GetAddrListCache();  //CacheType: query
            Assert.True(a1.AddAddrCacheNotClearCache() > 0);
            var list4 = a1.GetAddrListCache();
            Assert.True(list1?.Count == list4?.Count);

            var list2 = a1.GetAddrListCache1(); //CacheType: query1
            var list3 = a1.GetAddrListCache1();
            Assert.True(a1.AddAddrCacheNotClearCache() > 0);
            Assert.True(list2?.Count == list3?.Count);

            Assert.True(list1?.Count != list3?.Count);

            //"tables": [ "Address" ] //需要进行缓存清理的表的名称（一般用于清理 其他表下 的所有查询缓存）
            var c1 = p2.UpdatePersonL2Cache1(); //要有处理的数据才会触发清理缓存策略，因此上面要调用p2.AddPersonL2Cache()
            var list5 = a1.GetAddrListCache();
            Assert.True(list1?.Count != list5?.Count);

            var list6 = a1.GetAddrListCache1();
            Assert.True(list3?.Count != list6?.Count);

            Assert.True(p2.DeletePersonL2Cache() > 0);
            Assert.True(a1.DelAddrCache() > 0);
        }

        /// <summary>
        /// 清理其他表指定的CacheType的缓存
        /// </summary>
        [Fact]
        public void L2Cache6()
        {
            Assert.True(p2.AddPersonL2Cache() > 0);
            Assert.True(a1.AddAddrCache() > 0);

            var list1 = a1.GetAddrListCache();  //CacheType: query
            var list4 = a1.GetAddrListCache();
            Assert.True(a1.AddAddrCacheNotClearCache() > 0);
            Assert.True(list1?.Count == list4?.Count);

            var list2 = a1.GetAddrListCache1(); //CacheType: query1
            Assert.True(a1.AddAddrCacheNotClearCache() > 0);
            var list3 = a1.GetAddrListCache1();
            Assert.True(list2?.Count == list3?.Count);

            Assert.True(list1?.Count != list3?.Count);

            // "tableCacheTypes": { //需要进行缓存清理的类型(key为TableName，value为CacheType，一般用于清理 其他表下 的CacheType)
            //     "Address": [ "query" ]}
            var c1 = p2.UpdatePersonL2Cache2(); //要有处理的数据才会触发清理缓存策略，因此上面要调用AddPersonL2Cache
            var list5 = a1.GetAddrListCache();
            Assert.True(list1?.Count != list5?.Count);

            var list6 = a1.GetAddrListCache1();
            Assert.True(list3?.Count == list6?.Count);

            Assert.True(p2.DeletePersonL2Cache() > 0);
            Assert.True(a1.DelAddrCache() > 0);
        }


    }
}
