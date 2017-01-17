using EFCoreExtend.Evaluators.Default.Printer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;

namespace EFCoreExtend.Evaluators.Default.Printer.Default
{
    /// <summary>
    /// 用于获取Enumerable的Printer，就是使用ToString的时候把值打印出来，用于Expression的Visit中
    /// </summary>
    public class EnumerablePrinter : IEnumerablePrinter
    {
        protected readonly static Type _tList = typeof(List<>);
        protected readonly static Type _tLinkedList = typeof(LinkedList<>);
        protected readonly static Type _tReadOnlyCollection = typeof(ReadOnlyCollection<>);

        protected readonly ListPrinter _listPrinter = new ListPrinter();
        protected readonly LinkedListPrinter _linkedListPrinter = new LinkedListPrinter();
        protected readonly ReadOnlyCollectionPrinter _readOnlyCollectionPrinter = new ReadOnlyCollectionPrinter();

        public bool TryToPrinter(IEnumerable collection, Type elementType, out object printer)
        {
            printer = null;
            var typeCollec = collection.GetType();

            if (typeCollec.Name == _tList.Name)
            {
                return _listPrinter.TryToPrinter(collection, elementType, out printer);
            }
            else if (typeCollec.Name == _tLinkedList.Name)
            {
                return _linkedListPrinter.TryToPrinter(collection, elementType, out printer);
            }
            else if (typeCollec.Name == _tReadOnlyCollection.Name)
            {
                return _readOnlyCollectionPrinter.TryToPrinter(collection, elementType, out printer);
            }

            //return false;
            //其他的类型和List使用一样的Printer
            return _listPrinter.TryToPrinter(collection, elementType, out printer);
        }

    }
}
