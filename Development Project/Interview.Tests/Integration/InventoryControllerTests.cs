using Interview.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Interview.Tests
{
    public class InventoryControllerTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly TestWebApplicationFactory<Startup> _factory;

        public InventoryControllerTests(TestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AddInventory_Valid_Returns200()
        {
            var request = new InventoryTransactionRequest
            {
                ProductInstanceId = 12,
                Quantity = 5,
                TypeCategory = "restock",
            };

            var inventoryService = new Mock<IInventoryService>();
            inventoryService
                .Setup(service => service.AddInventoryAsync(It.Is<InventoryTransactionRequest>(model => model.ProductInstanceId == 12 && model.Quantity == 5)))
                .ReturnsAsync(new InventoryTransactionModel
                {
                    TransactionId = 7,
                    ProductInstanceId = 12,
                    Quantity = 5,
                    TypeCategory = "restock",
                    StartedTimestamp = DateTime.UtcNow,
                });

            using var client = CreateClient(inventoryService);

            var response = await client.PostAsJsonAsync("/api/v1/inventory/add", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<InventoryTransactionModel>();
            Assert.NotNull(body);
            Assert.Equal(7, body!.TransactionId);
            Assert.Equal(5, body.Quantity);
        }

        [Fact]
        public async Task GetInventoryCount_Returns200()
        {
            var inventoryService = new Mock<IInventoryService>();
            inventoryService
                .Setup(service => service.GetInventoryCountAsync(12))
                .ReturnsAsync(new InventoryCountModel
                {
                    ProductInstanceId = 12,
                    Quantity = 17,
                });

            using var client = CreateClient(inventoryService);

            var response = await client.GetAsync("/api/v1/inventory/count?productInstanceId=12");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<InventoryCountModel>();
            Assert.NotNull(body);
            Assert.Equal(12, body!.ProductInstanceId);
            Assert.Equal(17, body.Quantity);
        }

        [Fact]
        public async Task UndoTransaction_Returns200()
        {
            var inventoryService = new Mock<IInventoryService>();
            inventoryService
                .Setup(service => service.UndoTransactionAsync(9))
                .ReturnsAsync(new InventoryTransactionModel
                {
                    TransactionId = 9,
                    ProductInstanceId = 12,
                    Quantity = 5,
                    TypeCategory = "restock",
                    StartedTimestamp = DateTime.UtcNow.AddMinutes(-10),
                    CompletedTimestamp = DateTime.UtcNow,
                });

            using var client = CreateClient(inventoryService);

            var response = await client.DeleteAsync("/api/v1/inventory/transactions/9");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<InventoryTransactionModel>();
            Assert.NotNull(body);
            Assert.Equal(9, body!.TransactionId);
            Assert.NotNull(body.CompletedTimestamp);
        }

        [Fact]
        public async Task BulkAdd_Returns200()
        {
            var request = new BulkInventoryRequest
            {
                Items = new List<InventoryTransactionRequest>
                {
                    new InventoryTransactionRequest { ProductInstanceId = 12, Quantity = 5, TypeCategory = "restock" },
                    new InventoryTransactionRequest { ProductInstanceId = 13, Quantity = 3, TypeCategory = "restock" },
                },
            };

            var inventoryService = new Mock<IInventoryService>();
            inventoryService
                .Setup(service => service.AddInventoryBulkAsync(It.Is<BulkInventoryRequest>(model => model.Items.Count == 2)))
                .ReturnsAsync(new BulkInventoryResult
                {
                    Results = new List<InventoryTransactionModel>
                    {
                        new InventoryTransactionModel
                        {
                            TransactionId = 21,
                            ProductInstanceId = 12,
                            Quantity = 5,
                            StartedTimestamp = DateTime.UtcNow,
                            TypeCategory = "restock",
                        },
                        new InventoryTransactionModel
                        {
                            TransactionId = 22,
                            ProductInstanceId = 13,
                            Quantity = 3,
                            StartedTimestamp = DateTime.UtcNow,
                            TypeCategory = "restock",
                        },
                    },
                    Errors = new List<string>(),
                });

            using var client = CreateClient(inventoryService);

            var response = await client.PostAsJsonAsync("/api/v1/inventory/add/bulk", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<BulkInventoryResult>();
            Assert.NotNull(body);
            Assert.Equal(2, body!.Results.Count);
            Assert.Empty(body.Errors);
        }

        private HttpClient CreateClient(Mock<IInventoryService> inventoryService)
        {
            return _factory.CreateHttpsClient(services =>
            {
                services.RemoveAll<IInventoryService>();
                services.AddSingleton(inventoryService.Object);
            });
        }
    }
}
