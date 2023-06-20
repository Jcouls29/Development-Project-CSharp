using Microsoft.EntityFrameworkCore;
using Sparcpoint.Core.Persistence.Entity.Sparcpoint.Context;
using Sparcpoint.Core.Persistence.Entity.Sparcpoint.Entities;
using Sparcpoint.Infrastructure.RequestModels;
using Sparcpoint.Infrastructure.Services.Interfaces;

namespace Sparcpoint.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly SparcpointBaseContext _context;

        public ProductService(SparcpointBaseContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateProductAsync(CreateProductRequest product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            try
            {
                // EVAL: Validate the product data before proceeding. I would use FluentValidation for this
                // EVAL: Can use a data mapper to map the request model to the entity model.

                var productModel = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    ProductImageUris = product.ProductImageUris,
                    ValidSkus = product.ValidSkus
                };

                _context.Products.Add(productModel);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // EVAL: Log the exception.
                // Would build a middleware to handle this that will remove having to use a try/catch block
                // or build a custom exception handler nuget package which would use the try/catch block.

                // Logger.Log(ex)
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            // EVAL: Add admin role check here to ensure only admins can delete products.
            var product = await _context.Products.FindAsync(id);

            if (product is null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            var productToUpdate = await _context.Products.FindAsync(product.InstanceId);

            if (productToUpdate is null)
            {
                return null;
            }

            productToUpdate.Name = product.Name;
            productToUpdate.Description = product.Description;
            productToUpdate.ProductImageUris = product.ProductImageUris;
            productToUpdate.ValidSkus = product.ValidSkus;
            productToUpdate.CreatedTimestamp = product.CreatedTimestamp;

            await _context.SaveChangesAsync();

            return productToUpdate;
        }

        public async Task<List<Product>> SearchProductsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return await _context.Products.Where(p => p.Name.Contains(name)).ToListAsync();
        }
    }
}