using Microsoft.EntityFrameworkCore;
using Sparcpoint.Entities;

namespace Sparcpoint.DataRepository.DbContext
{
    //I understand that it is mentioned in the document that ISqlExecutor should be used.
    //I don't see in the class how we query the database. I see only Command. Not sure if I misunderstood the ISqlExecutor.
    //I never worked with the way database is setup in this project.
    //I am familiar with Entity Framework and Dapper.
    //I don't want to work with something I am not familiar with and go in a wrong way.
    //Sure, I want to learn and curious if this is also a right way to do.
    //For now, I thought this is just sample project and go on with Entity framework.
    public class SparcpointDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public SparcpointDbContext()
        {
        }

        public SparcpointDbContext(DbContextOptions<SparcpointDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        //public DbSet<ProductAttribute> ProductAttributes { get; set; }
        //public DbSet<CategoryAttribute> CategoryAttributes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}