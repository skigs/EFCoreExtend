using EFCoreExtend.Sql.SqlConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig
{
    /// <summary>
    /// lua sql配置
    /// </summary>
    public interface ILuaSqlConfig
    {
        /// <summary>
        /// 配置的Tables
        /// </summary>
        IReadOnlyDictionary<string, ILuaConfigTableInfo> TableSqlInfos { get; }

        /// <summary>
        /// lua sql配置发生了修改触发的事件
        /// </summary>
        event Action OnModified;

        /// <summary>
        /// 初始化lua sql配置
        /// </summary>
        void Init();

        /// <summary>
        /// 设置数据参数到lua的全局变量中
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool SetGlobalLuaParam(string parameterName, object parameter);

        /// <summary>
        /// 从lua全局变量中获取数据
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        IReadOnlyList<object> GetGlobalLuaParam(string parameterName);

        /// <summary>
        /// 设置数据参数到lua中，根据指定的表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameter"></param>
        bool SetLuaParam(string tableName, string parameterName, object parameter);

        /// <summary>
        /// 从lua参数中获取数据，根据指定的表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        IReadOnlyList<object> GetLuaParam(string tableName, string parameterName);

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="luascript">lua脚本</param>
        void Add(string tableName, string luascript);

        /// <summary>
        /// 移除配置
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        bool Remove(string tableName);

        /// <summary>
        /// 清空配置
        /// </summary>
        void Clear();

        /// <summary>
        /// 运行lua脚本中的函数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="sqlName">sql名称</param>
        /// <param name="luaFuncParameters">传递到lua函数中的参数</param>
        /// <returns></returns>
        LuaSqlConfigRunReturn Run(string tableName, string sqlName,
            object luaFuncParameters);

    }

    public class LuaSqlConfigRunReturn
    {
        public string Sql { get; set; }
        public ConfigSqlExecuteType Type { get; set; }
    }

}
