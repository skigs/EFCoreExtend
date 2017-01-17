using EFCoreExtend.EFCache;
using EFCoreExtend.EFCache.Default;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend.Sql.SqlConfig.Policies;

namespace EFCoreExtend.Sql.SqlConfig.Default
{
    public class ConfigTableInfo : IConfigTableInfo, IConfigTableInfoModifier
    {
        #region 数据字段
        public string Name { get; set; }

        ConcurrentDictionary<string, object> _policies;
        public IReadOnlyDictionary<string, object> Policies
        {
            get
            {
                return _policies;
            }
            set
            {
                if (value == null)
                {
                    _policies = new ConcurrentDictionary<string, object>();
                }
                else
                {
                    value.CheckPairValueIsNull(nameof(Policies));

                    if (value is ConcurrentDictionary<string, object>)
                    {
                        _policies = value as ConcurrentDictionary<string, object>;
                    }
                    else
                    {
                        _policies = new ConcurrentDictionary<string, object>(value);
                    }
                }
            }
        }

        ConcurrentDictionary<string, IConfigSqlInfo> _sqls;
        public IReadOnlyDictionary<string, IConfigSqlInfo> Sqls
        {
            get
            {
                return _sqls;
            }
            set
            {
                value.CheckPairValueIsNull(nameof(Sqls));
                CheckSqlIsNull(value, nameof(Sqls));

                if (value is ConcurrentDictionary<string, IConfigSqlInfo>)
                {
                    _sqls = value as ConcurrentDictionary<string, IConfigSqlInfo>;
                }
                else
                {
                    _sqls = new ConcurrentDictionary<string, IConfigSqlInfo>(value);
                }
            }
        }

        IDictionary<string, IConfigSqlInfo> IConfigTableInfoModifier.Sqls => _sqls;

        IDictionary<string, object> IConfigTableInfoModifier.Policies => _policies;
        #endregion

        public ConfigTableInfo()
        {
            _sqls = new ConcurrentDictionary<string, IConfigSqlInfo>();
        }

        public ConfigTableInfo(string name)
            : this()
        {
            name.CheckStringIsNullOrEmpty(nameof(name));

            this.Name = name;
        }

        public ConfigTableInfo(string name, IReadOnlyDictionary<string, IConfigSqlInfo> sqls)
        {
            name.CheckStringIsNullOrEmpty(nameof(name));

            this.Name = name;
            this.Sqls = sqls;
        }

        protected void CheckSqlIsNull(IEnumerable<KeyValuePair<string, IConfigSqlInfo>> pairs, string paramName)
        {
            var pair = pairs.Where(l => string.IsNullOrEmpty(l.Value.Sql)).FirstOrDefault();
            if (pair.Key != null)
            {
                throw new ArgumentException($"The sql value of the key [{pair.Key}] can not be null.", paramName);
            }
        }

    }
}
