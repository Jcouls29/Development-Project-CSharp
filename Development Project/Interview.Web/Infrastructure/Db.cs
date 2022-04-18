using System;
using System.Collections.Generic;
using Interview.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Infrastructure;

public interface IProductInvetoryDb
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<MetadataType> MetadataTypes { get; set; }
    public DbSet<Metadata> Metadatas { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<CategoryHierarhy> CategoryHierarhy { get; set; }
    public DbSet<InventoryOperation> InventoryOperations { get; set; }
}
public class Db : DbContext, IProductInvetoryDb
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<MetadataType> MetadataTypes { get; set; }
    public DbSet<Metadata> Metadatas { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<CategoryHierarhy> CategoryHierarhy { get; set; }
    public DbSet<InventoryOperation> InventoryOperations { get; set; }
    
    public Db(DbContextOptions options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DataSeed.SeedData(modelBuilder);
    }
}