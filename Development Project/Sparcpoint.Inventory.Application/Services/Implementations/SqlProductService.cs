using Sparcpoint.Inventory.Application.Repositories;
using Sparcpoint.Inventory.Domain.Entities.Instances;
using Sparcpoint.Inventory.Domain.Models.Instances;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Application.Services.Implementations
{
    public class SqlProductService : IProductService
    {
        private readonly IProductRepository _ProductRepository;

        public SqlProductService(IProductRepository productRepository)
        {
            _ProductRepository = productRepository;
        }

        public Task<Product> GetAsync(int id)
        {
            var result = _ProductRepository.GetAsync(id);
            return result;
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            var result = _ProductRepository.GetAllAsync();
            return result;
        }

        public Task<Product> AddProductAsync(Product item)
        {
            var result = _ProductRepository.AddAsync(item);
            return result;
        }

        public Task<Product> AddProductAttributeAsync(int productId, string key, string value)
        {
            throw new NotImplementedException();
        }

        public Task<Product> AddProductAttributesAsync(int productId, params (string key, string value)[] attributes)
        {
            throw new NotImplementedException();
        }

        public Task<Product> AddProductCategoriesAsync(int productId, params int[] categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<Product> AddProductCategoryAsync(int productId, int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchModel model)
        {
            throw new NotImplementedException();
        }
    }
}
