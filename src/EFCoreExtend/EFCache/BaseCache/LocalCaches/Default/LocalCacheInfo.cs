using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.BaseCache.LocalCaches.Default
{
    /// <summary>
    /// 缓存信息
    /// </summary>
    public class LocalCacheInfo : ILocalCacheInfo
    {
        DateTime? _lastCacheTime;

        object _data;
        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public LocalCacheInfo(object data)
        {
            _data = data;
        }

        public LocalCacheInfo(object data, TimeSpan expiry)
        {
            _data = data;
            UpdateExpiry(DateTime.Now.Add(expiry));
        }

        public LocalCacheInfo(object data, DateTime expiry)
        {
            _data = data;
            UpdateExpiry(expiry);
        }

        public bool IsValid
        {
            get
            {
                return IsValidTime(DateTime.Now);
            }
        }

        protected bool IsValidTime(DateTime time)
        {
            if (_lastCacheTime.HasValue)
            {
                return _lastCacheTime.Value > time;
            }
            else
            {
                return true;
            }
        }

        public void UpdateExpiry(DateTime expiry)
        {
            _lastCacheTime = expiry;
        }

    }
}
