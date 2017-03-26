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
        /// 将Dictionary => IDataParameter[] ( SqlParameters)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dictParams"></param>
        /// <param name="paramsCount">Params的个数</param>
        /// <returns></returns>
        IDataParameter[] DictionaryToDBParams(DbContext db, IEnumerable<KeyValuePair<string, object>> dictParams, int? paramsCount);
    }
}
