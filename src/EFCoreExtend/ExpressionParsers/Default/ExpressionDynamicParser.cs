using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers.Default
{
    class ExpressionDynamicParser : ExpressionParserBase
    {
        public override bool TryParse(Expression e, out object val, MemberInfo memberInfo = null,
            IReadOnlyList<Expression> args = null)
        {
            LambdaExpression lambda = Expression.Lambda(e);
            val = lambda.Compile().DynamicInvoke(null);
            return true;
        }
    }
}
