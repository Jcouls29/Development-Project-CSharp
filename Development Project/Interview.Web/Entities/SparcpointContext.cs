using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Entities
{
    public class SparcpointContext : DbContext
    {
        public virtual DbSet<Categories> Category { get; set; }
        public virtual DbSet<Products> Product { get; set; }

        public SparcpointContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\ProjectModels; Initial Catalog=Sparcpoint.Inventory.Database; Trusted_Connection = True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.InstanceId);

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

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(e => e.InstanceId);

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

                entity.Property(e => e.StartDate).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.EndDate).HasDefaultValueSql("9999-12-31");
            }); 
            
            modelBuilder.Entity<ProductAttributes>(entity =>
            {
                entity.HasKey(e => new { e.InstanceId, e.Key });

                entity.ToTable("ProductAttributes", "Instances");

               // entity.HasIndex(e => new { e.Key, e.Value }, "IX_ProductAttributes_Key_Value");

                entity.Property(e => e.Key)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductAttributes)
                    .HasForeignKey(d => d.InstanceId)
                    .HasConstraintName("FK_ProductAttributes_Products");
            });
        }
    }
}
