using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories;
using Interview.Web.Services;
using Moq;
using Xunit;

namespace Interview.Web.Tests
{
    public class InventoryServiceTests
    {
        [Fact]
        public async Task AddInventory_DelegatesToRepository()
        {
            var mockInvRepo = new Mock<IInventoryRepository>();
            var mockProdRepo = new Mock<IProductRepository>();

            var tx = new InventoryTransaction { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Delta = 5, Note = "add" };
            mockInvRepo.Setup(r => r.AddInventoryAsync(tx.ProductId, tx.Delta, tx.Note)).ReturnsAsync(tx);

            var svc = new InventoryService(mockInvRepo.Object, mockProdRepo.Object);

            var result = await svc.AddInventoryAsync(tx.ProductId, tx.Delta, tx.Note);

            Assert.NotNull(result);
            Assert.Equal(5, result.Delta);
            mockInvRepo.Verify(r => r.AddInventoryAsync(tx.ProductId, tx.Delta, tx.Note), Times.Once);
        }

        [Fact]
        public async Task UndoLastInventory_DelegatesToRepository()
        {
            var mockInvRepo = new Mock<IInventoryRepository>();
            var mockProdRepo = new Mock<IProductRepository>();

            var productId = Guid.NewGuid();
            mockInvRepo.Setup(r => r.UndoLastInventoryAsync(productId)).ReturnsAsync(true);

            var svc = new InventoryService(mockInvRepo.Object, mockProdRepo.Object);

            var ok = await svc.UndoLastInventoryAsync(productId);

            Assert.True(ok);
            mockInvRepo.Verify(r => r.UndoLastInventoryAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetQuantityByMetadata_SumsProductQuantities()
        {
            var mockInvRepo = new Mock<IInventoryRepository>();
            var mockProdRepo = new Mock<IProductRepository>();

            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "A", Quantity = 3 },
                new Product { Id = Guid.NewGuid(), Name = "B", Quantity = 7 }
            };

            mockProdRepo.Setup(r => r.SearchByMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(products);

            var svc = new InventoryService(mockInvRepo.Object, mockProdRepo.Object);

            var sum = await svc.GetQuantityByMetadataAsync(new Dictionary<string, string> { { "sku", "123" } });

            Assert.Equal(10, sum);
            mockProdRepo.Verify(r => r.SearchByMetadataAsync(It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
    }
}
