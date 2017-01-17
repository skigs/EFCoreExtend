using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    /// <summary>
    /// Sql配置文件的格式类型
    /// </summary>
    public enum SqlConfigFileExtType
    {
        /// <summary>
        /// 所有后缀格式文件
        /// </summary>
        all = 0,
        /// <summary>
        /// json后缀格式文件(xxx.json)
        /// </summary>
        [Description(".json")]
        json = 0x2,
        /// <summary>
        /// txt后缀格式文件(xxx.txt)
        /// </summary>
        [Description(".txt")]
        txt = 0x4,
    }
}
