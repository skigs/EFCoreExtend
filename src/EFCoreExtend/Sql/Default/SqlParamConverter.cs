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
        public IDataParameter[] DictionaryToDBParams(DbContext db, 
            IEnumerable<KeyValuePair<string, object>> dictParams, int? paramsCount)
        {
            IDataParameter[] sqlParams = null;
            if (paramsCount > 0)
            {
                //不使用Count()进行获取个数，而是参数传递count，然后扩展方法进行相应的扩展，从而提高性能
                sqlParams = new IDataParameter[paramsCount.Value];
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
