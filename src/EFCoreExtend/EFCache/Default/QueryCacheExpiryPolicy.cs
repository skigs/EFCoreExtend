using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.Default
{
    public class QueryCacheExpiryPolicy : IQueryCacheExpiryCanSetPolicy
    {
        DateTime? _expiryDate;
        /// <summary>
        /// 指定缓存的过期日期
        /// </summary>
        public DateTime? Date
        {
            get { return _expiryDate; }
            set
            {
                if (value != null && value.Value.Kind == DateTimeKind.Unspecified)
                {
                    //如果没有指定类型的，那么设置为本地时间
                    _expiryDate = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day,
                        value.Value.Hour, value.Value.Minute, value.Value.Second, value.Value.Millisecond, DateTimeKind.Local);
                }
                else
                {
                    _expiryDate = value;
                }

                if (value != null)
                {
                    _type = QueryCacheExpiryPolicyType.ExpiryDate; 
                }
            }
        }

        TimeSpan? _expirySpan;
        /// <summary>
        /// 指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
        /// </summary>
        public TimeSpan? Span
        {
            get { return _expirySpan; }
            set
            {
                _expirySpan = value;
                if (value != null)
                {
                    _type = QueryCacheExpiryPolicyType.ExpirySpan; 
                }
            }
        }

        bool _isUpdateEach;
        /// <summary>
        /// 是否每次获取缓存之后更新过期时间(这个属性 + span属性来进行模拟session访问更新过期时间)
        /// </summary>
        public bool IsUpdateEach
        {
            get { return _isUpdateEach; }
            set { _isUpdateEach = value; }
        }

        QueryCacheExpiryPolicyType? _type;
        //[JsonConverter(typeof(StringEnumConverter))]
        ////[DefaultValue(QueryCacheExpiryPolicyType.Forever)]
        //public QueryCacheExpiryPolicyType? Type
        //{
        //    get { return _type; }
        //    set
        //    {
        //        //value为null不赋值的目的：为了防止json配置初始化的时候配置了ExpirySpan / ExpiryDate的时候属性中自动设置的_type失效
        //        if (value != null)
        //        {
        //            _type = value; 
        //        }
        //    }
        //}

        public QueryCacheExpiryPolicy()
        {
            //SetNonExpiry();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expiry"></param>
        /// <param name="isUpdateEach">是否每次获取缓存之后都重新设置过期配置</param>
        public QueryCacheExpiryPolicy(TimeSpan expiry, bool isUpdateEach = false)
        {
            SetExpiry(expiry, isUpdateEach);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expiry"></param>
        /// <param name="isUpdateEach">是否每次获取缓存之后都重新设置过期配置</param>
        public QueryCacheExpiryPolicy(DateTime expiry, bool isUpdateEach = false)
        {
            SetExpiry(expiry, isUpdateEach);
        }

        /// <summary>
        /// 设置缓存为不过期
        /// </summary>
        public void SetForever()
        {
            _type = QueryCacheExpiryPolicyType.Forever;
        }

        /// <summary>
        /// 设置缓存为过期的
        /// </summary>
        /// <param name="expiry"></param>
        /// <param name="isUpdateEach">是否每次获取缓存之后都重新设置过期配置</param>
        public void SetExpiry(TimeSpan expiry, bool isUpdateEach = false)
        {
            Span = expiry;
            this.IsUpdateEach = isUpdateEach;
        }

        /// <summary>
        /// 设置缓存为过期的
        /// </summary>
        /// <param name="expiry"></param>
        /// <param name="isUpdateEach">是否每次获取缓存之后都重新设置过期配置</param>
        public void SetExpiry(DateTime expiry, bool isUpdateEach = false)
        {
            Date = expiry;
            this.IsUpdateEach = isUpdateEach;
        }

        public DateTime? GetExpiryTime()
        {
            DateTime? rtn = null;
            switch (_type)
            {
                case QueryCacheExpiryPolicyType.ExpirySpan:
                    if (Span.HasValue)
                    {
                        rtn = DateTime.Now.Add(Span.Value); 
                    }
                    break;
                case QueryCacheExpiryPolicyType.ExpiryDate:
                    rtn = Date;
                    break;
                case QueryCacheExpiryPolicyType.Forever:
                default:
                    break;
            }
            return rtn;
        }

    }
}
