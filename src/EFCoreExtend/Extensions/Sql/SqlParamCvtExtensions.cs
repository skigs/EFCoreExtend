using EFCoreExtend.Sql;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class SqlParamCvtExtensions
    {
        /// <summary>
        /// 将sql的参数模型对象 转换成 sql的参数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="paramsModel">sql的参数模型对象</param>
        /// <param name="ignoreProptNames">sql的参数模型对象中需要忽略的属性名</param>
        /// <returns></returns>
        public static IDataParameter[] ObjectToDBParams(this ISqlParamConverter sqlParamCvt, DbContext db, 
            object paramsModel, IEnumerable<string> ignoreProptNames = null)
        {
            if (paramsModel != null)
            {
                return sqlParamCvt.DictionaryToDBParams(db, 
                    EFHelper.Services.ObjReflector.GetPublicInstanceProptValues(paramsModel, ignoreProptNames));
            }
            else
            {
                return new IDataParameter[0];
            }
        }

        /// <summary>
        /// 将Dictionary => IDataParameter[] ( SqlParameters)
        /// </summary>
        /// <param name="sqlParamCvt"></param>
        /// <param name="db"></param>
        /// <param name="dictParams"></param>
        /// <returns></returns>
        public static IDataParameter[] DictionaryToDBParams(this ISqlParamConverter sqlParamCvt, DbContext db, 
            IDictionary<string, object> dictParams)
        {
            return sqlParamCvt.DictionaryToDBParams(db, dictParams, dictParams?.Count);
        }

        /// <summary>
        /// 将Dictionary => IDataParameter[] ( SqlParameters)
        /// </summary>
        /// <param name="sqlParamCvt"></param>
        /// <param name="db"></param>
        /// <param name="dictParams"></param>
        /// <returns></returns>
        public static IDataParameter[] DictionaryToDBParamsR(this ISqlParamConverter sqlParamCvt, DbContext db, 
            IReadOnlyDictionary<string, object> dictParams)
        {
            return sqlParamCvt.DictionaryToDBParams(db, dictParams, dictParams?.Count);
        }

    }
}
