using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Interview.Web.Models;

namespace Interview.Web.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext (DbContextOptions<InventoryContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
