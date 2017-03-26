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
using EFCoreExtend.Lua.SqlConfig;

namespace EFCoreExtend
{
    public static class LuaSqlConfigExtensions
    {
        /// <summary>
        /// 加载lua sql的配置文件
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="tableName">表名</param>
        /// <param name="luascript">lua脚本</param>
        public static void Add(this ILuaSqlConfigManager sqlConfig, string tableName, string luascript)
        {
            sqlConfig.Config.Add(tableName, luascript);
        }

        /// <summary>
        /// 加载lua sql的配置文件
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configFilePath"></param>
        /// <param name="encoding"></param>
        public static void LoadFile(this ILuaSqlConfigManager sqlConfig, string configFilePath, Encoding encoding)
        {
            sqlConfig.Config.Add(Path.GetFileNameWithoutExtension(configFilePath), File.ReadAllText(configFilePath, encoding));
        }

        /// <summary>
        /// 加载luasql的配置文件
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configFile"></param>
        /// <param name="encoding"></param>
        public static void LoadFile(this ILuaSqlConfigManager sqlConfig, FileInfo configFile, Encoding encoding)
        {
            sqlConfig.LoadFile(configFile.FullName, encoding);
        }

        /// <summary>
        /// 加载luasql的配置文件（Encoding默认为Encoding.UTF8）
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configFilePath"></param>
        public static void LoadFile(this ILuaSqlConfigManager sqlConfig, string configFilePath)
        {
            sqlConfig.LoadFile(configFilePath, Encoding.UTF8);
        }

        /// <summary>
        /// 加载sql的配置文件（Encoding默认为Encoding.UTF8）
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configFile"></param>
        public static void LoadFile(this ILuaSqlConfigManager sqlConfig, FileInfo configFile)
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
        public static void LoadDirectory(this ILuaSqlConfigManager sqlConfig, string configDirectoryPath, Encoding encoding, 
            bool isAllDirectories = true,
            LuaSqlConfigFileExtType fileType = LuaSqlConfigFileExtType.lua)
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

            if (files?.Length > 0)
            {
                if (fileType == LuaSqlConfigFileExtType.all)
                {
                    //先查找目录下时候有全局文件，全局文件先加载(带luaglobal前缀的文件)
                    var globalfile = files.Where(l => Path.GetFileNameWithoutExtension(l).ToLower().StartsWith(LuaSqlConfigConst.LuaGlobalFileLabel))
                        .OrderByDescending(l => Path.GetFileNameWithoutExtension(l)).ToList();
                    if (globalfile?.Count > 0)
                    {
                        //先加载全局文件
                        foreach (var g in globalfile)
                        {
                            sqlConfig.LoadFile(g, encoding);
                        }

                        foreach (var f in files)
                        {
                            if (!globalfile.Contains(f))
                            {
                                sqlConfig.LoadFile(f, encoding); 
                            }
                        }
                    }
                    else
                    {
                        foreach (var f in files)
                        {
                            sqlConfig.LoadFile(f, encoding);
                        }
                    }

                    bLoad = true;
                }
                else
                {
                    var fileExts = new List<string>();
                    if ((fileType & LuaSqlConfigFileExtType.txt) == LuaSqlConfigFileExtType.txt)
                    {
                        fileExts.Add(EFHelper.Services.EFCoreExUtility.GetEnumDescription(LuaSqlConfigFileExtType.txt));
                    }

                    if ((fileType & LuaSqlConfigFileExtType.lua) == LuaSqlConfigFileExtType.lua)
                    {
                        fileExts.Add(EFHelper.Services.EFCoreExUtility.GetEnumDescription(LuaSqlConfigFileExtType.lua));
                    }

                    var finfos = files.Select(l => new FileInfo(l)).Where(l => fileExts.Contains(l.Extension.ToLower())).ToList();
                    if (finfos?.Count > 0)
                    {
                        //先查找目录下时候有全局文件，全局文件先加载(带luaglobal前缀的文件)
                        var globalfile = finfos.Where(l => Path.GetFileNameWithoutExtension(l.Name)
                                .ToLower().StartsWith(LuaSqlConfigConst.LuaGlobalFileLabel))
                            .OrderByDescending(l => Path.GetFileNameWithoutExtension(l.Name)).ToList();
                        if (globalfile?.Count > 0)
                        {
                            //先加载全局文件
                            foreach (var g in globalfile)
                            {
                                sqlConfig.LoadFile(g, encoding);
                            }

                            foreach (var f in finfos)
                            {
                                if (!globalfile.Contains(f))
                                {
                                    sqlConfig.LoadFile(f, encoding);
                                }
                            }
                        }
                        else
                        {
                            foreach (var f in finfos)
                            {
                                sqlConfig.LoadFile(f, encoding);
                            }
                        }

                        bLoad = true;
                    }
                }
            }

            if (!bLoad)
            {
                throw new ArgumentException($"The directory [{configDirectoryPath}] does not exist LuaSQL configuration file.",
                    nameof(configDirectoryPath));
            }
        }

        /// <summary>
        /// 从目录中获取sql的配置文件（Encoding默认为Encoding.UTF8）
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configDirectoryPath"></param>
        /// <param name="isAllDirectories"></param>
        /// <param name="fileType"></param>
        public static void LoadDirectory(this ILuaSqlConfigManager sqlConfig, string configDirectoryPath,
            bool isAllDirectories = true,
            LuaSqlConfigFileExtType fileType = LuaSqlConfigFileExtType.lua)
        {
            sqlConfig.LoadDirectory(configDirectoryPath, Encoding.UTF8, isAllDirectories, fileType);
        }

        /// <summary>
        /// 从目录中获取sql的配置文件（Encoding默认为Encoding.UTF8）
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configDirectory"></param>
        /// <param name="isAllDirectories"></param>
        /// <param name="fileType"></param>
        public static void LoadDirectory(this ILuaSqlConfigManager sqlConfig, DirectoryInfo configDirectory,
            bool isAllDirectories = true,
            LuaSqlConfigFileExtType fileType = LuaSqlConfigFileExtType.lua)
        {
            sqlConfig.LoadDirectory(configDirectory.FullName, Encoding.UTF8, isAllDirectories, fileType);
        }

        /// <summary>
        /// 从目录中获取sql的配置文件(（Encoding默认为Encoding.UTF8）
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="configDirectory"></param>
        /// <param name="encoding"></param>
        /// <param name="isAllDirectories"></param>
        /// <param name="fileType"></param>
        public static void LoadDirectory(this ILuaSqlConfigManager sqlConfig, DirectoryInfo configDirectory, 
            Encoding encoding, bool isAllDirectories = true,
            LuaSqlConfigFileExtType fileType = LuaSqlConfigFileExtType.lua)
        {
            sqlConfig.LoadDirectory(configDirectory.FullName, encoding, isAllDirectories, fileType);
        }

    }
}
