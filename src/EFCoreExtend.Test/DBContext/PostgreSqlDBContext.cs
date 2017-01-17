using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace EFCoreExtend.Test
{
    public class PostgreSqlDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseNpgsql(@"User ID=admin;Password=123456;Host=localhost;Port=5432;Database=TestDB;Pooling=true;");
            }
            base.OnConfiguring(optionsBuilder);
        }

        //PostgreSql创建的表与字段名最好都为小写，不然测试sql的时候会抛异常：未找到该表或字段。
        //（因为有些sql的表名和字段名都不一定带有 "" 标记的，标记了就大小写忽略，否则会区分大小写的）
        public DbSet<Person> Person { get; set; }
        public DbSet<Address> Address { get; set; }
    }
}
