using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers.Default
{
    class MethodCallExpParser : ExpressionParserBase
    {
        public override bool TryParse(Expression e, out object val, MemberInfo memberInfo = null,
            IReadOnlyList<Expression> args = null)
        {
            if (e is MethodCallExpression)
            {
                return TryDoToGetMethodCallVal(e as MethodCallExpression, out val); 
            }
            val = null;
            return false;
        }

        bool TryDoToGetMethodCallVal(MethodCallExpression expMethod, out object val)
        {
            //可能出现的情况expMethod.Object又是一个MethodCallExpression，例如：l.Serverid.StartsWith(model.Name.TrimEnd().TrimStart().ToLower())
            val = null;
            if (expMethod.Object is MethodCallExpression)
            {
                return TryParseMethodCall(expMethod, out val);
            }
            else if (expMethod.Object is ConstantExpression)
            {
                return TryGetConstantExpOrStaticVal(expMethod.Object, expMethod.Method, expMethod.Arguments, out val);
            }
            else
            {
                TryGetMemberExpVal(expMethod.Object, out val);
                return TryMethodInvoke(expMethod.Method, val, expMethod.Arguments, out val);
            }
        }

        bool TryParseMethodCall(MethodCallExpression expMethod, out object val)
        {
            val = null;
            bool bRtn = false;
            Expression tempExp = expMethod.Object;

            MethodCallExpression tempExpMethod;
            var listMemberExp = new List<MethodCallExpression>();
            while (tempExp is MethodCallExpression)
            {
                tempExpMethod = tempExp as MethodCallExpression;
                listMemberExp.Add(tempExpMethod);
                tempExp = tempExpMethod.Object;
            }

            //例如：Expression中的存在Method这么被调用了：Name.TrimEnd().TrimStart().ToLower()
            //进行第一个Method的调用(list中的索引是倒过来排序的，和stack一样): TrimEnd
            tempExpMethod = listMemberExp[listMemberExp.Count - 1];

            if (TryGetMemberExpOrConstExpVal(tempExpMethod.Object, tempExpMethod.Method, tempExpMethod.Arguments, out val))
            {
                if (TryMethodInvoke(tempExpMethod.Method, val, tempExpMethod.Arguments, out val))
                {
                    //循环调用其余的Method(): TrimStart
                    for (int i = listMemberExp.Count - 2; i >= 0; i--)
                    {
                        if (!TryMethodInvoke(listMemberExp[i].Method, val, listMemberExp[i].Arguments, out val))
                        {
                            return false;
                        }
                    }

                    //进行最后一个Method的调用: ToLower
                    bRtn = TryMethodInvoke(expMethod.Method, val, expMethod.Arguments, out val);
                }
            }

            return bRtn;
        }

    }
}
