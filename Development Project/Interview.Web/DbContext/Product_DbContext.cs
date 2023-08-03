using Microsoft.EntityFrameworkCore;
using Interview.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;
// AppDbContext.cs
public class Product_DbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Metadata> Metadata { get; set; }

    public Product_DbContext(DbContextOptions<Product_DbContext> options) : base(options) { }
}
