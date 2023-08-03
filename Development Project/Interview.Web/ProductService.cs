using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Interview.Web.Services.interfaces;
using Interview.Web.Contexts;
using Interview.Web.Entities;
using Interview.Web.Model;

using System.Threading.Tasks;
 // If using Entity Framework

namespace Interview.Web.Services
{
    // Define your entity models (Product, Category, Metadata, etc.) here
    
    public class ProductService : IProductService
    {
        private readonly YourDbContext _dbContext; // Replace with your DbContext class

        public ProductService(YourDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddProduct(string name, string description, decimal price, List<string> categoryNames, Dictionary<string, string> metadata)
        {
            // Create a new product
            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                InventoryCount = 0 // Initialize inventory
            };

            // Add categories to the product
            foreach (var categoryName in categoryNames)
            {
                var category = _dbContext.Categories.FirstOrDefault(c => c.Name == categoryName);
                if (category != null)
                {
                    product.Categories.Add(category);
                }
            }

            // Add metadata to the product
            foreach (var kvp in metadata)
            {
                var metadataItem = new Metadata
                {
                    Key = kvp.Key,
                    Value = kvp.Value
                };
                product.Metadata.Add(metadataItem);
            }

            // Add the product to the database
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
        }
        public async Task<List<Product>> SearchProductsAsync(string searchTerm, List<int> categoryIds, List<int> metadataIds)
        {
        IQueryable<Product> query = _context.Products.Include(p => p.Categories).Include(p => p.Metadata);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm));
        }

        if (categoryIds != null && categoryIds.Count > 0)
        {
            query = query.Where(p => p.Categories.Any(c => categoryIds.Contains(c.Id)));
        }

        if (metadataIds != null && metadataIds.Count > 0)
        {
            query = query.Where(p => p.Metadata.Any(m => metadataIds.Contains(m.Id)));
        }

        return await query.ToListAsync();
        }
    }
}
