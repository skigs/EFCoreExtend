using EFCoreExtend.Sql.SqlConfig.Policies;
using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    /// <summary>
    /// 策略扩展测试：InsertInto策略
    /// </summary>
    [SqlConfigPolicy("insinto")]
    public class InsertIntoPolicy : SqlConfigPolicy
    {
        /// <summary>
        /// 在sql中呈现的标签名：##insinto
        /// </summary>
        [DefaultValue("##insinto")]
        public string Tag { get; set; }
    }
}
