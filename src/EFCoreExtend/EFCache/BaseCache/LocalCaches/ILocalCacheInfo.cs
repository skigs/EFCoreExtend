using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.BaseCache.LocalCaches
{
    /// <summary>
    /// 缓存信息
    /// </summary>
    public interface ILocalCacheInfo
    {
        object Data { get; }

        bool IsValid { get; }

        void UpdateExpiry(DateTime expiry);
    }
}
