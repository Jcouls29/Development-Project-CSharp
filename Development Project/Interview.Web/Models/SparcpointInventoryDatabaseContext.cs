using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Interview.Web.Models
{
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
        public virtual DbSet<CategoryCategory> CategoryCategories { get; set; }
        public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=127.0.0.1;user=tuseroni;password=Yamikaze5;Database=Sparcpoint.Inventory.Database;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.InstanceId)
                    .HasName("PK__Categori__5C51994F1FCAB40B");

                entity.ToTable("Categories", "Instances");

                entity.Property(e => e.CreatedTimestamp).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CategoryAttribute>(entity =>
            {
                entity.HasKey(e => new { e.InstanceId, e.Key });

                entity.ToTable("CategoryAttributes", "Instances");

                entity.Property(e => e.Key)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.HasOne(d => d.Instance)
                    .WithMany(p => p.CategoryAttributes)
                    .HasForeignKey(d => d.InstanceId)
                    .HasConstraintName("FK_CategoryAttributes_Categories");
            });

            modelBuilder.Entity<CategoryCategory>(entity =>
            {
                entity.HasKey(e => new { e.InstanceId, e.CategoryInstanceId });

                entity.ToTable("CategoryCategories", "Instances");

                entity.HasOne(d => d.CategoryInstance)
                    .WithMany(p => p.CategoryCategoryCategoryInstances)
                    .HasForeignKey(d => d.CategoryInstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CategoryCategories_Categories_Categories");

                entity.HasOne(d => d.Instance)
                    .WithMany(p => p.CategoryCategoryInstances)
                    .HasForeignKey(d => d.InstanceId)
                    .HasConstraintName("FK_CategoryCategories_Categories");
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Inventor__55433A6B8358D9AC");

                entity.ToTable("InventoryTransactions", "Transactions");

                entity.HasIndex(e => e.CompletedTimestamp, "IX_InventoryTransactions_CompletedTimestamp");

                entity.HasIndex(e => e.ProductInstanceId, "IX_InventoryTransactions_ProductInstanceId");

                entity.HasIndex(e => new { e.ProductInstanceId, e.Quantity }, "IX_InventoryTransactions_ProductInstanceId_Quantity");

                entity.Property(e => e.Quantity).HasColumnType("decimal(19, 6)");

                entity.Property(e => e.StartedTimestamp).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.TypeCategory)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProductInstance)
                    .WithMany(p => p.InventoryTransactions)
                    .HasForeignKey(d => d.ProductInstanceId)
                    .HasConstraintName("FK_InventoryTransactions_Products");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.InstanceId)
                    .HasName("PK__Products__5C51994F8242E089");

                entity.ToTable("Products", "Instances");

                entity.Property(e => e.CreatedTimestamp).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.ProductImageUris)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.ValidSkus)
                    .IsRequired()
                    .IsUnicode(false);
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
                    .IsRequired()
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.HasOne(d => d.Instance)
                    .WithMany(p => p.ProductAttributes)
                    .HasForeignKey(d => d.InstanceId)
                    .HasConstraintName("FK_ProductAttributes_Products");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => new { e.InstanceId, e.CategoryInstanceId });

                entity.ToTable("ProductCategories", "Instances");

                entity.HasOne(d => d.CategoryInstance)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(d => d.CategoryInstanceId)
                    .HasConstraintName("FK_ProductCategories_Categories");

                entity.HasOne(d => d.Instance)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(d => d.InstanceId)
                    .HasConstraintName("FK_ProductCategories_Products");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
