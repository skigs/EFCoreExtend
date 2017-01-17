using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers.Default.MethodParsers
{
    class MethodParser : IMethodParser
    {
        protected readonly ExpressionParserBase _parser;
        public ExpressionParserBase Parser
        {
            get { return _parser; }
        }

        public MethodParser(ExpressionParserBase parser)
        {
            parser.CheckNull(nameof(parser));

            _parser = parser;
        }

        public bool TryParserArrayArg(NewArrayExpression arg, out object outVal)
        {
            //outVal = null;
            //if (arg != null && arg.Expressions != null)
            //{
            //    Expression tempExp = null;
            //    object val = null;
            //    var valArray = Array.CreateInstance(Type.GetType(arg.Type.FullName.Substring(0, arg.Type.FullName.Length - 2)), 
            //        arg.Expressions.Count);
            //    for (int j = 0; j < arg.Expressions.Count; j++)
            //    {
            //        tempExp = arg.Expressions[j];

            //        if (!Parser.TryGetMemberExpOrConstExpVal(tempExp, null, null, out val))
            //        {
            //            return false;
            //        }

            //        valArray.SetValue(val, j);
            //    }
            //    outVal = valArray;
            //    return true;
            //}
            //return false;

            return Parser.TryGetNewArrayVal(arg, out outVal);
        }

        public bool TryParserObjectArg(Expression arg, out object outVal)
        {
            return Parser.TryGetMemberExpOrConstExpVal(arg, null, null, out outVal);
        }

        public bool TryParse(System.Reflection.MethodInfo method, object obj,
            IReadOnlyList<Expression> args, out object outVal)
        {
            outVal = null;
            object[] objArray = null;
            if (args != null && args.Count > 0)
            {
                objArray = new object[args.Count];
                object tempVal = null;
                Expression arg = null;

                for (int i = 0; i < args.Count; i++)
                {
                    arg = args[i];

                    if (arg is NewArrayExpression)
                    {
                        if (!TryParserArrayArg(arg as NewArrayExpression, out tempVal))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!TryParserObjectArg(arg, out tempVal))
                        {
                            return false;
                        }
                    } 

                    objArray[i] = tempVal;
                }
            }
            outVal = method.Invoke(obj, objArray);
            return true;
        }
    }

}
