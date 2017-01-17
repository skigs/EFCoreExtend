using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Executors
{
    public interface IPolicyExecutorInfo
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        string PolicyName { get; set; }

        /// <summary>
        /// 全局的策略对象
        /// </summary>
        object GlobalPolicy { get; set; }

        /// <summary>
        /// 通过方法形参传递的策略对象
        /// </summary>
        object ParameterPolicy { get; set; }
    }

    public interface IPolicyExecutorInfoBase
    {
        /// <summary>
        /// 所有的表的配置信息
        /// </summary>
        IReadOnlyDictionary<string, IConfigTableInfo> TableSqlInfos { get; }
        /// <summary>
        /// Info的临时数据（在策略执行器执行管道中传递，例如：类型为SqlInitPolicyExecutor的所有执行器执行的时候，可以用这个属性传递临时数据）
        /// </summary>
        IDictionary<string, object> TempDatas { get; }
    }

}
