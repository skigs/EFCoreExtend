using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.Default
{
    /// <summary>
    /// 缓存过期类型
    /// </summary>
    public enum QueryCacheExpiryPolicyType
    {
        /// <summary>
        /// 不设置过期
        /// </summary>
        Forever,
        /// <summary>
        /// 设置过期，使用TimeSpan
        /// </summary>
        ExpirySpan,
        /// <summary>
        /// 设置过期，使用DateTime
        /// </summary>
        ExpiryDate,
    }
}
