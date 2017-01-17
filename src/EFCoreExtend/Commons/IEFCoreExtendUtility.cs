using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Commons
{
    /// <summary>
    /// EFCoreExtend帮助工具类
    /// </summary>
    public interface IEFCoreExtendUtility
    {
        /// <summary>
        /// 获取类型的表名(TableAttribute)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isDisplayTypeName">如果没有设置TableAttribute，是否需要返回类型名称</param>
        /// <returns></returns>
        string GetTableName(Type type, bool isDisplayTypeName = true);

        /// <summary>
        /// 获取策略名称(SqlConfigPolicyAttribute)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isDisplayTypeName"></param>
        /// <returns></returns>
        string GetSqlConfigPolicyName(Type type, bool isDisplayTypeName = false);

        /// <summary>
        /// 合并sql与SqlParameters
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="toMD5"></param>
        /// <returns></returns>
        string CombineSqlAndParamsToString(string sql, IReadOnlyDictionary<string, object> parameters, bool toMD5 = true);

        /// <summary>
        /// 合并sql与SqlParameters
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="toMD5"></param>
        /// <returns></returns>
        string CombineSqlAndParamsToString(string sql, IReadOnlyCollection<IDataParameter> parameters, bool toMD5 = true);

        /// <summary>
        /// 将两个SqlParameters数组合并
        /// </summary>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        /// <returns></returns>
        IDataParameter[] CombineDataParams(IDataParameter[] params1, IDataParameter[] params2);

        /// <summary>
        /// 获取枚举项描述信息(DescriptionAttribute)
        /// </summary>
        /// <param name="enumVal">枚举类项</param>        
        /// <param name="isDisplayOriginalName">是否显示原始名称(如果没有DescriptionAttribute描述信息的时候)</param>
        /// <returns></returns>
        string GetEnumDescription(Enum enumVal, bool isDisplayOriginalName = false);
    }
}
