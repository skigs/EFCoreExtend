using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    public interface IDBConfigTable
    {
        DbContext DB { get; }
        string TableName { get; }
    }
}
