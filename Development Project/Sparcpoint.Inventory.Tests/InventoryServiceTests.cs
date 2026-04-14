using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories.InMemory;
using Sparcpoint.Inventory.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class InventoryServiceTests
    {
        private static async Task<(InventoryService service, int productId, InMemoryInventoryRepository repo)> SetupAsync()
        {
            var products = new InMemoryProductRepository();
            var inventory = new InMemoryInventoryRepository(products);
            var productService = new ProductService(products);
            var id = await productService.AddProductAsync(new Product
            {
                Name = "Widget",
                Attributes = new[] { new ProductAttribute(ProductAttributeKeys.Sku, "W-1") }
            });
            return (new InventoryService(inventory), id, inventory);
        }

        [Fact]
        public async Task Add_Then_Count_Returns_Quantity()
        {
            var (service, productId, _) = await SetupAsync();
            await service.AddInventoryAsync(productId, 10m);
            await service.AddInventoryAsync(productId, 5m);

            var count = await service.GetCountAsync(productId);
            Assert.Equal(15m, count.Quantity);
        }

        [Fact]
        public async Task Remove_Decrements_Count()
        {
            var (service, productId, _) = await SetupAsync();
            await service.AddInventoryAsync(productId, 10m);
            await service.RemoveInventoryAsync(productId, 3m);

            var count = await service.GetCountAsync(productId);
            Assert.Equal(7m, count.Quantity);
        }

        [Fact]
        public async Task Undo_Transaction_Removes_Its_Effect()
        {
            var (service, productId, _) = await SetupAsync();
            var addId = await service.AddInventoryAsync(productId, 10m);
            await service.AddInventoryAsync(productId, 2m);

            var undone = await service.UndoTransactionAsync(addId);
            Assert.True(undone);

            var count = await service.GetCountAsync(productId);
            Assert.Equal(2m, count.Quantity);
        }

        [Fact]
        public async Task Undo_Unknown_Transaction_Returns_False()
        {
            var (service, _, _) = await SetupAsync();
            Assert.False(await service.UndoTransactionAsync(9999));
        }

        [Fact]
        public async Task Bulk_Adjust_Records_All()
        {
            var (service, productId, _) = await SetupAsync();
            var ids = await service.AdjustInventoryBulkAsync(new[]
            {
                (productId, 4m),
                (productId, -1m),
                (productId, 7m),
            });

            Assert.Equal(3, ids.Count);
            var count = await service.GetCountAsync(productId);
            Assert.Equal(10m, count.Quantity);
        }

        [Fact]
        public async Task Add_NonPositive_Throws()
        {
            var (service, productId, _) = await SetupAsync();
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddInventoryAsync(productId, 0m));
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddInventoryAsync(productId, -1m));
        }

        [Fact]
        public async Task Count_By_Attribute_Aggregates_Matching_Products()
        {
            var products = new InMemoryProductRepository();
            var inventory = new InMemoryInventoryRepository(products);
            var productService = new ProductService(products);
            var service = new InventoryService(inventory);

            var a = await productService.AddProductAsync(new Product
            {
                Name = "A",
                Attributes = new[] { new ProductAttribute(ProductAttributeKeys.Color, "Red") }
            });
            var b = await productService.AddProductAsync(new Product
            {
                Name = "B",
                Attributes = new[] { new ProductAttribute(ProductAttributeKeys.Color, "Red") }
            });
            await productService.AddProductAsync(new Product
            {
                Name = "C",
                Attributes = new[] { new ProductAttribute(ProductAttributeKeys.Color, "Blue") }
            });

            await service.AddInventoryAsync(a, 4m);
            await service.AddInventoryAsync(b, 6m);

            var results = await service.GetCountsByAttributeAsync(ProductAttributeKeys.Color, "Red");
            Assert.Equal(2, results.Count);
            Assert.Equal(10m, results[0].Quantity + results[1].Quantity);
        }
    }
}
