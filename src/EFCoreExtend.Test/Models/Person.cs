using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    [Table(nameof(Person))]
    public class Person
    {
        public int id { get; set; }
        public string name { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? birthday { get; set; }
        public int? addrid { get; set; }
    }
}
