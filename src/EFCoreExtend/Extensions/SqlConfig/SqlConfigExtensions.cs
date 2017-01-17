using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Executors;
using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EFCoreExtend.Sql.SqlConfig.Default.Json;

namespace EFCoreExtend
{
    public static class SqlConfigExtensions
    {
        /// <summary>
        /// 添加或者合并Tables
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="sqlTables"></param>
        public static void AddOrCombine(this ISqlConfig sqlConfig, IEnumerable<IConfigTableInfo> sqlTables)
        {
            foreach (var l in sqlTables)
            {
                sqlConfig.AddOrCombine(l);
            }
        }

        /// <summary>
        /// 添加sql
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sqlConfig"></param>
        /// <param name="sqlInfos"></param>
        public static void AddSqls<TEntity>(this ISqlConfig sqlConfig,
            IEnumerable<KeyValuePair<string, IConfigSqlInfo>> sqlInfos)
        {
            sqlConfig.AddSqls(EFHelper.Services.EFCoreExUtility.GetTableName(typeof(TEntity), true), sqlInfos);
        }

        /// <summary>
        /// 添加sql
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="tableName"></param>
        /// <param name="sqlInfos"></param>
        public static void AddSqls(this ISqlConfig sqlConfig, string tableName, 
            IEnumerable<KeyValuePair<string, IConfigSqlInfo>> sqlInfos)
        {
            foreach (var pair in sqlInfos)
            {
                sqlConfig.Add(tableName, pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// 加载sql的配置文件
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configFilePath"></param>
        /// <param name="encoding"></param>
        public static void LoadFile(this ISqlConfig sqlConfig, string configFilePath, Encoding encoding)
        {
            var config = CommonExtensions.JsonToObjectNeedDefaultValue<JsonConfigTableInfo>(File.ReadAllText(configFilePath, encoding));
            if (string.IsNullOrEmpty(config.Name))
            {
                config.Name = Path.GetFileNameWithoutExtension(configFilePath);
            }
            sqlConfig.AddOrCombine(config);
        }

        /// <summary>
        /// 加载sql的配置文件
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configFile"></param>
        /// <param name="encoding"></param>
        public static void LoadFile(this ISqlConfig sqlConfig, FileInfo configFile, Encoding encoding)
        {
            sqlConfig.LoadFile(configFile.FullName, encoding);
        }

        /// <summary>
        /// 加载sql的配置文件（Encoding默认为Encoding.UTF8）
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configFilePath"></param>
        public static void LoadFile(this ISqlConfig sqlConfig, string configFilePath)
        {
            sqlConfig.LoadFile(configFilePath, Encoding.UTF8);
        }

        /// <summary>
        /// 加载sql的配置文件（Encoding默认为Encoding.UTF8）
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configFile"></param>
        public static void LoadFile(this ISqlConfig sqlConfig, FileInfo configFile)
        {
            sqlConfig.LoadFile(configFile, Encoding.UTF8);
        }

        /// <summary>
        /// 从目录中获取sql的配置文件
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configDirectoryPath"></param>
        /// <param name="encoding"></param>
        /// <param name="isAllDirectories"></param>
        /// <param name="fileType"></param>
        public static void LoadDirectory(this ISqlConfig sqlConfig, string configDirectoryPath, Encoding encoding, 
            bool isAllDirectories = true,
            SqlConfigFileExtType fileType = SqlConfigFileExtType.json)
        {
            bool bLoad = false;
            string[] files;
            if (isAllDirectories)
            {
                files = Directory.GetFiles(configDirectoryPath, "*", SearchOption.AllDirectories); 
            }
            else
            {
                files = Directory.GetFiles(configDirectoryPath);
            }
            if (files != null && files.Length > 0)
            {
                if (fileType == SqlConfigFileExtType.all)
                {
                    foreach (var f in files)
                    {
                        sqlConfig.LoadFile(f, encoding);
                    }
                    bLoad = true;
                }
                else
                {
                    var fileExts = new List<string>();
                    if ((fileType & SqlConfigFileExtType.json) == SqlConfigFileExtType.json)
                    {
                        fileExts.Add(EFHelper.Services.EFCoreExUtility.GetEnumDescription(SqlConfigFileExtType.json));
                    }
                    if ((fileType & SqlConfigFileExtType.txt) == SqlConfigFileExtType.txt)
                    {
                        fileExts.Add(EFHelper.Services.EFCoreExUtility.GetEnumDescription(SqlConfigFileExtType.txt));
                    }

                    var finfos = files.Select(l => new FileInfo(l)).Where(l => fileExts.Contains(l.Extension.ToLower())).ToList();
                    if (finfos != null && finfos.Count > 0)
                    {
                        foreach (var f in finfos)
                        {
                            sqlConfig.LoadFile(f, encoding);
                        }
                        bLoad = true;
                    }
                }
            }

            if (!bLoad)
            {
                throw new ArgumentException($"The directory [{configDirectoryPath}] does not exist SQL configuration file.",
                    nameof(configDirectoryPath));
            }
        }

        /// <summary>
        /// 从目录中获取sql的配置文件
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configDirectoryPath"></param>
        /// <param name="isAllDirectories"></param>
        /// <param name="fileType"></param>
        public static void LoadDirectory(this ISqlConfig sqlConfig, string configDirectoryPath,
            bool isAllDirectories = true,
            SqlConfigFileExtType fileType = SqlConfigFileExtType.json)
        {
            sqlConfig.LoadDirectory(configDirectoryPath, Encoding.UTF8, isAllDirectories, fileType);
        }

        /// <summary>
        /// 从目录中获取sql的配置文件
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configDirectory"></param>
        /// <param name="isAllDirectories"></param>
        /// <param name="fileType"></param>
        public static void LoadDirectory(this ISqlConfig sqlConfig, DirectoryInfo configDirectory,
            bool isAllDirectories = true,
            SqlConfigFileExtType fileType = SqlConfigFileExtType.json)
        {
            sqlConfig.LoadDirectory(configDirectory.FullName, Encoding.UTF8, isAllDirectories, fileType);
        }

        /// <summary>
        /// 从目录中获取sql的配置文件
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configDirectory"></param>
        /// <param name="encoding"></param>
        /// <param name="isAllDirectories"></param>
        /// <param name="fileType"></param>
        public static void LoadDirectory(this ISqlConfig sqlConfig, DirectoryInfo configDirectory, 
            Encoding encoding, bool isAllDirectories = true,
            SqlConfigFileExtType fileType = SqlConfigFileExtType.json)
        {
            sqlConfig.LoadDirectory(configDirectory.FullName, encoding, isAllDirectories, fileType);
        }

        /// <summary>
        /// 获取sql的配置信息
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="tableName"></param>
        /// <param name="sqlName"></param>
        /// <returns></returns>
        public static IConfigSqlInfo GetSqlInfo(this ISqlConfig sqlConfig, string tableName, string sqlName)
        {
            IConfigTableInfo table;
            if (sqlConfig.TableSqlInfos.TryGetValue(tableName, out table))
            {
                IConfigSqlInfo sqlInfo = null;
                if (table.Sqls.TryGetValue(sqlName, out sqlInfo))
                {
                    return sqlInfo;
                }
                else
                {
                    throw new ArgumentException($"The key [{sqlName}] does not exist in the {nameof(IConfigSqlInfo)} collection.",
                        nameof(sqlName));
                }
            }
            else
            {
                throw new ArgumentException($"The key [{tableName}] does not exist in the {nameof(IConfigTableInfo)} collection.",
                    nameof(tableName));
            }
        }

        /// <summary>
        /// 获取Table的配置信息
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="tableName"></param>
        /// <param name="sqlName"></param>
        /// <param name="sqlInfo"></param>
        /// <returns></returns>
        public static bool TryGetSqlInfo(this ISqlConfig sqlConfig, string tableName, string sqlName, out IConfigSqlInfo sqlInfo)
        {
            sqlInfo = null;
            IConfigTableInfo table;
            if (sqlConfig.TableSqlInfos.TryGetValue(tableName, out table))
            {
                if (table.Sqls.TryGetValue(sqlName, out sqlInfo))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
