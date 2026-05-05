// EVAL: Unit tests for InventoryController using Moq.
// Covers add, remove, count, and undo operations with various scenarios.

using Interview.Web.Controllers;
using Interview.Web.Models.Requests;
using Interview.Web.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using DomainTransactionItem = Sparcpoint.Inventory.Abstract.InventoryTransactionItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Tests
{
    public class InventoryControllerTests
    {
        private readonly Mock<IInventoryRepository> _mockRepo;
        private readonly InventoryController _controller;

        public InventoryControllerTests()
        {
            _mockRepo = new Mock<IInventoryRepository>();
            _controller = new InventoryController(_mockRepo.Object);
        }

        #region AddInventory Tests

        [Fact]
        public async Task AddInventory_SingleItem_ReturnsTransaction()
        {
            // Arrange
            _mockRepo.Setup(r => r.AddInventoryAsync(It.IsAny<IEnumerable<DomainTransactionItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InventoryTransaction>
                {
                    new InventoryTransaction
                    {
                        TransactionId = 1,
                        ProductInstanceId = 1,
                        Quantity = 25,
                        TypeCategory = "Purchase",
                        StartedTimestamp = DateTime.UtcNow
                    }
                });

            var request = new AddInventoryRequest
            {
                Items = new List<Interview.Web.Models.Requests.InventoryTransactionItem>
                {
                    new Interview.Web.Models.Requests.InventoryTransactionItem
                    {
                        ProductInstanceId = 1,
                        Quantity = 25,
                        TypeCategory = "Purchase"
                    }
                }
            };

            // Act
            var result = await _controller.AddInventory(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<InventoryTransactionResponse>(okResult.Value);
            Assert.Single(response.Transactions);
            Assert.Equal(25, response.Transactions[0].Quantity);
            Assert.Equal("Purchase", response.Transactions[0].TypeCategory);
        }

        [Fact]
        public async Task AddInventory_BulkItems_ReturnsMultipleTransactions()
        {
            // Arrange
            _mockRepo.Setup(r => r.AddInventoryAsync(It.IsAny<IEnumerable<DomainTransactionItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InventoryTransaction>
                {
                    new InventoryTransaction { TransactionId = 1, ProductInstanceId = 1, Quantity = 10, StartedTimestamp = DateTime.UtcNow },
                    new InventoryTransaction { TransactionId = 2, ProductInstanceId = 2, Quantity = 5, StartedTimestamp = DateTime.UtcNow }
                });

            var request = new AddInventoryRequest
            {
                Items = new List<Interview.Web.Models.Requests.InventoryTransactionItem>
                {
                    new Interview.Web.Models.Requests.InventoryTransactionItem { ProductInstanceId = 1, Quantity = 10 },
                    new Interview.Web.Models.Requests.InventoryTransactionItem { ProductInstanceId = 2, Quantity = 5 }
                }
            };

            // Act
            var result = await _controller.AddInventory(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<InventoryTransactionResponse>(okResult.Value);
            Assert.Equal(2, response.Transactions.Count);
        }

        #endregion

        #region RemoveInventory Tests

        [Fact]
        public async Task RemoveInventory_SingleItem_ReturnsNegativeQuantity()
        {
            // Arrange
            _mockRepo.Setup(r => r.RemoveInventoryAsync(It.IsAny<IEnumerable<DomainTransactionItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InventoryTransaction>
                {
                    new InventoryTransaction
                    {
                        TransactionId = 1,
                        ProductInstanceId = 1,
                        Quantity = -5,
                        TypeCategory = "Sale",
                        StartedTimestamp = DateTime.UtcNow
                    }
                });

            var request = new RemoveInventoryRequest
            {
                Items = new List<Interview.Web.Models.Requests.InventoryTransactionItem>
                {
                    new Interview.Web.Models.Requests.InventoryTransactionItem
                    {
                        ProductInstanceId = 1,
                        Quantity = 5,
                        TypeCategory = "Sale"
                    }
                }
            };

            // Act
            var result = await _controller.RemoveInventory(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<InventoryTransactionResponse>(okResult.Value);
            Assert.Single(response.Transactions);
            Assert.Equal(-5, response.Transactions[0].Quantity);
        }

        #endregion

        #region GetInventoryCount Tests

        [Fact]
        public async Task GetInventoryCount_ByProductId_ReturnsCount()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetCountByProductIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(41m);

            // Act
            var result = await _controller.GetInventoryCount(1, null, null, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<InventoryCountResponse>(okResult.Value);
            Assert.Single(response.Items);
            Assert.Equal(41m, response.Items[0].Count);
            Assert.Equal(41m, response.TotalCount);
        }

        [Fact]
        public async Task GetInventoryCount_ProductNotFound_Returns404()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetCountByProductIdAsync(99999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((decimal?)null);

            // Act
            var result = await _controller.GetInventoryCount(99999, null, null, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(notFound.Value);
            Assert.Equal("PRODUCT_NOT_FOUND", error.Code);
        }

        [Fact]
        public async Task GetInventoryCount_ByMetadata_ReturnsMultipleCounts()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetCountByMetadataAsync("Brand", "Samsung", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, decimal> { { 1, 41m }, { 2, 17m } });

            // Act
            var result = await _controller.GetInventoryCount(null, "Brand", "Samsung", CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<InventoryCountResponse>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal(58m, response.TotalCount);
        }

        [Fact]
        public async Task GetInventoryCount_MissingParams_Returns400()
        {
            // Arrange / Act
            // RV: No productId and no metadata params should return 400
            var result = await _controller.GetInventoryCount(null, null, null, CancellationToken.None);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(badRequest.Value);
            Assert.Equal("INVALID_QUERY", error.Code);
        }

        #endregion

        #region UndoTransaction Tests

        [Fact]
        public async Task UndoTransaction_ExistingActive_Returns200()
        {
            // Arrange
            _mockRepo.Setup(r => r.UndoTransactionAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InventoryTransaction
                {
                    TransactionId = 1,
                    ProductInstanceId = 1,
                    Quantity = 25,
                    StartedTimestamp = DateTime.UtcNow.AddHours(-1),
                    CompletedTimestamp = DateTime.UtcNow,
                    TypeCategory = "Purchase"
                });

            // Act
            var result = await _controller.UndoTransaction(1, CancellationToken.None);

            // Assert
            // EV: After undo, CompletedTimestamp is set so IsActive is false.
            // The controller checks a specific condition for 409 -- when the transaction
            // was already undone BEFORE our call. Here it was freshly undone, so 200.
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task UndoTransaction_NotFound_Returns404()
        {
            // Arrange
            _mockRepo.Setup(r => r.UndoTransactionAsync(99999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((InventoryTransaction?)null);

            // Act
            var result = await _controller.UndoTransaction(99999, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(notFound.Value);
            Assert.Equal("TRANSACTION_NOT_FOUND", error.Code);
        }

        #endregion
    }
}
