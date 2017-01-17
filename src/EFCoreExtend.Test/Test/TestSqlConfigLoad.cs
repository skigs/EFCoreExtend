using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Default;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtend.Test
{
    public class TestSqlConfigLoad
    {

        [Fact]
        public void LoadDir()
        {
            EFHelper.Services.SqlConfigMgr.Config.LoadDirectory(
                Directory.GetCurrentDirectory() + "/Datas/LoadTest");
            Assert.True(EFHelper.Services.SqlConfigMgr.Config.TableSqlInfos["Person"]
                .Sqls.Where(l => l.Key == "LoadTest" || l.Key == "LoadTest1").Count() == 2);
            Assert.True(EFHelper.Services.SqlConfigMgr.Config.TableSqlInfos["Address"]
                .Sqls.Where(l => l.Key == "LoadTest" || l.Key == "LoadTest1").Count() == 2);
        }

        [Fact]
        public void LoadDir1()
        {
            EFHelper.Services.SqlConfigMgr.Config.LoadDirectory(
                new DirectoryInfo(Directory.GetCurrentDirectory() + "/Datas/LoadTest1"), false, 
                SqlConfigFileExtType.all);
            Assert.True(EFHelper.Services.SqlConfigMgr.Config.TableSqlInfos["Person"]
                .Sqls.Where(l => l.Key == "LoadTest11" || l.Key == "LoadTest111").Count() == 1);
            Assert.True(EFHelper.Services.SqlConfigMgr.Config.TableSqlInfos["Address"]
                .Sqls.Where(l => l.Key == "LoadTest11" || l.Key == "LoadTest111").Count() == 1);
        }

        [Fact]
        public void LoadFile()
        {
            EFHelper.Services.SqlConfigMgr.Config.LoadFile(
                Directory.GetCurrentDirectory() + "/Datas/LoadTest2/Person.json");
            Assert.True(EFHelper.Services.SqlConfigMgr.Config.TableSqlInfos["Person"]
                .Sqls.Where(l => l.Key == "LoadTest22").Count() == 1);
        }

        [Fact]
        public void LoadFile1()
        {
            EFHelper.Services.SqlConfigMgr.Config.LoadFile(
                new FileInfo(Directory.GetCurrentDirectory() + "/Datas/LoadTest2/Address.json"));
            Assert.True(EFHelper.Services.SqlConfigMgr.Config.TableSqlInfos["Address"]
                .Sqls.Where(l => l.Key == "LoadTest22").Count() == 1);
        }

        [Fact]
        public void AddSqls()
        {
            EFHelper.Services.SqlConfigMgr.Config.AddSqls<Person>(new Dictionary<string, IConfigSqlInfo>
            {
                {
                    "AddSqlsTest1",
                    new ConfigSqlInfo
                    {
                        Sql = $"update {nameof(Person)} set name=@name where id=@id",
                        Type = ConfigSqlExecuteType.nonquery,
                    }
                },
                {
                    "AddSqlsTest2",
                    new ConfigSqlInfo
                    {
                        Sql = $"select * from {nameof(Person)} id=@id",
                        Type = ConfigSqlExecuteType.query,
                    }
                }
            });
            Assert.True(EFHelper.Services.SqlConfigMgr.Config.TableSqlInfos["Person"]
                .Sqls.Where(l => l.Key == "AddSqlsTest1" || l.Key == "AddSqlsTest2").Count() == 2);
        }

        [Fact]
        public void AddTables()
        {
            EFHelper.Services.SqlConfigMgr.Config.AddOrCombine(new[] 
            {
                new ConfigTableInfo
                {
                    Name = nameof(Person),
                    Sqls = new Dictionary<string, IConfigSqlInfo>
                    {
                        {
                            "AddTablesTest1",
                            new ConfigSqlInfo
                            {
                                Sql = $"update {nameof(Person)} set name=@name where id=@id",
                                Type = ConfigSqlExecuteType.nonquery,
                            }
                        },
                        {
                            "AddTablesTest2",
                            new ConfigSqlInfo
                            {
                                Sql = $"select * from {nameof(Person)} id=@id",
                                Type = ConfigSqlExecuteType.query,
                            }
                        }
                    }
                },
                new ConfigTableInfo
                {
                    Name = nameof(Address),
                    Sqls = new Dictionary<string, IConfigSqlInfo>
                    {
                        {
                            "AddTablesTest1",
                            new ConfigSqlInfo
                            {
                                Sql = $"update {nameof(Address)} set fullAddress=@fullAddress where id=@id",
                                Type = ConfigSqlExecuteType.nonquery,
                            }
                        },
                        {
                            "AddTablesTest2",
                            new ConfigSqlInfo
                            {
                                Sql = $"select * from {nameof(Address)} id=@id",
                                Type = ConfigSqlExecuteType.query,
                            }
                        }
                    }
                },
            });
            Assert.True(EFHelper.Services.SqlConfigMgr.Config.TableSqlInfos["Person"]
                .Sqls.Where(l => l.Key == "AddTablesTest1" || l.Key == "AddTablesTest2").Count() == 2);
            Assert.True(EFHelper.Services.SqlConfigMgr.Config.TableSqlInfos["Address"]
                .Sqls.Where(l => l.Key == "AddTablesTest1" || l.Key == "AddTablesTest2").Count() == 2);
        }

    }
}
