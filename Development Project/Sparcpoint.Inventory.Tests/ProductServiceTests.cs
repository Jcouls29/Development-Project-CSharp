using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories.InMemory;
using Sparcpoint.Inventory.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class ProductServiceTests
    {
        private static ProductService CreateService(out InMemoryProductRepository repo)
        {
            repo = new InMemoryProductRepository();
            return new ProductService(repo);
        }

        [Fact]
        public async Task AddProduct_Returns_Positive_Id()
        {
            var service = CreateService(out _);
            var id = await service.AddProductAsync(new Product { Name = "Widget", Description = "d" });
            Assert.True(id > 0);
        }

        [Fact]
        public async Task AddProduct_Missing_Name_Throws()
        {
            var service = CreateService(out _);
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddProductAsync(new Product { Name = "" }));
        }

        [Fact]
        public async Task AddProduct_OversizedName_Throws()
        {
            var service = CreateService(out _);
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.AddProductAsync(new Product { Name = new string('x', 257) }));
        }

        [Fact]
        public async Task Search_By_Attribute_Matches()
        {
            var service = CreateService(out _);
            var redId = await service.AddProductAsync(new Product
            {
                Name = "Red",
                Attributes = new[] { new ProductAttribute(ProductAttributeKeys.Color, "Red") }
            });
            await service.AddProductAsync(new Product
            {
                Name = "Blue",
                Attributes = new[] { new ProductAttribute(ProductAttributeKeys.Color, "Blue") }
            });

            var results = await service.SearchAsync(new ProductSearchCriteria
            {
                AttributeMatches = new[] { new ProductAttribute(ProductAttributeKeys.Color, "Red") }
            });

            Assert.Single(results);
            Assert.Equal(redId, results[0].InstanceId);
        }

        [Fact]
        public async Task Search_By_Category_Requires_All_Categories()
        {
            var service = CreateService(out _);
            await service.AddProductAsync(new Product { Name = "A", CategoryIds = new[] { 1, 2 } });
            await service.AddProductAsync(new Product { Name = "B", CategoryIds = new[] { 1 } });

            var results = await service.SearchAsync(new ProductSearchCriteria
            {
                CategoryIds = new[] { 1, 2 }
            });

            Assert.Single(results);
            Assert.Equal("A", results[0].Name);
        }
    }
}
