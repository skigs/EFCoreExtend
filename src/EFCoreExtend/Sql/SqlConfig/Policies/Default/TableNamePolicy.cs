using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    /// <summary>
    /// 表名在sql中呈现的标签名策略
    /// </summary>
    [SqlConfigPolicy(SqlConfigConst.TableNamePolicyName)]
    public class TableNamePolicy : SqlConfigPolicy
    {
        /// <summary>
        /// 表名在sql中呈现的标签名，默认为：##tname
        /// </summary>
        [DefaultValue(SqlConfigConst.TableNameLabel)]
        public string Tag { get; set; }

        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 后缀
        /// </summary>
        public string Suffix { get; set; }

    }
}
