using Microsoft.EntityFrameworkCore;
using Sparcpoint.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Contexts
{
    public class ProductContext : DbContext
    {
        public ProductContext() : base()
        {
        }

        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        { }

        public virtual DbSet<Product> Products { get; set; }

    }
}
