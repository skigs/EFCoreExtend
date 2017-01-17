using EFCoreExtend.Sql.SqlConfig.Executors;
using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Executors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig
{
    /// <summary>
    /// sql配置管理器
    /// </summary>
    public interface ISqlConfigManager
    {
        /// <summary>
        /// Sql的配置
        /// </summary>
        ISqlConfig Config { get; }
        /// <summary>
        /// 策略管理器
        /// </summary>
        ISqlPolicyManager PolicyMgr { get; }
        /// <summary>
        /// 获取sql的执行器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <param name="sqlName"></param>
        /// <returns></returns>
        ISqlConfigExecutor GetExecutor(DbContext db, string tableName, [CallerMemberName] string sqlName = null);
    }
}
