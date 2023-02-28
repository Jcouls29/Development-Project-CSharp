using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory;

public partial class SparcpointInventoryDatabaseContext : DbContext
{
    public SparcpointInventoryDatabaseContext()
    {
    }

    public SparcpointInventoryDatabaseContext(DbContextOptions<SparcpointInventoryDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryAttribute> CategoryAttributes { get; set; }

    public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Sparcpoint.Inventory.Database;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.InstanceId).HasName("PK__Categori__5C51994F03529CC5");

            entity.ToTable("Categories", "Instances");

            entity.Property(e => e.CreatedTimestamp).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasMany(d => d.CategoryInstances).WithMany(p => p.Instances)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryInstanceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CategoryCategories_Categories_Categories"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("InstanceId")
                        .HasConstraintName("FK_CategoryCategories_Categories"),
                    j =>
                    {
                        j.HasKey("InstanceId", "CategoryInstanceId");
                        j.ToTable("CategoryCategories", "Instances");
                    });

            entity.HasMany(d => d.Instances).WithMany(p => p.CategoryInstances)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("InstanceId")
                        .HasConstraintName("FK_CategoryCategories_Categories"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryInstanceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CategoryCategories_Categories_Categories"),
                    j =>
                    {
                        j.HasKey("InstanceId", "CategoryInstanceId");
                        j.ToTable("CategoryCategories", "Instances");
                    });
        });

        modelBuilder.Entity<CategoryAttribute>(entity =>
        {
            entity.HasKey(e => new { e.InstanceId, e.Key });

            entity.ToTable("CategoryAttributes", "Instances");

            entity.Property(e => e.Key)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Value)
                .HasMaxLength(512)
                .IsUnicode(false);

            entity.HasOne(d => d.Instance).WithMany(p => p.CategoryAttributes)
                .HasForeignKey(d => d.InstanceId)
                .HasConstraintName("FK_CategoryAttributes_Categories");
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Inventor__55433A6BA1F919AF");

            entity.ToTable("InventoryTransactions", "Transactions");

            entity.HasIndex(e => e.CompletedTimestamp, "IX_InventoryTransactions_CompletedTimestamp");

            entity.HasIndex(e => e.ProductInstanceId, "IX_InventoryTransactions_ProductInstanceId");

            entity.HasIndex(e => new { e.ProductInstanceId, e.Quantity }, "IX_InventoryTransactions_ProductInstanceId_Quantity");

            entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");
            entity.Property(e => e.StartedTimestamp).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.TypeCategory)
                .HasMaxLength(32)
                .IsUnicode(false);

            entity.HasOne(d => d.ProductInstance).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.ProductInstanceId)
                .HasConstraintName("FK_InventoryTransactions_Products");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.InstanceId).HasName("PK__Products__5C51994F1D2665C1");

            entity.ToTable("Products", "Instances");

            entity.Property(e => e.CreatedTimestamp).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.ProductImageUris).IsUnicode(false);
            entity.Property(e => e.ValidSkus).IsUnicode(false);

            entity.HasMany(d => d.CategoryInstances).WithMany(p => p.InstancesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryInstanceId")
                        .HasConstraintName("FK_ProductCategories_Categories"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("InstanceId")
                        .HasConstraintName("FK_ProductCategories_Products"),
                    j =>
                    {
                        j.HasKey("InstanceId", "CategoryInstanceId");
                        j.ToTable("ProductCategories", "Instances");
                    });
        });

        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.HasKey(e => new { e.InstanceId, e.Key });

            entity.ToTable("ProductAttributes", "Instances");

            entity.HasIndex(e => new { e.Key, e.Value }, "IX_ProductAttributes_Key_Value");

            entity.Property(e => e.Key)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Value)
                .HasMaxLength(512)
                .IsUnicode(false);

            entity.HasOne(d => d.Instance).WithMany(p => p.ProductAttributes)
                .HasForeignKey(d => d.InstanceId)
                .HasConstraintName("FK_ProductAttributes_Products");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
