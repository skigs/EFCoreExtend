using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    [Table(nameof(Address))]
    public class Address
    {
        public int id { get; set; }
        public string fullAddress { get; set; }
        public double? lat { get; set; }
        public double? lon { get; set; }
    }
}
