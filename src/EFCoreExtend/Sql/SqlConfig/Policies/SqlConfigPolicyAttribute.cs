using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SqlConfigPolicyAttribute : Attribute
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">策略名称</param>
        public SqlConfigPolicyAttribute(string name)
        {
            Name = name;
        }
    }
}
