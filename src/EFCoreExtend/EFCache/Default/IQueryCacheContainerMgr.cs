using EFCoreExtend.EFCache.BaseCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.Default
{
    public interface IQueryCacheContainerMgr : IDisposable
    {
        bool IsUseCache { get; set; }
        bool TryGetIfUseCache(string tableName, out IQueryCache cache);
        void Clear();
    }
}
