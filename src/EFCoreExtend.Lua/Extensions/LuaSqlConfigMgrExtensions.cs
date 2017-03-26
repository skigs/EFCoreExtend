using EFCoreExtend.Commons;
using EFCoreExtend.Lua.SqlConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class LuaSqlConfigMgrExtensions
    {

        /// <summary>
        /// 获取lua sql的执行器
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ILuaSqlConfigExecutor GetExecutor(this ILuaSqlConfigManager mgr, DbContext db, Type tableEntityType, 
            [CallerMemberName] string sqlName = null)
        {
            return mgr.GetExecutor(db, EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), sqlName);
        }

        /// <summary>
        /// 获取lua sql的执行器
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="mgr"></param>
        /// <param name="db"></param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ILuaSqlConfigExecutor GetExecutor<TEntity>(this ILuaSqlConfigManager mgr, DbContext db, 
            [CallerMemberName] string sqlName = null)
        {
            return mgr.GetExecutor(db, typeof(TEntity), sqlName);
        }

        /// <summary>
        /// 获取lua sql的执行器
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ILuaSqlConfigExecutor GetLuaExecutor<TEntity>(this DbContext db, [CallerMemberName] string sqlName = null)
        {
            return EFHelper.Services.GetLuaSqlMgr().GetExecutor<TEntity>(db, sqlName);
        }

        /// <summary>
        /// 获取lua sql的执行器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ILuaSqlConfigExecutor GetLuaExecutor(this DbContext db, Type tableEntityType, [CallerMemberName] string sqlName = null)
        {
            return EFHelper.Services.GetLuaSqlMgr().GetExecutor(db, tableEntityType, sqlName);
        }

        /// <summary>
        /// 获取lua sql的执行器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ILuaSqlConfigExecutor GetLuaExecutor(this DbContext db, string tableName, [CallerMemberName] string sqlName = null)
        {
            return EFHelper.Services.GetLuaSqlMgr().GetExecutor(db, tableName, sqlName);
        }

        /// <summary>
        /// 获取lua配置的Table信息
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public static LuaDBConfigTable GetLuaConfigTable<TEntity>(this DbContext db)
        {
            return db.GetLuaConfigTable(typeof(TEntity));
        }

        /// <summary>
        /// 获取lua配置的Table信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <returns></returns>
        public static LuaDBConfigTable GetLuaConfigTable(this DbContext db, Type tableEntityType)
        {
            return db.GetLuaConfigTable(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType));
        }

        /// <summary>
        /// 获取lua配置的Table信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static LuaDBConfigTable GetLuaConfigTable(this DbContext db, string tableName)
        {
            return new LuaDBConfigTable(EFHelper.Services.GetLuaSqlMgr().Config, tableName, db);
        }

        /// <summary>
        /// 获取lua sql的执行器
        /// </summary>
        /// <param name="tinfo"></param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ILuaSqlConfigExecutor GetLuaExecutor(this IDBConfigTable tinfo, [CallerMemberName] string sqlName = null)
        {
            return tinfo.DB.GetLuaExecutor(tinfo.TableName, sqlName);
        }

    }
}
