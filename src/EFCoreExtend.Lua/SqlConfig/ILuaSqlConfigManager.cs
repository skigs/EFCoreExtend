using EFCoreExtend.Lua.SqlConfig.Policies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig
{
    /// <summary>
    /// lua配置sql管理器
    /// </summary>
    public interface ILuaSqlConfigManager
    {
        /// <summary>
        /// 策略管理器
        /// </summary>
        ILuaSqlPolicyManager PolicyMgr { get; }

        /// <summary>
        /// Sql的配置
        /// </summary>
        ILuaSqlConfig Config { get; }

        /// <summary>
        /// 获取sql的执行器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <param name="sqlName"></param>
        /// <returns></returns>
        ILuaSqlConfigExecutor GetExecutor(DbContext db, string tableName, [CallerMemberName] string sqlName = null);
    }
}
