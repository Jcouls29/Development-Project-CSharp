using Microsoft.EntityFrameworkCore;
using Sparcpoint.Domain;

namespace Interview.Web
{
    /** ApplicationDbContext is a class that inherits from DbContext and represents the database context for the application.*/
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductAttribute>()
                .HasKey(e => new { e.Key, e.InstanceId });
        }
    }
}
