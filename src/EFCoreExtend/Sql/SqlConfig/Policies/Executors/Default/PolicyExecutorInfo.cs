using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors.Default
{
    public class PolicyExecutorInfo : IPolicyExecutorInfo, ISqlExecutePolicyExecutorInfo, 
        ISqlPreExecutePolicyExecutorInfo, ISqlInitPolicyExecutorInfo
    {
        public ConfigSqlExecuteType ExecuteType { get; set; }

        public IConfigSqlInfo SqlInfo { get; set; }

        public IConfigTableInfo TableInfo { get; set; }

        public DbContext DB { get; set; }

        public object GlobalPolicy { get; set; }

        public bool IsEnd { get; set; }

        public IReadOnlyDictionary<string, IDictionary<string, string>> NewlySqls { get; set; }

        public object ParameterPolicy { get; set; }

        public string PolicyName { get; set; }

        public object ReturnValue { get; set; }

        public Type ReturnType { get; set; }

        public string Sql { get; set; }

        public string SqlName { get; set; }

        public IDataParameter[] SqlParams { get; set; }

        public string TableName { get; set; }

        public IReadOnlyDictionary<string, IConfigTableInfo> TableSqlInfos { get; set; }

        public Func<object> ToDBExecutor { get; set; }

        IReadOnlyList<IDataParameter> ISqlExecutePolicyExecutorInfo.SqlParams { get { return this.SqlParams; } }

        public IDictionary<string, object> SqlExecutorTempDatas { get; set; }

        IDictionary<string, object> _TempDatas = new ConcurrentDictionary<string, object>();
        public IDictionary<string, object> TempDatas => _TempDatas;

        public object GetPolicy(IConfigSqlInfo sqlInfo, IConfigTableInfo tableInfo)
        {
            object tempPolicy = null;
            if (ParameterPolicy != null)
            {
                return ParameterPolicy;
            }
            else if (sqlInfo.Policies?.TryGetValue(PolicyName, out tempPolicy) == true)
            {
                return tempPolicy;
            }
            else if (tableInfo.Policies?.TryGetValue(PolicyName, out tempPolicy) == true)
            {
                return tempPolicy;
            }
            else
            {
                return GlobalPolicy;
            }
        }

        public object GetPolicy()
        {
            return GetPolicy(SqlInfo, TableInfo);
        }

    }
}
