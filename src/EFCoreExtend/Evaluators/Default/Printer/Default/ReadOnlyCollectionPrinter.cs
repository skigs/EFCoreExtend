using EFCoreExtend.Commons;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Evaluators.Default.Printer.Default
{
    public class ReadOnlyCollectionPrinter : IEnumerablePrinter
    {
        protected readonly static Type _tPrinterReadOnlyCollection = typeof(PrinterReadOnlyCollection<>);
        public bool TryToPrinter(IEnumerable collection, Type elementType, out object printer)
        {
            var printerType = _tPrinterReadOnlyCollection.MakeGenericType(elementType);
            printer = Activator.CreateInstance(printerType, collection);
            return true;
        }
    }

    class PrinterReadOnlyCollection<T> : ReadOnlyCollection<T>
    {
        public PrinterReadOnlyCollection(IEnumerable collection)
            : base(collection.Cast<T>().ToList())
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
