using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Extensions.EFQueryCaches
{
    /// <summary>
    /// 缓存的类型（用于扩展方法）
    /// </summary>
    public enum EFQueryCacheType
    {
        /// <summary>
        /// Sql的Query
        /// </summary>
        query = 0,
        /// <summary>
        /// Sql的Scalar
        /// </summary>
        scalar = 1,

        /// <summary>
        /// EF的IQueryable的List
        /// </summary>
        List = 100,
        /// <summary>
        /// EF的IQueryable的FirstOrDefault
        /// </summary>
        FirstOrDefault,
        /// <summary>
        /// EF的IQueryable的Count
        /// </summary>
        Count,
        /// <summary>
        /// EF的IQueryable的LongCount
        /// </summary>
        LongCount,
    }
}
