using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class TestID
    {
        public Guid id { get; set; }
        //public string id { get; set; }    //使用string存储GUID
        public string name { get; set; }
    }
}
