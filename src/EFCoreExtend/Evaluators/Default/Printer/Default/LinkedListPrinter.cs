using EFCoreExtend.Commons;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Evaluators.Default.Printer.Default
{
    public class LinkedListPrinter : IEnumerablePrinter
    {
        protected readonly static Type _tPrinterLinkedList = typeof(PrinterLinkedList<>);
        public bool TryToPrinter(IEnumerable collection, Type elementType, out object printer)
        {
            var printerType = _tPrinterLinkedList.MakeGenericType(elementType);
            printer = Activator.CreateInstance(printerType, collection);
            return true;
        }
    }

    public class PrinterLinkedList<T> : LinkedList<T>
    {
        public PrinterLinkedList(IEnumerable collection)
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
