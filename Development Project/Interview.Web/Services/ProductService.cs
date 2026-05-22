using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;using Interview.Web.Data;
using Interview.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;

        public ProductService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProducts()
        {
            var products = await _db.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .ToListAsync();

            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Metadata = p.Metadata,
                Categories = p.ProductCategories
                    .Select(pc => pc.Category.Name)
                    .ToList()
            });
        }

        public async Task<ProductResponseDto> AddProduct(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Metadata = dto.Metadata ?? new Dictionary<string, string>()
            };

            foreach (var categoryName in dto.Categories)
            {
                var existingCategory = await _db.Categories
                    .FirstOrDefaultAsync(c => c.Name == categoryName);

                if (existingCategory == null)
                {
                    existingCategory = new Category
                    {
                        Name = categoryName
                    };

                    _db.Categories.Add(existingCategory);
                }

                product.ProductCategories.Add(new ProductCategory
                {
                    Product = product,
                    Category = existingCategory
                });
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Metadata = product.Metadata,
                Categories = product.ProductCategories
                    .Select(pc => pc.Category.Name)
                    .ToList()
            };
        }

        public async Task<IEnumerable<ProductResponseDto>> Search(
            string? query,
            string? category,
            string? metaKey,
            string? metaValue)
        {
            var productsQuery = _db.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(query) ||
                    p.Description.Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                productsQuery = productsQuery.Where(p =>
                    p.ProductCategories.Any(pc =>
                        pc.Category.Name == category));
            }

            var products = await productsQuery.ToListAsync();

            if (!string.IsNullOrWhiteSpace(metaKey) &&
                !string.IsNullOrWhiteSpace(metaValue))
            {
                products = products.Where(p =>
                    p.Metadata != null &&
                    p.Metadata.TryGetValue(metaKey, out var value) &&
                    value == metaValue)
                    .ToList();
            }

            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Metadata = p.Metadata,
                Categories = p.ProductCategories
                    .Select(pc => pc.Category.Name)
                    .ToList()
            });
        }
    }
}
