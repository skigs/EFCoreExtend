using EFCoreExtend.Commons;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreExtend.EFCache.BaseCache.LocalCaches.Default
{
    /// <summary>
    /// 缓存清理器
    /// </summary>
    public class LocalCacheClearManager
    {
        static TimeSpan _clearSpan = TimeSpan.FromMinutes(15);
        readonly static InitAction _doStart;
        readonly static ConcurrentDictionary<string, ConcurrentDictionary<string, ILocalCacheInfo>> _dictCacheMgr;

        static LocalCacheClearManager()
        {
            _dictCacheMgr = new ConcurrentDictionary<string, ConcurrentDictionary<string, ILocalCacheInfo>>();
            _doStart = new InitAction(ClearCache);
        }

        public static void Add(string key, ConcurrentDictionary<string, ILocalCacheInfo> val)
        {
            if (key != null && val != null)
            {
                _dictCacheMgr[key] = val;
            }
        }

        public static void Remove(string key)
        {
            ConcurrentDictionary<string, ILocalCacheInfo> val;
            _dictCacheMgr.TryRemove(key, out val);
        }

        /// <summary>
        /// 默认15分钟
        /// </summary>
        /// <param name="clearSpan"></param>
        public static void StartClear(TimeSpan? clearSpan = null)
        {
            if (clearSpan != null)
            {
                _clearSpan = clearSpan.Value;
            }
            
            _doStart.Invoke();
        }

        static Timer _timer;
        static void ClearCache()
        {
            using (_timer) { }

            List<ConcurrentDictionary<string, ILocalCacheInfo>> listDict = null;
            ConcurrentDictionary<string, ILocalCacheInfo> dict = null;
            List<KeyValuePair<string, ILocalCacheInfo>> listCache = null;
            KeyValuePair<string, ILocalCacheInfo> pair;
            ILocalCacheInfo cacheModel = null;
            int i = 0, j = 0;

            _timer = new Timer(obj =>
            {
                try
                {
                    if (_dictCacheMgr.Count > 0)
                    {
                        listDict = _dictCacheMgr.Values.ToList();
                        //DateTime now = DateTime.Now;

                        for (i = 0; i < listDict.Count; i++)
                        {
                            dict = listDict[i];
                            if (dict != null && dict.Count > 0)
                            {
                                listCache = dict.ToList();
                                for (j = 0; j < listCache.Count; j++)
                                {
                                    pair = listCache[j];
                                    if (pair.Value != null)
                                    {
                                        //if (!pair.Value.IsValidTime(now))
                                        if (!pair.Value.IsValid)
                                        {
                                            //过期缓存无效清理
                                            dict.TryRemove(pair.Key, out cacheModel);
                                        }
                                    }
                                    else
                                    {
                                        //清除空缓存Model
                                        dict.TryRemove(pair.Key, out cacheModel);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }

            }, null, _clearSpan, _clearSpan);
        }

    }
}
