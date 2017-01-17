using EFCoreExtend.Sql.SqlConfig.Policies.Default;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Default.Json
{
    public class JsonConfigTableInfo : ConfigTableInfo
    {
        ConcurrentDictionary<string, ConfigSqlInfo> _sqls;
        public new IReadOnlyDictionary<string, ConfigSqlInfo> Sqls
        {
            get
            {
                return this._sqls;
            }
            set
            {
                value.CheckPairValueIsNull(nameof(Sqls));

                if (value is ConcurrentDictionary<string, ConfigSqlInfo>)
                {
                    this._sqls = value as ConcurrentDictionary<string, ConfigSqlInfo>;
                }
                else
                {
                    this._sqls = new ConcurrentDictionary<string, ConfigSqlInfo>(value);
                }
                base.Sqls = this._sqls.ToDictionary(l => l.Key, l => (IConfigSqlInfo)l.Value);
            }
        }
    }
}
