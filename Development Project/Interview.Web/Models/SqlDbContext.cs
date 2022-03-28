using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Interview.Web.Models
{
    public partial class SqlDbContext : DbContext
    {
        public SqlDbContext()
        {
        }

        public SqlDbContext(DbContextOptions<SqlDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryAttribute> CategoryAttributes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=localhost;Database=assignment;Trusted_Connection=True;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.InstanceId)
                    .HasName("PK__Categori__5C51994FF8D6F2CF");

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

                entity.HasMany(d => d.CategoryInstances)
                    .WithMany(p => p.Instances)
                    .UsingEntity<Dictionary<string, object>>(
                        "CategoryCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("CategoryInstanceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CategoryCategories_Categories_Categories"),
                        r => r.HasOne<Category>().WithMany().HasForeignKey("InstanceId").HasConstraintName("FK_CategoryCategories_Categories"),
                        j =>
                        {
                            j.HasKey("InstanceId", "CategoryInstanceId");

                            j.ToTable("CategoryCategories", "Instances");
                        });

                entity.HasMany(d => d.Instances)
                    .WithMany(p => p.CategoryInstances)
                    .UsingEntity<Dictionary<string, object>>(
                        "CategoryCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("InstanceId").HasConstraintName("FK_CategoryCategories_Categories"),
                        r => r.HasOne<Category>().WithMany().HasForeignKey("CategoryInstanceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CategoryCategories_Categories_Categories"),
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
                    .IsRequired()
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.HasOne(d => d.Instance)
                    .WithMany(p => p.CategoryAttributes)
                    .HasForeignKey(d => d.InstanceId)
                    .HasConstraintName("FK_CategoryAttributes_Categories");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.InstanceId)
                    .HasName("PK__Products__5C51994FDA00B8CF");

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

                entity.HasMany(d => d.CategoryInstances)
                    .WithMany(p => p.InstancesNavigation)
                    .UsingEntity<Dictionary<string, object>>(
                        "ProductCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("CategoryInstanceId").HasConstraintName("FK_ProductCategories_Categories"),
                        r => r.HasOne<Product>().WithMany().HasForeignKey("InstanceId").HasConstraintName("FK_ProductCategories_Products"),
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
