using Microsoft.EntityFrameworkCore;
using MySQL.Data.Entity.Extensions;
//using MySQL.Data.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Test
{
    public class MysqlDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseMySQL(@"Data Source=localhost;port=3306;Initial Catalog=testdb;user id=root;password=123456;");
            }
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Person> Person { get; set; }
        public DbSet<Address> Address { get; set; }
    }
}
