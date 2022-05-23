using Microsoft.EntityFrameworkCore;
using Sparcpoint.DataRepository.DbContext;
using Sparcpoint.DataRepository.Interfaces;
using Sparcpoint.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.DataRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly SparcpointDbContext dbContext;

        public ProductRepository(SparcpointDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> Insert(Product product)
        {
            using var context = new SparcpointDbContext();
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product.InstanceId;
        }

        public async Task<Product> GetById(int id)
        {
            return await this.dbContext.Products.SingleOrDefaultAsync(p => p.InstanceId == id);
        }

        public async Task<List<Product>> GetAll()
        {
            return await this.dbContext.Products.ToListAsync();

            // return this.GetAllProducts();
        }

        private List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            var product1 = new Product
            {
                InstanceId = 1,
                Name = "Product 1",
                Description = "Product 1 Description",
                ProductImageUris = "Product 1 ProductImageUris",
                ValidSkus = "Product 1 Skus",
                CreatedTimestamp = DateTime.UtcNow
            };

            var product2 = new Product
            {
                InstanceId = 2,
                Name = "Product 2",
                Description = "Product 2 Description",
                ProductImageUris = "Product 2 ProductImageUris",
                ValidSkus = "Product 2 Skus",
                CreatedTimestamp = DateTime.UtcNow
            };

            products.Add(product1);
            products.Add(product2);

            return products;
        }
    }
}