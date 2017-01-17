using EFCoreExtend.EFCache.BaseCache;
using EFCoreExtend.EFCache.BaseCache.Default;

namespace EFCoreExtend.EFCache.Default
{
    public class QueryCacheCreator : IQueryCacheCreator
    {
        public IQueryCache Create(string tableName)
        {
            return new QueryCache();
        }
    }
}
