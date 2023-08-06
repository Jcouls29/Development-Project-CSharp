using Microsoft.EntityFrameworkCore;

namespace Inventory.Data
{
    public class InventoryDataContext: DbContext
    {
        public InventoryDataContext()
        {

        }

        public InventoryDataContext(DbContextOptions<InventoryDataContext> options) : base(options) { } 

        public virtual DbSet<InventoryTransaction> InventoryTransaction { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });


            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.Property(e => e.Quantity).HasMaxLength(50);
            });
        }
    }
}