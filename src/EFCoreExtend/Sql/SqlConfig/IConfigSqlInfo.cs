using EFCoreExtend.Sql.SqlConfig.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig
{
    /// <summary>
    /// sql的配置模型
    /// </summary>
    public interface IConfigSqlInfo
    {
        /// <summary>
        /// 配置的sql
        /// </summary>
        string Sql { get; }
        /// <summary>
        /// sql执行的类型
        /// </summary>
        ConfigSqlExecuteType Type { get; }
        /// <summary>
        /// 配置的策略集合
        /// </summary>
        IReadOnlyDictionary<string, object> Policies { get; }
    }

    public interface IConfigSqlInfoModifier : IConfigSqlInfo
    {
        /// <summary>
        /// 配置的sql
        /// </summary>
        new string Sql { get; set; }
        /// <summary>
        /// sql执行的类型
        /// </summary>
        new ConfigSqlExecuteType Type { get; set; }
        /// <summary>
        /// 配置的策略集合
        /// </summary>
        new IDictionary<string, object> Policies { get; }
    }

}
