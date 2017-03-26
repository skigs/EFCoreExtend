using EFCoreExtend.Sql.SqlConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.Default
{
    public class LuaSqlPolicyExecutorInfo : ILuaSqlPolicyExecutorInfo
        , ILuaSqlPreExecutePolicyExecutorInfo, ILuaSqlExecutePolicyExecutorInfo
    {

        public ConfigSqlExecuteType ExecuteType { get; set; }

        public ILuaConfigSqlInfo SqlInfo { get; set; }

        public ILuaConfigTableInfo TableInfo { get; set; }

        public DbContext DB { get; set; }

        public object GlobalPolicy { get; set; }

        public bool IsEnd { get; set; }

        public object ParameterPolicy { get; set; }

        public string PolicyName { get; set; }

        public object ReturnValue { get; set; }
        
        public Type ReturnType { get; set; }

        public string Sql { get; set; }

        public string SqlName { get; set; }

        public string TableName { get; set; }

        public ILuaSqlConfig Config { get; set; }

        public Func<object> ToDBExecutor { get; set; }

        public IDataParameter[] SqlParams { get; set; }
        IReadOnlyList<IDataParameter> ILuaSqlExecutePolicyExecutorInfo.SqlParams => SqlParams;

        public IDictionary<string, object> PreSqlParams { get; set; }

        public IDictionary<string, object> LuaSqlParamFuncs { get; set; }

        public Action LuaRan { get; set; }

        public object GetPolicy(ILuaConfigSqlInfo sqlInfo, ILuaConfigTableInfo tableInfo)
        {
            object tempPolicy = null;
            if (ParameterPolicy != null)
            {
                return ParameterPolicy;
            }
            else if (sqlInfo?.Policies?.TryGetValue(PolicyName, out tempPolicy) == true)
            {
                return tempPolicy;
            }
            else if (tableInfo?.Policies?.TryGetValue(PolicyName, out tempPolicy) == true)
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

    public class LuaSqlInitPolicyExecutorInfo : ILuaSqlPolicyExecutorInfo, ILuaSqlInitPolicyExecutorInfo
    {
        public ILuaSqlConfig Config { get; set; }

        public object GlobalPolicy { get; set; }

        public object ParameterPolicy { get; set; }

        public string PolicyName { get; set; }

        public IDictionary<string, IDictionary<string, object>> LuaSqlParamFuncsContainer { get; set; }

        public object GetPolicy(ILuaConfigSqlInfo sqlInfo, ILuaConfigTableInfo tableInfo)
        {
            object tempPolicy = null;
            if (ParameterPolicy != null)
            {
                return ParameterPolicy;
            }
            else if (sqlInfo?.Policies?.TryGetValue(PolicyName, out tempPolicy) == true)
            {
                return tempPolicy;
            }
            else if (tableInfo?.Policies?.TryGetValue(PolicyName, out tempPolicy) == true)
            {
                return tempPolicy;
            }
            else
            {
                return GlobalPolicy;
            }
        }

    }
}
