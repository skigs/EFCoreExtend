using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql
{
    public interface ISqlParamConverter
    {
        /// <summary>
        /// 将Dictionary<string, object> => IDataParameter[] (SqlParameters)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dictParams"></param>
        /// <returns></returns>
        IDataParameter[] DictionaryToDBParams(DbContext db, IReadOnlyDictionary<string, object> dictParams);
    }
}
