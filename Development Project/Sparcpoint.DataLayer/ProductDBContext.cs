using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataLayer.Context
{
    public class ProductDBContext : DbContext,IProductDBContext
    {
        protected IConfiguration _configuration;
        public ProductDBContext(IConfiguration configuration) {
            _configuration = configuration;
        } 
        public DbSet<Products> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryOfCategory> CategoryOfCategory { get; set; }
        public DbSet<CatergoryAttributes> CatergoryAttributes { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<ProductAttribute> ProductAttribute { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }

        public Task<int> SaveChangesAsync()
        {
           return base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Check Appsettings File for Connection String
            optionsBuilder.UseSqlServer(_configuration["Data:DefaultConnection:ConnectionString"]);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatergoryAttributes>(entity =>
            {
                entity.HasKey(p => new { p.InstanceId, p.Key });
            });
            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.HasKey(p => new { p.InstanceId, p.Key });
            });
            modelBuilder.Entity<ProductCategories>(entity =>
            {
                entity.HasKey(p => new { p.InstanceId, p.CategoryInstanceId }); ;
            });
        }

        }
}
