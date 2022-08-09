using Microsoft.EntityFrameworkCore;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataLayer.Context
{
    public interface IProductDBContext
    {
        public DbSet<Products> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryOfCategory> CategoryOfCategory { get; set; }
        public DbSet<CatergoryAttributes> CatergoryAttributes { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<ProductAttribute> ProductAttribute { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        Task<int> SaveChangesAsync();
    }
}
