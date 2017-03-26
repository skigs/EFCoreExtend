using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Default
{
    public class LuaSqlModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// lua脚本中的sql表的配置
        /// </summary>
        public Table Config { get; set; }
        /// <summary>
        /// lua脚本中的sql函数集合
        /// </summary>
        public Table Sqls { get; set; }
    }
}
