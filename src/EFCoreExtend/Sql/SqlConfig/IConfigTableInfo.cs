using EFCoreExtend.EFCache;
using EFCoreExtend.Sql.SqlConfig.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig
{
    /// <summary>
    /// Table的配置模型
    /// </summary>
    public interface IConfigTableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 表下所配置的sql
        /// </summary>
        IReadOnlyDictionary<string, IConfigSqlInfo> Sqls { get; }
        /// <summary>
        /// 配置的策略集合
        /// </summary>
        IReadOnlyDictionary<string, object> Policies { get; }
    }

    public interface IConfigTableInfoModifier : IConfigTableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        new string Name { get; set; }
        /// <summary>
        /// 表下所配置的sql
        /// </summary>
        new IDictionary<string, IConfigSqlInfo> Sqls { get; }
        /// <summary>
        /// 配置的策略集合
        /// </summary>
        new IDictionary<string, object> Policies { get; }
    }

}
