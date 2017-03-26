using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public static class EFCoreExtendUtilityExtensions
    {
        /// <summary>
        /// 获取策略名称(SqlConfigPolicyAttribute)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isDisplayTypeName"></param>
        /// <returns></returns>
        public static string GetSqlConfigPolicyName<T>(this IEFCoreExtendUtility util, bool isDisplayTypeName = false)
        {
            return util.GetSqlConfigPolicyName(typeof(T), isDisplayTypeName);
        }

    }
}
