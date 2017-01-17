using EFCoreExtend.Commons;
using EFCoreExtend.Evaluators.Default.Printer;
using EFCoreExtend.Evaluators.Default.Printer.Default;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers.Default
{
    /// <summary>
    /// EF的Expression的解析器
    /// </summary>
    public class EFExpressionParser : IEFExpressionParser
    {
        protected readonly IExpressionParser _expParser = new ExpressionParser();
        protected readonly IEnumerablePrinter _printer = new EnumerablePrinter();

        public EFExpressionParser()
        {
        }

        public EFExpressionParser(IExpressionParser expParser, IEnumerablePrinter printer)
        {
            expParser.CheckNull(nameof(expParser));
            printer.CheckNull(nameof(printer));

            _expParser = expParser;
            _printer = printer;
        }

        public Expression EvalExpression(Expression e)
        {
            object val;
            _expParser.TryParse(e, out val);

            //如果e为NewArrayExpression的不需要进行List转换Printer进行打印，否则抛异常
            if (!(e is NewArrayExpression))
            {
                //打印List
                object listVal;
                if (TryObjToListPrinter(val, out listVal))
                {
                    return Expression.Constant(listVal);
                }
            }

            return Expression.Constant(val, e.Type);
        }

        public IReadOnlyDictionary<string, object> MemberInitExpression2Dictionary(Expression e)
        {
            var dict = new Dictionary<string, object>();
            if (e is MemberInitExpression)
            {
                var miExp = e as MemberInitExpression;

                if (miExp.Bindings != null && miExp.Bindings.Count > 0)
                {
                    MemberAssignment assim = null;
                    object val;
                    foreach (var b in miExp.Bindings)
                    {
                        assim = b as MemberAssignment;
                        if (assim != null)
                        {
                            _expParser.TryParse(assim.Expression, out val, assim.Member);
                            dict[assim.Member.Name] = val;
                        }
                    }
                }
            }
            return dict;
        }

        public bool TryParse(Expression e, out object val, MemberInfo memberInfo = null, IReadOnlyList<Expression> args = null)
        {
            return _expParser.TryParse(e, out val, memberInfo, args);
        }

        protected virtual bool TryObjToListPrinter(object obj, out object listVal)
        {
            listVal = null;
            var list = obj as IEnumerable;
            if (list != null)
            {
                var tList = list.GetType();
                //数组的
                var elementType = tList.GetElementType();
                if (elementType == null)
                {
                    //泛型的
                    var argTypes = tList.GetGenericArguments();
                    if (argTypes?.Length > 0)
                    {
                        elementType = argTypes[0];
                    }
                }

                if (elementType != null)
                {
                    return _printer.TryToPrinter(list, elementType, out listVal);
                }
            }
            return false;
        }

    }

}
