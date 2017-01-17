using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers.Default
{
    class UnaryExpParser : ExpressionParserBase
    {
        public override bool TryParse(Expression e, out object val, MemberInfo memberInfo = null,
            IReadOnlyList<Expression> args = null)
        {
            if (e is UnaryExpression)
            {
                return TryGetUnaryExpressionVal(e as UnaryExpression, out val); 
            }
            val = null;
            return false;
        }

        bool TryGetUnaryExpressionVal(UnaryExpression exp, out object val)
        {
            //可能出现的问题，exp.Operand又是UnaryExpression
            //UnaryExpression expUnary = exp;
            //List<UnaryExpression> listUnaryExp = new List<UnaryExpression>();
            //while (expUnary.Operand is UnaryExpression)
            //{
            //    expUnary = expUnary.Operand as UnaryExpression;
            //    listUnaryExp.Add(expUnary);
            //}

            if (TryGetMemberExpVal(exp.Operand, out val))
            {
                return true;
            }
            else
            {
                return TryGetConstantExpOrStaticVal(exp.Operand, null, null, out val);
            } 
        }

    }
}
