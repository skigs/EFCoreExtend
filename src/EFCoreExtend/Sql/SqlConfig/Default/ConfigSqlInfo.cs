using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System;
using EFCoreExtend.Sql.SqlConfig.Policies;
using System.Collections.Concurrent;

namespace EFCoreExtend.Sql.SqlConfig.Default
{
    public class ConfigSqlInfo : IConfigSqlInfo, IConfigSqlInfoModifier
    {
        public string Sql { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(ConfigSqlExecuteType.notsure)]
        public ConfigSqlExecuteType Type { get; set; }

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

        IDictionary<string, object> IConfigSqlInfoModifier.Policies => _policies;

    }
}
