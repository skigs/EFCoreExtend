using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace EFCoreExtend.Sql.Default
{
    public abstract class SqlExecutorBase : ISqlExecutor
    {
        IObjectReflector _objReflec;
        public SqlExecutorBase(IObjectReflector objReflec)
        {
            objReflec.CheckNull(nameof(objReflec));

            _objReflec = objReflec;
        }

        public IReadOnlyList<T> Query<T>(DbContext db, string sql, IDataParameter[] parameters = null,
            IEnumerable<string> ignoreProptsForRtnType = null)
            where T : new()
        {
            var concurrencyDetector = db.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                var reader = GetReader(db, sql, parameters);
                var rtnList = new List<T>();
                T model;
                object val;
                using (reader.DbDataReader)
                {
                    var propts = _objReflec.GetPublicInstancePropts(typeof(T), ignoreProptsForRtnType);
                    while (reader.DbDataReader.Read())
                    {
                        model = new T();
                        foreach (var l in propts)
                        {
                            val = reader.DbDataReader[l.Name];
                            val = ChangeType(l.PropertyType, val);
                            l.SetValue(model, val);
                        }
                        rtnList.Add(model);
                    }
                }
                return rtnList;
            }
        }

        /// <summary>
        /// 值的类型转换
        /// </summary>
        /// <param name="proptType"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        protected abstract object ChangeType(Type proptType, object val);

        protected RelationalDataReader GetReader(DbContext db, string sql, IDataParameter[] parameters)
        {
            if (parameters?.Length > 0)
            {
                //带参数的
                var cmd = db.GetService<IRawSqlCommandBuilder>()
                    .Build(sql, parameters);
                return cmd
                    .RelationalCommand
                    .ExecuteReader(
                        db.GetService<IRelationalConnection>(),
                        parameterValues: cmd.ParameterValues);
            }
            else
            {
                //不带参数的
                var cmd = db.GetService<IRawSqlCommandBuilder>()
                    .Build(sql);
                return cmd
                    .ExecuteReader(db.GetService<IRelationalConnection>());
            }
        }

        public object Scalar(DbContext db, string sql, params IDataParameter[] parameters)
        {
            var concurrencyDetector = db.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                if (parameters?.Length > 0)
                {
                    //带参数的
                    var cmd = db.GetService<IRawSqlCommandBuilder>()
                                .Build(sql, parameters);
                    return cmd
                        .RelationalCommand
                        .ExecuteScalar(
                            db.GetService<IRelationalConnection>(),
                            parameterValues: cmd.ParameterValues);
                }
                else
                {
                    //不带参数的
                    var cmd = db.GetService<IRawSqlCommandBuilder>()
                                .Build(sql);
                    return cmd
                        .ExecuteScalar(db.GetService<IRelationalConnection>());
                }
            }
        }

        public int NonQuery(DbContext db, string sql, params IDataParameter[] parameters)
        {
            if (parameters?.Length > 0)
            {
                return db.Database.ExecuteSqlCommand(sql, parameters);
            }
            else
            {
                return db.Database.ExecuteSqlCommand(sql);
            }
        }

    }
}
