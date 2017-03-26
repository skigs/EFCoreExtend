using EFCoreExtend.Sql.SqlConfig.Default.Json;
using EFCoreExtend.Commons;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Default
{
    public class SqlConfig : ISqlConfig
    {
        readonly ConcurrentDictionary<string, IConfigTableInfo> _tableSqls = new ConcurrentDictionary<string, IConfigTableInfo>();
        public IReadOnlyDictionary<string, IConfigTableInfo> TableSqlInfos => _tableSqls as IReadOnlyDictionary<string, IConfigTableInfo>;
        /// <summary>
        /// 配置数据被修改了
        /// </summary>
        public event Action OnModified;

        public SqlConfig()
        {
        }

        public SqlConfig(Action onModified)
        {
            OnModified = onModified;
        }

        protected IConfigTableInfoModifier AsModifiers(IConfigTableInfo sqlTable, string paramName)
        {
            var modifier = sqlTable as IConfigTableInfoModifier;
            if (modifier == null)
            {
                throw new ArgumentException($"The {nameof(IConfigTableInfo)} can not as {nameof(IConfigTableInfoModifier)}",
                    paramName);
            }
            return modifier;
        }

        public void AddOrCombine(IConfigTableInfo sqlTable)
        {
            var tconfig = _tableSqls.GetOrAdd(sqlTable.Name, sqlTable);
            if (tconfig != sqlTable)
            {
                var modifier = AsModifiers(tconfig, nameof(sqlTable));

                if (sqlTable.Sqls?.Count > 0)
                {
                    foreach (var pair in sqlTable.Sqls)
                    {
                        modifier.AddSql(pair.Key, pair.Value);
                    } 
                }

                if (sqlTable.Policies?.Count > 0)
                {
                    foreach (var pair in sqlTable.Policies)
                    {
                        modifier.AddPolicy(pair.Key, pair.Value);
                    } 
                }
            }
            OnModified?.Invoke();
        }

        public void Add(string tableName, string sqlName, IConfigSqlInfo sqlInfo)
        {
            var tconfig = _tableSqls.GetOrAdd(tableName, new ConfigTableInfo(tableName));
            var modifier = AsModifiers(tconfig, tableName);
            modifier.AddSql(sqlName, sqlInfo);
            OnModified?.Invoke();
        }

        public bool TryRemove(string tableName, string sqlName, out IConfigSqlInfo sqlInfo)
        {
            sqlInfo = null;
            IConfigTableInfo tconfig;
            if (TableSqlInfos.TryGetValue(tableName, out tconfig))
            {
                var modifier = AsModifiers(tconfig, tableName);
                bool bRtn = modifier.TryRemoveSql(sqlName, out sqlInfo);
                if (bRtn)
                {
                    OnModified?.Invoke();
                }
                return bRtn;
            }
            return false;
        }

        public bool TryRemove(string tableName, out IConfigTableInfo tableModel)
        {
            bool bRtn = _tableSqls.TryRemove(tableName, out tableModel);
            if (bRtn)
            {
                OnModified?.Invoke();
            }
            return bRtn;
        }

    }
}
