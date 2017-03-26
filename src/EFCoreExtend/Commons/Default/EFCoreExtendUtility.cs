using EFCoreExtend.Sql;
using EFCoreExtend.Sql.SqlConfig.Policies;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Commons.Default
{
    /// <summary>
    /// EFCoreExtend帮助工具类
    /// </summary>
    public class EFCoreExtendUtility : IEFCoreExtendUtility
    {
        #region GetTableName
        protected readonly IDictionary<Type, TableAttribute> _dictTableAttr = new ConcurrentDictionary<Type, TableAttribute>();

        public string GetTableName(Type type, bool isDisplayTypeName = true)
        {
            TableAttribute tAttr;
            if (!_dictTableAttr.TryGetValue(type, out tAttr))
            {
                tAttr = type.GetTypeInfo().GetCustomAttribute<TableAttribute>();
                _dictTableAttr[type] = tAttr;
            }
            if (tAttr == null)
            {
                if (isDisplayTypeName)
                {
                    return type.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return tAttr.Name;
            }
        }
        #endregion

        #region GetPolicyName
        protected readonly IDictionary<Type, SqlConfigPolicyAttribute> _dictSqlPolicyAttr = new ConcurrentDictionary<Type, SqlConfigPolicyAttribute>();
        public string GetSqlConfigPolicyName(Type type, bool isDisplayTypeName = false)
        {
            SqlConfigPolicyAttribute attr = null;
            if (!_dictSqlPolicyAttr.TryGetValue(type, out attr))
            {
                attr = type.GetTypeInfo().GetCustomAttribute(typeof(SqlConfigPolicyAttribute), false) as SqlConfigPolicyAttribute;
                _dictSqlPolicyAttr[type] = attr;
            }
            if (attr == null)
            {
                if (isDisplayTypeName)
                {
                    return type.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return attr.Name;
            }
        }
        #endregion

        #region Combine
        protected readonly string _splitChar = ",";
        protected readonly string _splitSqlAndParamsChar = ";";
        public string CombineSqlAndParamsToString(string sql, IEnumerable<KeyValuePair<string, object>> parameters, 
            bool toMD5 = true)
        {
            //不使用Any()进行判断，而是参数传递Count属性，然后扩展方法进行相应的扩展，提高些许性能
            if (parameters.HasValueE())
            {
                StringBuilder sb = new StringBuilder();
                var pairs = parameters.OrderBy(l => l.Key);
                bool bSplit = false;
                foreach (var pair in pairs)
                {
                    if (bSplit)
                    {
                        sb.Append(_splitChar);
                    }
                    else
                    {
                        bSplit = true;
                    }
                    sb.Append(pair.Key + "=" + pair.Value);
                }

                return StringToMD5(sql + _splitSqlAndParamsChar + sb.ToString(), toMD5);
            }
            else
            {
                return StringToMD5(sql, toMD5);
            }
        }

        public string CombineSqlAndParamsToString(string sql, IEnumerable<IDataParameter> parameters, 
            bool toMD5 = true)
        {
            //不使用Any()进行判断，而是参数传递Count属性，然后扩展方法进行相应的扩展，提高些许性能
            if (parameters.HasValueE())
            {
                StringBuilder sb = new StringBuilder();
                var pairs = parameters.OrderBy(l => l.ParameterName);

                bool bSplit = false;
                foreach (var pair in pairs)
                {
                    if (bSplit)
                    {
                        sb.Append(_splitChar);
                    }
                    else
                    {
                        bSplit = true;
                    }
                    sb.Append(pair.ParameterName + "=" + pair.Value);
                }

                return StringToMD5(sql + _splitSqlAndParamsChar + sb.ToString(), toMD5);
            }
            else
            {
                return StringToMD5(sql, toMD5);
            }
        }

        protected static string StringToMD5(string val, bool toMD5)
        {
            if (toMD5)
            {
                return val.ToMD5();
            }
            else
            {
                return val;
            }
        }

        public IDataParameter[] CombineDataParams(IDataParameter[] params1, IDataParameter[] params2)
        {
            if (params1?.Length > 0)
            {
                if (params2?.Length > 0)
                {
                    var tempArray = new IDataParameter[params1.Length + params2.Length];
                    params1.CopyTo(tempArray, 0);
                    params2.CopyTo(tempArray, params1.Length);
                    return tempArray;
                }
                else
                {
                    return params1;
                }
            }
            else
            {
                return params2;
            }
        }
        #endregion

        #region Enum
        protected readonly IDictionary<Enum, DescriptionAttribute> _dictEnumDeciAttr = new ConcurrentDictionary<Enum, DescriptionAttribute>();

        /// <summary>
        /// 获取枚举项描述信息(DescriptionAttribute)
        /// </summary>
        /// <param name="enumVal">枚举类项</param>        
        /// <param name="isDisplayOriginalName">是否显示原始名称(如果没有DescriptionAttribute描述信息的时候)</param>
        /// <returns>描述信息</returns>
        public string GetEnumDescription(Enum enumVal, bool isDisplayOriginalName = false)
        {
            DescriptionAttribute attr = null;
            if (!_dictEnumDeciAttr.TryGetValue(enumVal, out attr))
            {
                string strValue = enumVal.ToString();
                var fieldinfo = enumVal.GetType().GetField(strValue);
                if (fieldinfo != null)
                {
                    attr = fieldinfo.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    _dictEnumDeciAttr[enumVal] = attr;
                }
                else
                {
                    _dictEnumDeciAttr[enumVal] = null;
                }
            }
            if (attr == null)
            {
                if (isDisplayOriginalName)
                {
                    return enumVal.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return attr.Description;
            }
        }
        #endregion

    }
}
