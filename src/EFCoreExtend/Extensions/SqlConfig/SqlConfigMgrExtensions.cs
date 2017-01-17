using EFCoreExtend.Sql.SqlConfig;
using EFCoreExtend.Sql.SqlConfig.Executors;
using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class SqlConfigMgrExtensions
    {
        /// <summary>
        /// 获取sql的执行器
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ISqlConfigExecutor GetExecutor(this ISqlConfigManager mgr, DbContext db, Type tableEntityType, 
            [CallerMemberName] string sqlName = null)
        {
            return mgr.GetExecutor(db, EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType), sqlName);
        }

        /// <summary>
        /// 获取sql的执行器
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="mgr"></param>
        /// <param name="db"></param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ISqlConfigExecutor GetExecutor<TEntity>(this ISqlConfigManager mgr, DbContext db, 
            [CallerMemberName] string sqlName = null)
        {
            return mgr.GetExecutor(db, typeof(TEntity), sqlName);
        }

        /// <summary>
        /// 获取sql的执行器
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ISqlConfigExecutor GetExecutor<TEntity>(this DbContext db, [CallerMemberName] string sqlName = null)
        {
            return EFHelper.Services.SqlConfigMgr.GetExecutor<TEntity>(db, sqlName);
        }

        /// <summary>
        /// 获取sql的执行器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ISqlConfigExecutor GetExecutor(this DbContext db, Type tableEntityType, [CallerMemberName] string sqlName = null)
        {
            return EFHelper.Services.SqlConfigMgr.GetExecutor(db, tableEntityType, sqlName);
        }

        /// <summary>
        /// 获取sql的执行器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ISqlConfigExecutor GetExecutor(this DbContext db, string tableName, [CallerMemberName] string sqlName = null)
        {
            return EFHelper.Services.SqlConfigMgr.GetExecutor(db, tableName, sqlName);
        }

        /// <summary>
        /// 获取配置的Table信息
        /// </summary>
        /// <typeparam name="TEntity">表的实体类型（用于获取表名）</typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public static DBConfigTable GetConfigTable<TEntity>(this DbContext db)
        {
            return db.GetConfigTable(typeof(TEntity));
        }

        /// <summary>
        /// 获取配置的Table信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableEntityType">表的实体类型（用于获取表名）</param>
        /// <returns></returns>
        public static DBConfigTable GetConfigTable(this DbContext db, Type tableEntityType)
        {
            return db.GetConfigTable(EFHelper.Services.EFCoreExUtility.GetTableName(tableEntityType));
        }

        /// <summary>
        /// 获取配置的Table信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static DBConfigTable GetConfigTable(this DbContext db, string tableName)
        {
            return new DBConfigTable(EFHelper.Services.SqlConfigMgr.Config, tableName, db);
        }

        /// <summary>
        /// 获取sql的执行器
        /// </summary>
        /// <param name="tinfo"></param>
        /// <param name="sqlName">sql名称</param>
        /// <returns></returns>
        public static ISqlConfigExecutor GetExecutor(this DBConfigTable tinfo, [CallerMemberName] string sqlName = null)
        {
            return tinfo.DB.GetExecutor(tinfo.TableName, sqlName);
        }

    }
}
