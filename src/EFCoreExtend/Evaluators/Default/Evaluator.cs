using EFCoreExtend.ExpressionParsers;
using EFCoreExtend.ExpressionParsers.Default;
using EFCoreExtend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Evaluators.Default
{
    /// <summary>
    /// 将Expression中的变量替换成值
    /// </summary>
    public class Evaluator : IEvaluator
    {
        readonly IEFExpressionParser _expParser = new EFExpressionParser();

        public Evaluator()
        {
        }

        public Evaluator(IEFExpressionParser expParser)
        {
            expParser.CheckNull(nameof(expParser));

            _expParser = expParser;
        }

        public Expression PartialEval(Expression expression)
        {
            var sub = new SubEvaluator(_expParser, new Nominator().Nominate(expression));
            return sub.Visit(expression);
        }

        class SubEvaluator : ExpressionVisitor
        {
            HashSet<Expression> _candidates;
            readonly IEFExpressionParser _expParser;
            internal SubEvaluator(IEFExpressionParser expParser, HashSet<Expression> candidates)
            {
                _candidates = candidates;
                _expParser = expParser;
            }

            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (_candidates.Contains(exp))
                {
                    return this.Evaluate(exp);
                }
                return base.Visit(exp);
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                if (node.NewExpression.NodeType == ExpressionType.New)
                    return node;

                return base.VisitMemberInit(node);
            }

            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }

                return _expParser.EvalExpression(e);
            }

        }
    }

}
