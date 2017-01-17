using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Evaluators.Default.Printer
{
    /// <summary>
    /// 用于获取Enumerable的Printer，就是使用ToString的时候把值打印出来，用于Expression的Visit中
    /// </summary>
    public interface IEnumerablePrinter
    {
        bool TryToPrinter(IEnumerable collection, Type elementType, out object printer);
    }
}
