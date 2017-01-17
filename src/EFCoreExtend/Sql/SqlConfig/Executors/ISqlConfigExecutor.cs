using EFCoreExtend.Sql.SqlConfig.Policies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;

namespace EFCoreExtend.Sql.SqlConfig.Executors
{
    public interface ISqlConfigExecutor
    {
        DbContext DB { get; }
        string TableName { get; }
        string SqlName { get; }
        IConfigSqlInfo SqlInfo { get; }

        IReadOnlyList<T> Query<T>(IDataParameter[] parameters = null,
            IReadOnlyCollection<string> ignoreProptsForRtnType = null, 
            IReadOnlyDictionary<string, ISqlConfigPolicy> policies = null) where T : new();

        object Scalar(IDataParameter[] parameters = null, IReadOnlyDictionary<string, ISqlConfigPolicy> policies = null);

        int NonQuery(IDataParameter[] parameters = null, IReadOnlyDictionary<string, ISqlConfigPolicy> policies = null);

    }
}
