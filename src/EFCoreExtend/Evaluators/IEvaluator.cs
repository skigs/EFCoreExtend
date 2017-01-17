using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Evaluators
{
    /// <summary>
    /// 用于将Expression中的变量替换成值，用于生成IQueryable的CacheKey
    /// </summary>
    public interface IEvaluator
    {
        Expression PartialEval(Expression expression);
    }
}
