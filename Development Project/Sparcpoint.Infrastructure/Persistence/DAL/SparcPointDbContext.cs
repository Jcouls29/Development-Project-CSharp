using Microsoft.EntityFrameworkCore;
using Sparcpoint.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastructure.Persistence.DAL
{
    public class SparcPointDbContext:DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryAttribute> CategoryAttributes { get; set; }
        public DbSet<CategoryCategory> CategoryCategories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }


        public SparcPointDbContext(DbContextOptions<SparcPointDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Your model configurations
            //modelBuilder.Entity<Product>()
            //    .Property(p => p.CreatedTimestamp)
            //    .HasDefaultValueSql("SYSUTCDATETIME()");

            //modelBuilder.Entity<Category>()
            //    .Property(c => c.CreatedTimestamp)
            //    .HasDefaultValueSql("SYSUTCDATETIME()");


            modelBuilder.Entity<InventoryTransaction>()
            .HasIndex(it => it.ProductInstanceId);

            modelBuilder.Entity<InventoryTransaction>()
                .HasIndex(it => new { it.ProductInstanceId, it.Quantity });

            modelBuilder.Entity<InventoryTransaction>()
                .HasIndex(it => it.CompletedTimestamp);

            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.InstanceId, pc.CategoryInstanceId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductAttribute>()
                .HasKey(pa => new { pa.InstanceId, pa.Key });

            modelBuilder.Entity<ProductAttribute>()
                .HasOne(pa => pa.Product)
                .WithMany(p => p.ProductAttributes)
                .HasForeignKey(pa => pa.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CategoryCategory>()
                .HasKey(cc => new { cc.InstanceId, cc.CategoryInstanceId });

            modelBuilder.Entity<CategoryCategory>()
                .HasOne(cc => cc.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(cc => cc.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CategoryCategory>()
                .HasOne(cc => cc.ChildCategory)
                .WithMany(c => c.ParentCategories)
                .HasForeignKey(cc => cc.CategoryInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CategoryAttribute>()
                .HasKey(ca => new { ca.InstanceId, ca.Key });

            modelBuilder.Entity<CategoryAttribute>()
                .HasOne(ca => ca.Category)
                .WithMany(c => c.CategoryAttributes)
                .HasForeignKey(ca => ca.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductAttribute>()
                .HasIndex(pa => new { pa.Key, pa.Value });

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name).IsUnique();

            base.OnModelCreating(modelBuilder);
        }


    }
}
