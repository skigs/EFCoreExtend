using EFCoreExtend.Commons;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Evaluators.Default.Printer.Default
{
    public class ListPrinter : IEnumerablePrinter
    {
        protected readonly static Type _tPrinterList = typeof(PrinterList<>);
        public bool TryToPrinter(IEnumerable collection, Type elementType, out object printer)
        {
            var printerType = _tPrinterList.MakeGenericType(elementType);
            printer = Activator.CreateInstance(printerType, collection);
            return true;
        }
    }

    /// <summary>
    /// 使List.ToString()的时候把内容打印出来
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class PrinterList<T> : List<T>
    {
        public PrinterList(IEnumerable collection)
            : base(collection.Cast<T>())
        {
        }

        public override string ToString()
        {
            if (typeof(T).IsValueOrStringType())
            {
                return "[" + this.JoinToString(",") + "]";
            }
            else
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }
}
