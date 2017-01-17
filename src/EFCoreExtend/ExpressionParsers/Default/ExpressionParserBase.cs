using EFCoreExtend.ExpressionParsers.Default.MethodParsers;
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
    abstract class ExpressionParserBase : IExpressionParser
    {
        IMethodParser _methodParser;
        public ExpressionParserBase()
        {
            _methodParser = new MethodParser(this);
        }

        public abstract bool TryParse(Expression e, out object val, MemberInfo memberInfo = null, IReadOnlyList<Expression> args = null);

        public bool TryGetNewArrayVal(NewArrayExpression arg, out object outVal)
        {
            outVal = null;
            if (arg != null && arg.Expressions != null)
            {
                Expression tempExp = null;
                object val = null;
                var valArray = Array.CreateInstance(Type.GetType(arg.Type.FullName.Substring(0, arg.Type.FullName.Length - 2)),
                    arg.Expressions.Count);
                for (int j = 0; j < arg.Expressions.Count; j++)
                {
                    tempExp = arg.Expressions[j];

                    if (!TryGetMemberExpOrConstExpVal(tempExp, null, null, out val))
                    {
                        return false;
                    }

                    valArray.SetValue(val, j);
                }
                outVal = valArray;
                return true;
            }
            return false;
        }

        protected bool TryMethodInvoke(MethodInfo method, object obj, IReadOnlyList<Expression> args, out object outVal)
        {
            return _methodParser.TryParse(method, obj, args, out outVal);
        }

        public bool TryGetMemberExpOrConstExpVal(Expression exp, MemberInfo member, IReadOnlyList<Expression> args, 
            out object val)
        {
            if (!TryGetConstantExpOrStaticVal(exp, member, args, out val))
            {
                if (!TryGetMemberExpVal(exp, out val))
                {
                    return false;
                }
            }
            return true;
        }

        protected bool TryGetMemberExpVal(Expression exp, out object val)
        {
            val = null;
            if (exp is MemberExpression)
            {
                var node = exp as MemberExpression;
                if (node.Expression is MemberExpression)
                {
                    MemberExpression lastMemberExp;
                    List<MemberExpression> listMemberExp;
                    var constExp = MemberExpToConstantExp(node.Expression, out lastMemberExp, out listMemberExp);
                    object constVal = null;
                    if (TryGetConstantExpOrStaticVal(constExp, lastMemberExp.Member, null, out constVal))
                    {
                        if (listMemberExp != null && listMemberExp.Count > 0)
                        {
                            for (int i = listMemberExp.Count - 1; i >= 0; i--)
                            {
                                TryGetMemberVal(listMemberExp[i].Member, constVal, null, out constVal);
                            }
                            return TryGetMemberVal(node.Member, constVal, null, out val);
                        }
                        else
                        {
                            return TryGetMemberVal(node.Member, constVal, null, out val);
                        }
                    }
                }
                else if (node.Expression is ConstantExpression)
                {
                    return TryGetConstantExpOrStaticVal(node.Expression, node.Member, null, out val);
                }
                else if (node.Expression == null)
                {
                    //获取静态的成员数据
                    return TryGetConstantExpOrStaticVal(node.Expression, node.Member, null, out val);
                }
            }
            return false;
        }

        /// <summary>
        /// 获取最后的MemberExpression和ConstantExpression，和Expression所链接的所有MemberExpression
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="lastMemberExp"></param>
        /// <param name="listMemberExp"></param>
        /// <returns></returns>
        protected Expression MemberExpToConstantExp(Expression exp, out MemberExpression lastMemberExp, out List<MemberExpression> listMemberExp)
        {
            lastMemberExp = null;
            listMemberExp = new List<MemberExpression>();
            while (exp is MemberExpression)
            {
                if (lastMemberExp != null)
                {
                    listMemberExp.Add(lastMemberExp);
                }
                lastMemberExp = exp as MemberExpression;
                exp = lastMemberExp.Expression;
            }
            return exp;
        }

        protected bool TryGetConstantExpOrStaticVal(Expression exp, MemberInfo member, IReadOnlyList<Expression> args, out object val)
        {
            val = null;
            if (exp is ConstantExpression)
            {
                val = (exp as ConstantExpression).Value;

                if (args != null && member != null)
                {
                    return TryGetMemberVal(member, val, args, out val);
                }
                else
                {
                    if (val is ValueType || val is string || val == null)
                    {
                        return true;
                    }

                    if (member != null)
                    {
                        return TryGetMemberVal(member, val, args, out val);
                    }
                }

            }
            else if (exp == null)
            {
                //获取静态的成员数据
                return TryGetMemberVal(member, member.Name, args, out val);
            }

            return false;
        }

        protected bool TryGetMemberVal(MemberInfo member, object obj, IReadOnlyList<Expression> args, out object val)
        {
            val = null;
            if (obj != null)
            {
                if (member is PropertyInfo)
                {
                    val = (member as PropertyInfo).GetValue(obj);
                    return true;
                }
                else if (member is FieldInfo)
                {
                    val = (member as FieldInfo).GetValue(obj);
                    return true;
                }
                else if (member is MethodInfo)
                {
                    return TryMethodInvoke(member as MethodInfo, obj, args, out val);
                }
            }
            return false;
        }

    }
}
