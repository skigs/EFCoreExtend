using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.ExpressionParsers.Default.MethodParsers
{
    interface IMethodParser
    {
        bool TryParse(MethodInfo method, object obj, IReadOnlyList<Expression> args, out object outVal);
    }
}
