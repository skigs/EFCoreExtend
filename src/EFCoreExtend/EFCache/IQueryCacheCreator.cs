using EFCoreExtend.EFCache.BaseCache;

namespace EFCoreExtend.EFCache
{
    public interface IQueryCacheCreator
    {
        /// <summary>
        /// 根据表名创建查询缓存实例对象
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        IQueryCache Create(string tableName);
    }
}
