using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache
{
    /// <summary>
    /// 缓存过期策略
    /// </summary>
    public interface IQueryCacheExpiryPolicy
    {
        /// <summary>
        /// 是否每次获取缓存之后都重新设置过期配置
        /// </summary>
        bool IsUpdateEach { get; }

        /// <summary>
        /// 获取过期时间(可能为null，null说明不过期)
        /// </summary>
        /// <returns></returns>
        DateTime? GetExpiryTime();
    }

    public interface IQueryCacheExpiryCanSetPolicy : IQueryCacheExpiryPolicy
    {
        /// <summary>
        /// 设置缓存为不过期
        /// </summary>
        void SetForever();

        /// <summary>
        /// 设置缓存为过期的
        /// </summary>
        /// <param name="expiry"></param>
        /// <param name="isUpdateEach">是否每次获取缓存之后都重新设置过期配置</param>
        void SetExpiry(TimeSpan expiry, bool isUpdateEach = false);

        /// <summary>
        /// 设置缓存为过期的
        /// </summary>
        /// <param name="expiry"></param>
        /// <param name="isUpdateEach">是否每次获取缓存之后都重新设置过期配置</param>
        void SetExpiry(DateTime expiry, bool isUpdateEach = false);

    }
}
