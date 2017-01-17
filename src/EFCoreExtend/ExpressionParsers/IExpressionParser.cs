using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers
{
    /// <summary>
    /// Expression解析器
    /// </summary>
    public interface IExpressionParser
    {
        /// <summary>
        /// 解析传进的Expression并获取相关的值，用于ExpressionVisitor的Visit中（是针对DynamicInvoke的一系列优化）
        /// </summary>
        /// <param name="e"></param>
        /// <param name="val"></param>
        /// <param name="memberInfo"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool TryParse(Expression e, out object val, MemberInfo memberInfo = null,
            IReadOnlyList<Expression> args = null);
    }
}
