using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers.Default
{
    class NewArrayExpParser : ExpressionParserBase
    {
        public override bool TryParse(Expression e, out object val, System.Reflection.MemberInfo memberInfo = null,
            IReadOnlyList<Expression> args = null)
        {
            if (e is NewArrayExpression)
            {
                return TryGetNewArrayVal(e as NewArrayExpression, out val);
            }
            val = null;
            return false;
        }
    }
}
