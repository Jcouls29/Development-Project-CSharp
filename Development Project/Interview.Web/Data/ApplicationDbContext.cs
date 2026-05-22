using System.Collections.Generic;
using System.Text.Json;
using Interview.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Interview.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ProductCategory composite key
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);

            // Metadata stored as JSON
            var converter = new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrEmpty(v) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null));

            modelBuilder.Entity<Product>()
                .Property(p => p.Metadata)
                .HasConversion(converter)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);

                entity.Property(t => t.Quantity)
                    .HasColumnType("decimal(19,6)");

                entity.Property(t => t.Type)
                    .HasConversion<string>()
                    .HasMaxLength(32);

                entity.HasOne<Product>()
                    .WithMany(p => p.InventoryTransactions)
                    .HasForeignKey(t => t.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => t.ProductId);
                entity.HasIndex(t => new { t.ProductId, t.CreatedAt });
            });
        }
    }
}
