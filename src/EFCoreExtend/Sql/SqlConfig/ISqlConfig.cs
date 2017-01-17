using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig
{
    /// <summary>
    /// sql配置存储
    /// </summary>
    public interface ISqlConfig
    {
        /// <summary>
        /// 配置的Tables
        /// </summary>
        IReadOnlyDictionary<string, IConfigTableInfo> TableSqlInfos { get; }

        /// <summary>
        /// 添加或者合并Table的配置(如果Table不存在，那么添加到TableSqlInfos中，如果存在，那么合并不同的sql/policy，但是如果存在相同sql/policy的那么抛异常)
        /// </summary>
        /// <param name="sqlTable"></param>
        void AddOrCombine(IConfigTableInfo sqlTable);

        /// <summary>
        /// 添加sql的配置
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="sqlName"></param>
        /// <param name="sqlInfo"></param>
        void Add(string tableName, string sqlName, IConfigSqlInfo sqlInfo);

        /// <summary>
        /// 移除sql的配置
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="sqlName"></param>
        /// <param name="sqlInfo"></param>
        /// <returns></returns>
        bool TryRemove(string tableName, string sqlName, out IConfigSqlInfo sqlInfo);

        /// <summary>
        /// 移除table的配置
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="sqlTable"></param>
        /// <returns></returns>
        bool TryRemove(string tableName, out IConfigTableInfo sqlTable);

    }
}
