using EFCoreExtend.Commons;
using EFCoreExtend.ExpressionParsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers.Default
{
    public class ExpressionParser : IExpressionParser
    {
        readonly static Type _tNullable = typeof(Nullable<>);

        readonly MethodCallExpParser _methodCallParser = new MethodCallExpParser();

        public IExpressionParser MethodCallParser
        {
            get { return _methodCallParser; }
        }

        readonly MemberExpParser _memberParser = new MemberExpParser();

        public IExpressionParser MemberParser
        {
            get { return _memberParser; }
        }

        readonly ConstantExpOrStaticParser _constParser = new ConstantExpOrStaticParser();

        public IExpressionParser ConstParser
        {
            get { return _constParser; }
        }

        readonly ExpressionDynamicParser _otherParser = new ExpressionDynamicParser();

        public IExpressionParser OtherParser
        {
            get { return _otherParser; }
        }

        readonly UnaryExpParser _unaryParser = new UnaryExpParser();

        public IExpressionParser UnaryParser
        {
            get { return _unaryParser; }
        }

        readonly NewArrayExpParser _newArrayParser = new NewArrayExpParser();

        public IExpressionParser NewArrayParser
        {
            get { return _newArrayParser; }
        }

        public bool TryParse(Expression e, out object val, 
            MemberInfo memberInfo = null,
            IReadOnlyList<Expression> args = null)
        {
            bool bOther = false;
            if (MethodCallParser.TryParse(e, out val, memberInfo, args))
            {
            }
            else if (MemberParser.TryParse(e, out val, memberInfo, args))
            {
            }
            else if (ConstParser.TryParse(e, out val, memberInfo, args))
            {
            }
            else if (NewArrayParser.TryParse(e, out val, memberInfo, args))
            {
                //解析出来之后，显示的数据依然是：Trim(value(System.Char[]))
                //在EF的Where中是不支持l.name.Trim('_')这样的操作的，只能支持：l.name.Trim()
                //但是还是需要进行解析，不然会交由OtherParser（DynamicParser）进行解析了
            }
            else if (UnaryParser.TryParse(e, out val, memberInfo, args))
            {
            }
            else
            {
                bOther = true;
                OtherParser.TryParse(e, out val, memberInfo, args);
            }

            //不是OtherParser解析的，而且val不为null，判断val的类型与Expression.Type的类型是否一致
            if (!bOther && val != null)
            {
                val = GetRightfulValue(e, val);
            }

            return true;
        }

        object GetRightfulValue(Expression e, object val)
        {
            object tempVal;
            if (e.Type.TryChangeValueType(val, out tempVal))
            {
                return tempVal;
            }
            else
            {
                //如果val为IEnumerable那么不需要转换
                if (val is IEnumerable)
                {
                    return val;
                }
                OtherParser.TryParse(e, out val);
                return val;
            }
        }

    }
}
