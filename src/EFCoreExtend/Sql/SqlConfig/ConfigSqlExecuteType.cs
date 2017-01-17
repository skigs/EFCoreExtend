using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig
{
    /// <summary>
    /// sql执行的类型
    /// </summary>
    public enum ConfigSqlExecuteType
    {
        /// <summary>
        /// 不确定
        /// </summary>
        notsure,
        /// <summary>
        /// 查询
        /// </summary>
        query,
        /// <summary>
        /// 非查询
        /// </summary>
        nonquery,
        /// <summary>
        /// Scalar查询
        /// </summary>
        scalar,
        /// <summary>
        /// 不用于执行的sql（例如：分部sql）
        /// </summary>
        nonexecute,
    }
}
