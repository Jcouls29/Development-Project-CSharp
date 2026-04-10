using System.Collections.Generic;
using Interview.Web.Controllers;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;
using Sparcpoint.Inventory.Tests.Fakes;
using Xunit;

namespace Sparcpoint.Inventory.Tests.Controllers
{
    /// <summary>
    /// EVAL: Controller tests verify HTTP response codes and DTO mapping.
    /// With global ErrorHandlingMiddleware, error cases now throw exceptions
    /// (the middleware maps them to HTTP status codes at runtime).
    /// Unit tests verify both the happy path (HTTP responses) and error path (exceptions).
    /// </summary>
    public class InventoryControllerTests
    {
        private readonly InventoryController _Controller;
        private readonly InMemoryProductRepository _ProductRepository;
        private readonly IInventoryService _InventoryService;

        public InventoryControllerTests()
        {
            _ProductRepository = new InMemoryProductRepository();
            var inventoryRepository = new InMemoryInventoryRepository(_ProductRepository);
            _InventoryService = new InventoryService(inventoryRepository, _ProductRepository);
            _Controller = new InventoryController(_InventoryService);
        }

        private async Task<Product> CreateTestProduct()
        {
            return await _ProductRepository.CreateAsync(new Product
            {
                Name = "Test Product",
                Description = "For testing",
                ProductImageUris = "",
                ValidSkus = ""
            });
        }

        [Fact]
        public async Task AddToInventory_ReturnsOkWithTransaction()
        {
            var product = await CreateTestProduct();

            var result = await _Controller.AddToInventory(product.InstanceId, new InventoryTransactionRequest { Quantity = 25 });

            var okResult = Assert.IsType<OkObjectResult>(result);
            var transaction = Assert.IsType<InventoryTransactionResponse>(okResult.Value);
            Assert.Equal(25, transaction.Quantity);
            Assert.Equal(product.InstanceId, transaction.ProductInstanceId);
        }

        [Fact]
        public async Task AddToInventory_WithNonExistentProduct_ThrowsKeyNotFoundException()
        {
            // EVAL: With middleware handling errors, the controller throws rather than catching.
            // The middleware maps KeyNotFoundException → 404 at runtime.
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _Controller.AddToInventory(999, new InventoryTransactionRequest { Quantity = 10 }));
        }

        [Fact]
        public async Task RemoveFromInventory_CreatesNegativeTransaction()
        {
            var product = await CreateTestProduct();

            var result = await _Controller.RemoveFromInventory(product.InstanceId, new InventoryTransactionRequest { Quantity = 15 });

            var okResult = Assert.IsType<OkObjectResult>(result);
            var transaction = Assert.IsType<InventoryTransactionResponse>(okResult.Value);
            Assert.Equal(-15, transaction.Quantity);
        }

        [Fact]
        public async Task GetInventoryCount_ReturnsCorrectNetCount()
        {
            var product = await CreateTestProduct();

            await _Controller.AddToInventory(product.InstanceId, new InventoryTransactionRequest { Quantity = 100 });
            await _Controller.RemoveFromInventory(product.InstanceId, new InventoryTransactionRequest { Quantity = 30 });

            var result = await _Controller.GetInventoryCount(product.InstanceId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<InventoryCountResponse>(okResult.Value);
            Assert.Equal(70, response.Count);
        }

        [Fact]
        public async Task UndoTransaction_WithValidId_ReturnsNoContent()
        {
            var product = await CreateTestProduct();
            var addResult = await _Controller.AddToInventory(product.InstanceId, new InventoryTransactionRequest { Quantity = 50 });
            var transaction = (addResult as OkObjectResult)!.Value as InventoryTransactionResponse;

            var result = await _Controller.UndoTransaction(transaction!.TransactionId);

            Assert.IsType<NoContentResult>(result);

            // Verify count reflects the undo
            var countResult = await _Controller.GetInventoryCount(product.InstanceId);
            var countResponse = (countResult as OkObjectResult)!.Value as InventoryCountResponse;
            Assert.Equal(0, countResponse!.Count);
        }

        [Fact]
        public async Task UndoTransaction_WithNonExistentId_ReturnsNotFound()
        {
            var result = await _Controller.UndoTransaction(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
