using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers.Default
{
    class ConstantExpOrStaticParser : ExpressionParserBase
    {
        public override bool TryParse(Expression e, out object val, MemberInfo memberInfo = null,
            IReadOnlyList<Expression> args = null)
        {
            return TryGetConstantExpOrStaticVal(e, memberInfo, args, out val);
        }

    }
}
