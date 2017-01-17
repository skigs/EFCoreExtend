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
    public class SqlExecutor : SqlExecutorBase
    {
        public SqlExecutor(IObjectReflector objReflec)
            : base(objReflec)
        {
        }

        static Type _tString = typeof(string);
        protected override object ChangeType(Type proptType, object val)
        {
            if (val == DBNull.Value || val == null)
            {
                return null;
            }
            else
            {
                if (proptType.TryChangeValueType(val, out val))
                {
                    return val;
                }
                else if (proptType == _tString)
                {
                    if (val.GetType() != _tString)
                    {
                        return val.ToString();
                    }
                    else
                    {
                        return val;
                    }
                }
                else
                {
                    return val;
                }
            }
        }

    }
}
