using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers
{
    /// <summary>
    /// 用于将Expression中的变量替换成值，用于生成IQueryable的CacheKey
    /// </summary>
    public interface IEFExpressionParser : IExpressionParser
    {
        /// <summary>
        /// 用于Evaluator中
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        Expression EvalExpression(Expression e);

        /// <summary>
        /// 用于将Update的Expression生成UpdateSetModel
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        IReadOnlyDictionary<string, object> MemberInitExpression2Dictionary(Expression e);
    }
}
