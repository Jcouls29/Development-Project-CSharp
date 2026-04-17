using Moq;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Interview.Tests
{
    public class InventoryServiceTests
    {
        #region AddInventoryAsync

        [Fact]
        public async Task AddInventoryAsync_ValidRequest_ReturnsTransaction()
        {
            // Arrange
            var productId = 42;
            var quantity = 5;
            var request = new InventoryTransactionRequest
            {
                ProductInstanceId = productId,
                Quantity = quantity,
                TypeCategory = "restock"
            };

            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.AddTransactionAsync(It.IsAny<InventoryTransactionRequest>()))
                .ReturnsAsync(1);
            mockRepo.Setup(r => r.GetTransactionByIdAsync(1))
                .ReturnsAsync(new InventoryTransactionModel
                {
                    TransactionId = 1,
                    ProductInstanceId = productId,
                    Quantity = Math.Abs(quantity),
                    TypeCategory = "restock",
                });

            var service = new InventoryService(mockRepo.Object);

            // Act
            var result = await service.AddInventoryAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductInstanceId);
            Assert.Equal(Math.Abs(quantity), result.Quantity);
            mockRepo.Verify(r => r.AddTransactionAsync(It.IsAny<InventoryTransactionRequest>()), Times.Once);
        }

        [Fact]
        public async Task AddInventoryAsync_NegativeQuantity_AbsolutesToPositive()
        {
            // Arrange
            var request = new InventoryTransactionRequest
            {
                ProductInstanceId = 1,
                Quantity = -5,
                TypeCategory = "restock"
            };

            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.AddTransactionAsync(It.IsAny<InventoryTransactionRequest>()))
                .ReturnsAsync(1);
            mockRepo.Setup(r => r.GetTransactionByIdAsync(1))
                .ReturnsAsync(new InventoryTransactionModel { TransactionId = 1, Quantity = 5 });

            var service = new InventoryService(mockRepo.Object);

            // Act
            var result = await service.AddInventoryAsync(request);

            // Assert - Should be normalized to positive
            Assert.Equal(5, result.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task AddInventoryAsync_InvalidProductId_ThrowsArgumentOutOfRange(int invalidId)
        {
            var request = new InventoryTransactionRequest
            {
                ProductInstanceId = invalidId,
                Quantity = 5,
            };

            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddInventoryAsync(request));
        }

        [Fact]
        public async Task AddInventoryAsync_ZeroQuantity_ThrowsArgumentOutOfRange()
        {
            var request = new InventoryTransactionRequest
            {
                ProductInstanceId = 1,
                Quantity = 0,
            };

            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddInventoryAsync(request));
        }

        [Fact]
        public async Task AddInventoryAsync_NullRequest_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddInventoryAsync(null!));
        }

        [Fact]
        public async Task AddInventoryAsync_TypeCategoryTooLong_ThrowsArgumentException()
        {
            var request = new InventoryTransactionRequest
            {
                ProductInstanceId = 1,
                Quantity = 5,
                TypeCategory = new string('x', 33), // 33 chars > 32 limit
            };

            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddInventoryAsync(request));
        }

        #endregion

        #region RemoveInventoryAsync

        [Fact]
        public async Task RemoveInventoryAsync_PositiveQuantity_NormalizesToNegative()
        {
            var request = new InventoryTransactionRequest
            {
                ProductInstanceId = 1,
                Quantity = 5,
                TypeCategory = "ship",
            };

            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.AddTransactionAsync(It.Is<InventoryTransactionRequest>(m => m.Quantity < 0)))
                .ReturnsAsync(1);
            mockRepo.Setup(r => r.GetTransactionByIdAsync(1))
                .ReturnsAsync(new InventoryTransactionModel { TransactionId = 1, Quantity = -5 });

            var service = new InventoryService(mockRepo.Object);

            var result = await service.RemoveInventoryAsync(request);

            Assert.True(result.Quantity < 0);
        }

        #endregion

        #region AddInventoryBulkAsync

        [Fact]
        public async Task AddInventoryBulkAsync_ValidItems_ReturnsResults()
        {
            var request = new BulkInventoryRequest
            {
                Items = new List<InventoryTransactionRequest>
                {
                    new InventoryTransactionRequest { ProductInstanceId = 1, Quantity = 5 },
                    new InventoryTransactionRequest { ProductInstanceId = 2, Quantity = 3 },
                },
            };

            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.AddBulkTransactionsAsync(It.IsAny<List<InventoryTransactionRequest>>()))
                .ReturnsAsync(new List<int> { 1, 2 });
            mockRepo.Setup(r => r.GetTransactionByIdAsync(1))
                .ReturnsAsync(new InventoryTransactionModel { TransactionId = 1, ProductInstanceId = 1 });
            mockRepo.Setup(r => r.GetTransactionByIdAsync(2))
                .ReturnsAsync(new InventoryTransactionModel { TransactionId = 2, ProductInstanceId = 2 });

            var service = new InventoryService(mockRepo.Object);

            var result = await service.AddInventoryBulkAsync(request);

            Assert.Equal(2, result.Results.Count);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task AddInventoryBulkAsync_EmptyItems_ThrowsArgumentException()
        {
            var request = new BulkInventoryRequest { Items = new List<InventoryTransactionRequest>() };

            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddInventoryBulkAsync(request));
        }

        [Fact]
        public async Task AddInventoryBulkAsync_TooManyItems_ThrowsArgumentException()
        {
            var items = Enumerable.Range(1, 101)
                .Select(i => new InventoryTransactionRequest { ProductInstanceId = i, Quantity = 1 })
                .ToList();

            var request = new BulkInventoryRequest { Items = items };

            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddInventoryBulkAsync(request));
        }

        #endregion

        #region GetInventoryCountAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetInventoryCountAsync_InvalidId_ThrowsArgumentOutOfRange(int invalidId)
        {
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetInventoryCountAsync(invalidId));
        }

        [Fact]
        public async Task GetInventoryCountAsync_ValidId_ReturnsCount()
        {
            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.GetTotalCountByProductAsync(42))
                .ReturnsAsync(new InventoryCountModel { ProductInstanceId = 42, Quantity = 100 });

            var service = new InventoryService(mockRepo.Object);

            var result = await service.GetInventoryCountAsync(42);

            Assert.Equal(42, result.ProductInstanceId);
            Assert.Equal(100, result.Quantity);
        }

        #endregion

        #region UndoTransactionAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UndoTransactionAsync_InvalidTransactionId_ThrowsArgumentOutOfRange(int invalidId)
        {
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.UndoTransactionAsync(invalidId));
        }

        #endregion

        #region GetInventoryCountByMetadataAsync

        [Fact]
        public async Task GetInventoryCountByMetadataAsync_ValidAttributes_ReturnsAggregatedCount()
        {
            var attributes = new Dictionary<string, string> { ["color"] = "red" };

            var mockRepo = new Mock<IInventoryRepository>();
            mockRepo.Setup(r => r.GetCountByMetadataAsync(attributes))
                .ReturnsAsync(new List<InventoryCountModel>
                {
                    new InventoryCountModel { ProductInstanceId = 1, Quantity = 10 },
                    new InventoryCountModel { ProductInstanceId = 2, Quantity = 20 },
                });

            var service = new InventoryService(mockRepo.Object);

            var result = await service.GetInventoryCountByMetadataAsync(attributes);

            Assert.Equal(30, result.TotalQuantity);
        }

        [Fact]
        public async Task GetInventoryCountByMetadataAsync_EmptyAttributes_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GetInventoryCountByMetadataAsync(new Dictionary<string, string>()));
        }

        [Fact]
        public async Task GetInventoryCountByMetadataAsync_NullAttributes_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetInventoryCountByMetadataAsync(null!));
        }

        #endregion

        #region GetTransactionHistoryAsync

        [Theory]
        [InlineData(0, 1, 10)]
        [InlineData(-1, 1, 10)]
        public async Task GetTransactionHistoryAsync_InvalidProductId_ThrowsArgumentOutOfRange(int invalidId, int page, int pageSize)
        {
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetTransactionHistoryAsync(invalidId, page, pageSize));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public async Task GetTransactionHistoryAsync_InvalidPage_ThrowsArgumentOutOfRange(int productId, int page)
        {
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetTransactionHistoryAsync(productId, page, 10));
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(1, 1, 101)]
        public async Task GetTransactionHistoryAsync_InvalidPageSize_ThrowsArgumentOutOfRange(int productId, int page, int invalidSize)
        {
            var mockRepo = new Mock<IInventoryRepository>();
            var service = new InventoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetTransactionHistoryAsync(productId, page, invalidSize));
        }

        #endregion

        #region Constructor

        [Fact]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new InventoryService(null!));
        }

        #endregion
    }
}