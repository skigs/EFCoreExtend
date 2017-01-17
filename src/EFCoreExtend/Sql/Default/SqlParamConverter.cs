using EFCoreExtend.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.Default
{
    public class SqlParamConverter : ISqlParamConverter
    {
        public IDataParameter[] DictionaryToDBParams(DbContext db, IReadOnlyDictionary<string, object> dictParams)
        {
            IDataParameter[] sqlParams = null;
            if (dictParams != null && dictParams.Count > 0)
            {
                sqlParams = new IDataParameter[dictParams.Count];
                int i = 0;
                using (var command = db.Database.GetDbConnection().CreateCommand())
                {
                    foreach (var p in dictParams)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = p.Key;
                        param.Value = p.Value ?? DBNull.Value;
                        sqlParams[i++] = param;
                    }
                }
            }
            return sqlParams ?? new IDataParameter[0];
        }

    }
}
