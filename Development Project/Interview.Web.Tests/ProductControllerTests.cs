using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.DTOs;
using Sparcpoint.Core.Models;
using Sparcpoint.Core.Repositories;
using Sparcpoint.Core.Services;
using Xunit;

namespace Interview.Web.Tests
{
    public class ProductControllerTests
    {
        private class FakeProductRepository : IProductRepository
        {
            private readonly List<Product> _products = new List<Product>();

            public void Seed(Product product)
            {
                _products.Add(product);
            }

            public Task<int> AddAsync(Product product) => Task.FromResult(1);
            public Task<decimal> GetCurrentInventoryCountAsync(int productInstanceId) => Task.FromResult(0m);
            public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult<IEnumerable<Product>>(_products);
            public Task<Product> GetByIdAsync(int instanceId) => Task.FromResult(_products.FirstOrDefault(p => p.InstanceId == instanceId));
            public Task<int> GetSearchCountAsync(string name = null, string description = null, IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null)
            {
                return Task.FromResult(_products.Count);
            }

            public Task<IEnumerable<Product>> SearchAsync(string name = null, string description = null, IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null, int? skip = null, int? take = null)
            {
                return Task.FromResult<IEnumerable<Product>>(_products);
            }

            public Task UpdateAsync(Product product) => Task.CompletedTask;
        }

        private class FakeCategoryRepository : ICategoryRepository
        {
            public Task<int> AddAsync(Category category) => Task.FromResult(1);
            public Task DeleteAsync(int instanceId) => Task.CompletedTask;
            public Task<IEnumerable<Category>> GetAllAsync() => Task.FromResult<IEnumerable<Category>>(new List<Category>());
            public Task<Category> GetByIdAsync(int instanceId) => Task.FromResult<Category>(null);
            public Task<IEnumerable<int>> GetChildCategoryIdsAsync(int categoryInstanceId) => Task.FromResult<IEnumerable<int>>(new List<int>());
            public Task<IEnumerable<int>> GetParentCategoryIdsAsync(int categoryInstanceId) => Task.FromResult<IEnumerable<int>>(new List<int>());
            public Task UpdateAsync(Category category) => Task.CompletedTask;
            public Task UpdateParentCategoryRelationshipsAsync(int categoryInstanceId, IEnumerable<int> parentCategoryIds) => Task.CompletedTask;
        }

        private class FakeInventoryService : IInventoryService
        {
            public Task<int> AddInventoryAsync(int productInstanceId, decimal quantity, string typeCategory = null) => Task.FromResult(1);
            public Task<IEnumerable<int>> AddInventoryAsync(IEnumerable<InventoryAdjustment> adjustments) => Task.FromResult<IEnumerable<int>>(new List<int> { 1 });
            public Task<int> RemoveInventoryAsync(int productInstanceId, decimal quantity, string typeCategory = null) => Task.FromResult(1);
            public Task<IEnumerable<int>> RemoveInventoryAsync(IEnumerable<InventoryAdjustment> adjustments) => Task.FromResult<IEnumerable<int>>(new List<int> { 1 });
            public Task<bool> UndoTransactionAsync(int transactionId) => Task.FromResult(true);
            public Task<decimal> GetCurrentInventoryAsync(int productInstanceId) => Task.FromResult(42m);
            public Task<decimal> GetInventoryCountByFilterAsync(string name = null, string description = null, IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null) => Task.FromResult(42m);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsAllProducts_WhenNoSearchCriteriaProvided()
        {
            var productRepository = new FakeProductRepository();
            var categoryRepository = new FakeCategoryRepository();
            var inventoryService = new FakeInventoryService();
            var controller = new ProductController(productRepository, categoryRepository, inventoryService);

            var product = Product.Create("Test Product", "Description");
            product.SetInstanceId(1);
            productRepository.Seed(product);

            var result = await controller.GetAllProducts(new ProductSearchDto());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var productDtos = Assert.IsType<List<ProductDto>>(okResult.Value);

            Assert.Single(productDtos);
            Assert.Equal(1, productDtos[0].InstanceId);
            Assert.Equal(42m, productDtos[0].CurrentInventoryCount);
        }

        [Fact]
        public async Task SearchProducts_ReturnsSearchResultDto_WhenBodyProvided()
        {
            var productRepository = new FakeProductRepository();
            var categoryRepository = new FakeCategoryRepository();
            var inventoryService = new FakeInventoryService();
            var controller = new ProductController(productRepository, categoryRepository, inventoryService);

            var product = Product.Create("Search Product", "Description");
            product.SetInstanceId(2);
            productRepository.Seed(product);

            var result = await controller.SearchProducts(new ProductSearchDto { Name = "Search" });
            var okResult = Assert.IsType<OkObjectResult>(result);
            var searchResult = Assert.IsType<ProductSearchResultDto>(okResult.Value);

            Assert.Single(searchResult.Products);
            Assert.Equal(1, searchResult.TotalCount);
        }
    }
}
