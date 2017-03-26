using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig
{
    public static class LuaSqlConfigConst
    {
        /// <summary>
        /// lua配置的全局配置对象的标签名
        /// </summary>
        public const string SqlConfigLabel = "cfg";
        /// <summary>
        /// lua配置的表配置的标签名
        /// </summary>
        public const string SqlConfigTablesLabel = "__tables";
        /// <summary>
        /// lua配置的获取表配置的标签名
        /// </summary>
        public const string SqlConfigGetTableLabel = "gettable";
        /// <summary>
        /// lua配置的配置sql表的标签名
        /// </summary>
        public const string SqlConfigConfigTableLabel = "table";
        /// <summary>
        /// lua配置的sqls函数标签名
        /// </summary>
        public const string SqlConfigSqlsLabel = "sqls";
        /// <summary>
        /// lua配置的布局页标签名
        /// </summary>
        public const string SqlConfigLayoutLabel = "layout";
        /// <summary>
        /// lua配置的表名标签名
        /// </summary>
        public const string SqlTableNameLabel = "name";
        /// <summary>
        /// lua配置的全局配置标签名
        /// </summary>
        public const string SqlConfigGlobalLabel = "global";
        /// <summary>
        /// lua配置的表策略配置标签名
        /// </summary>
        public const string TablePoliciesLabel = "tpolis";
        /// <summary>
        /// lua配置的sql策略配置标签名
        /// </summary>
        public const string SqlPoliciesLabel = "spolis";
        /// <summary>
        /// lua配置的SqlParameters(对所有SqlParameter的操作)相关操作的函数标签名
        /// </summary>
        public const string SqlParamsFuncLabel = "params";
        /// <summary>
        /// lua配置的SqlParameter(对指定的SqlParameter的操作)相关操作的函数标签名
        /// </summary>
        public const string SqlParamFuncLabel = "param";
        /// <summary>
        /// lua配置的SqlParameter相关遍历(foreach)操作的函数标签名
        /// </summary>
        public const string SqlParamForeachFuncLabel = "each";

        /// <summary>
        /// SqlParameter默认的分隔符
        /// </summary>
        public const string SqlParamDefaultSeparate = ",";
        /// <summary>
        /// ,
        /// </summary>
        public const string DefPairSeparate = ",";
        /// <summary>
        /// =
        /// </summary>
        public const string DefKeyValSeparate = "=";


        /// <summary>
        /// lua脚本文件的全局文件名
        /// </summary>
        public const string LuaGlobalFileLabel = "luaglobal";
    }
}
