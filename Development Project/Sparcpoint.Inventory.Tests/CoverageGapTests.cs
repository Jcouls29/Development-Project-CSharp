// Tests targeting remaining uncovered lines for 100% line coverage.

using Interview.Web.Controllers;
using Interview.Web.Models.Requests;
using Interview.Web.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainTransactionItem = Sparcpoint.Inventory.Abstract.InventoryTransactionItem;

namespace Sparcpoint.Inventory.Tests
{
    public class CoverageGapTests
    {
        #region InventoryController - UndoTransaction success path (200)

        [Fact]
        public async Task UndoTransaction_ActiveTransaction_Returns200WithDetail()
        {
            // Arrange: repository returns a transaction that WAS active before undo
            // and is now active (the undo happened inside the controller call)
            var mockRepo = new Mock<IInventoryRepository>();
            // Return a transaction that is still active (CompletedTimestamp null)
            // This simulates the undo happening but repo returning updated record where IsActive is still true temporarily
            mockRepo.Setup(r => r.UndoTransactionAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InventoryTransaction
                {
                    TransactionId = 1,
                    ProductInstanceId = 1,
                    Quantity = 25,
                    StartedTimestamp = DateTime.UtcNow.AddHours(-1),
                    CompletedTimestamp = null, // Still active = freshly returned before completion
                    TypeCategory = "Purchase"
                });

            var controller = new InventoryController(mockRepo.Object);

            // Act
            var result = await controller.UndoTransaction(1, CancellationToken.None);

            // Assert - should return 200 Ok with the transaction detail
            var okResult = Assert.IsType<OkObjectResult>(result);
            var detail = Assert.IsType<TransactionDetail>(okResult.Value);
            Assert.Equal(1, detail.TransactionId);
            Assert.True(detail.IsActive);
        }

        #endregion

        #region ProductController - CreateProduct with valid ImageUris

        [Fact]
        public async Task CreateProduct_WithValidImageUris_Returns201()
        {
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) =>
                {
                    p.InstanceId = 1;
                    p.CreatedTimestamp = DateTime.UtcNow;
                    p.Categories = new Dictionary<int, string>();
                    return p;
                });

            var controller = new ProductController(mockRepo.Object);

            var request = new CreateProductRequest
            {
                Name = "Test Product",
                Description = "Description",
                ImageUris = new List<string> { "https://example.com/image.jpg", "https://example.com/image2.png" },
                Skus = new List<string> { "SKU-1" },
                Attributes = new Dictionary<string, string> { { "Color", "Red" } },
                CategoryIds = new List<int> { 1 }
            };

            var result = await controller.CreateProduct(request, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_WithInvalidImageUri_Returns400()
        {
            var mockRepo = new Mock<IProductRepository>();
            var controller = new ProductController(mockRepo.Object);

            var request = new CreateProductRequest
            {
                Name = "Test",
                Description = "Desc",
                ImageUris = new List<string> { "not-a-uri" }
            };

            var result = await controller.CreateProduct(request, CancellationToken.None);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(badRequest.Value);
            Assert.Equal("VALIDATION_ERROR", error.Code);
            Assert.Contains("not-a-uri", error.Message);
        }

        [Fact]
        public async Task CreateProduct_WithEmptyImageUris_Returns201()
        {
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) =>
                {
                    p.InstanceId = 1;
                    p.CreatedTimestamp = DateTime.UtcNow;
                    p.Categories = new Dictionary<int, string>();
                    return p;
                });

            var controller = new ProductController(mockRepo.Object);

            var request = new CreateProductRequest
            {
                Name = "Test",
                Description = "Desc",
                ImageUris = new List<string>() // empty is valid
            };

            var result = await controller.CreateProduct(request, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateProduct_NullImageUris_Returns201()
        {
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) =>
                {
                    p.InstanceId = 1;
                    p.CreatedTimestamp = DateTime.UtcNow;
                    p.Categories = new Dictionary<int, string>();
                    return p;
                });

            var controller = new ProductController(mockRepo.Object);

            var request = new CreateProductRequest
            {
                Name = "Test",
                Description = "Desc",
                ImageUris = null // null is valid
            };

            var result = await controller.CreateProduct(request, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateProduct_NullAttributes_Returns201()
        {
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) =>
                {
                    p.InstanceId = 1;
                    p.CreatedTimestamp = DateTime.UtcNow;
                    p.Categories = new Dictionary<int, string>();
                    return p;
                });

            var controller = new ProductController(mockRepo.Object);

            var request = new CreateProductRequest
            {
                Name = "Test",
                Description = "Desc",
                Attributes = null
            };

            var result = await controller.CreateProduct(request, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        #endregion

        #region InventoryController - RemoveInventory bulk

        [Fact]
        public async Task RemoveInventory_BulkItems_ReturnsAllTransactions()
        {
            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.RemoveInventoryAsync(It.IsAny<IEnumerable<DomainTransactionItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InventoryTransaction>
                {
                    new() { TransactionId = 1, ProductInstanceId = 1, Quantity = -5, StartedTimestamp = DateTime.UtcNow },
                    new() { TransactionId = 2, ProductInstanceId = 2, Quantity = -3, StartedTimestamp = DateTime.UtcNow }
                });

            var controller = new InventoryController(mockRepo.Object);

            var request = new RemoveInventoryRequest
            {
                Items = new List<Interview.Web.Models.Requests.InventoryTransactionItem>
                {
                    new() { ProductInstanceId = 1, Quantity = 5 },
                    new() { ProductInstanceId = 2, Quantity = 3 }
                }
            };

            var result = await controller.RemoveInventory(request, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<InventoryTransactionResponse>(okResult.Value);
            Assert.Equal(2, response.Transactions.Count);
        }

        #endregion

        #region ProductController - MapToResponse with null categories

        [Fact]
        public async Task GetProductById_NullCategories_ReturnsEmptyList()
        {
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Product
                {
                    InstanceId = 1,
                    Name = "Test",
                    Description = "Desc",
                    Categories = null // null categories
                });

            var controller = new ProductController(mockRepo.Object);

            var result = await controller.GetProductById(1, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ProductResponse>(okResult.Value);
            Assert.NotNull(response.Categories);
            Assert.Empty(response.Categories);
        }

        [Fact]
        public async Task GetProductById_NullAttributes_ReturnsEmptyDict()
        {
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Product
                {
                    InstanceId = 1,
                    Name = "Test",
                    Description = "Desc",
                    Attributes = null
                });

            var controller = new ProductController(mockRepo.Object);

            var result = await controller.GetProductById(1, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ProductResponse>(okResult.Value);
            Assert.NotNull(response.Attributes);
            Assert.Empty(response.Attributes);
        }

        #endregion
    }
}
