using Microsoft.EntityFrameworkCore;
using Sparcpoint.Core.Entities;

namespace Sparcpoint.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var productEntity = modelBuilder.Entity<Product>();
        var categoryEntity = modelBuilder.Entity<Category>();
        var inventoryTransactionEntity = modelBuilder.Entity<InventoryTransaction>();

        productEntity
            .OwnsMany(
                e => e.Attributes, builder => { builder.ToJson(); }
            );

        productEntity
            .HasMany(e => e.Categories)
            .WithMany(e => e.Products)
            .UsingEntity("ProductCategory",
                r => r.HasOne(typeof(Category)).WithMany().HasForeignKey("CategoryId").HasPrincipalKey(nameof(Category.Id)),
                l => l.HasOne(typeof(Product)).WithMany().HasForeignKey("ProductId").HasPrincipalKey(nameof(Product.Id)),
                j => j.HasKey("ProductId", "CategoryId")
            );

        categoryEntity
            .OwnsMany(
                e => e.Attributes, builder => { builder.ToJson(); }
            );

        inventoryTransactionEntity
            .HasOne(e => e.Product)
            .WithMany()
            .HasForeignKey(e => e.ProductId);
    }
}
