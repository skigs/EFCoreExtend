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
    /// Performs bottom-up analysis to determine which nodes can possibly
    /// be part of an evaluated sub-tree.
    /// </summary>
    internal class Nominator : ExpressionVisitor
    {
        static readonly Func<Expression, bool> _canBeEvaluated;
        static readonly Type _tIQueryable;
        static Nominator()
        {
            _tIQueryable = typeof(IQueryable);
            _canBeEvaluated = expression =>
            {
                if (expression.NodeType == ExpressionType.Parameter)
                {
                    return false;
                }

                //if (_tIQueryable.GetTypeInfo().IsAssignableFrom(expression.Type))
                if (_tIQueryable.IsAssignableFrom(expression.Type))
                {
                    return false;
                }
                return true;
            };
        }

        Func<Expression, bool> fnCanBeEvaluated;
        HashSet<Expression> candidates;
        bool cannotBeEvaluated;

        internal Nominator()
        {
            this.fnCanBeEvaluated = _canBeEvaluated;
        }

        internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
        {
            this.fnCanBeEvaluated = fnCanBeEvaluated;
        }

        internal HashSet<Expression> Nominate(Expression expression)
        {
            this.candidates = new HashSet<Expression>();
            this.Visit(expression);
            return this.candidates;
        }

        public override Expression Visit(Expression expression)
        {
            if (expression != null)
            {
                bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                this.cannotBeEvaluated = false;
                base.Visit(expression);
                if (!this.cannotBeEvaluated)
                {
                    if (this.fnCanBeEvaluated(expression))
                    {
                        this.candidates.Add(expression);
                    }
                    else
                    {
                        this.cannotBeEvaluated = true;
                    }
                }
                this.cannotBeEvaluated |= saveCannotBeEvaluated;
            }
            return expression;
        }
    }
}
