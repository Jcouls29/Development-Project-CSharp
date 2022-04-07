using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DBCore
{
    public class SparcpointDbContext:DbContext
    {

        public SparcpointDbContext(DbContextOptions<SparcpointDbContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SparcpointDbContext).Assembly);
        }
        //public DbSet<Category> Categories { get; set; }
        //public DbSet<CategoryAttribute> CategoryAttributes { get; set; }
        //public DbSet<CategoryOfCategory> categoryOfCategories { get; set; }
        //public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
