using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors
{
    /// <summary>
    /// 用于初始化的策略执行器，就是在程序运行期间只执行一次，除非sql的配置数据发生了改变（例如：替换表名 / 合并分部sql等的执行器）
    /// </summary>
    public interface ISqlInitPolicyExecutor : ISqlPolicyExecutor<ISqlInitPolicyExecutorInfo>
    {
    }

    public interface ISqlInitPolicyExecutorInfo : IPolicyExecutorInfoBase
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        string PolicyName { get; }

        /// <summary>
        /// 保存sqls（key一为表名，key二为sql名，值为sql，这个属性用于保存 初始化策略执行器生成的sql (表名替换/合并分部sql等等所生成的sql)）
        /// </summary>
        IReadOnlyDictionary<string, IDictionary<string, string>> NewlySqls { get; }

        /// <summary>
        /// 获取策略对象（策略对象可能 通过方法传递的policy / 全局的policy / SqlInfo和TableInfo配置的policy中获取，如果这些地方都了相同类型的policy
        /// 那么获取策略对象优先级是：通过方法传递的policy > SqlInfo配置的 > TableInfo配置的 > GlobalPolicy）
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        object GetPolicy(IConfigSqlInfo sqlInfo, IConfigTableInfo tableInfo);
    }

}
