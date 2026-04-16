using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Interview.DataEntities.Models;
using Moq;
using Sparcpoint.SqlServer.Abstractions;
using Xunit;

namespace Interview.Services.Tests
{
    public class InventoryTransactionServiceTests
    {
        private readonly Mock<ISqlExecutor> _mockSqlExecutor;
        private readonly InventoryTransactionService _inventoryService;

        public InventoryTransactionServiceTests()
        {
            _mockSqlExecutor = new Mock<ISqlExecutor>();
            _inventoryService = new InventoryTransactionService(_mockSqlExecutor.Object);
        }

        [Fact]
        public async Task RecordInventoryTransactionAsync_ShouldCallSqlExecutor()
        {
            // Arrange
            var request = new InventoryRequest { ProductInstanceId = 1, Quantity = 10, TypeCategory = "In" };
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(1);

            // Act
            var result = await _inventoryService.RecordInventoryTransactionAsync(request);

            // Assert
            Assert.Equal(1, result);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()), Times.Once);
        }

        [Fact]
        public async Task RecordInventoryTransactionsAsync_ShouldCallSqlExecutor()
        {
            // Arrange
            var requests = new List<InventoryRequest> { new InventoryRequest { ProductInstanceId = 1, Quantity = 10 } };
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()))
                .Returns(Task.CompletedTask);

            // Act
            await _inventoryService.RecordInventoryTransactionsAsync(requests);

            // Assert
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()), Times.Once);
        }

        [Fact]
        public async Task GetProductStockAsync_ShouldCallSqlExecutor()
        {
            // Arrange
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()))
                .ReturnsAsync(100m);

            // Act
            var result = await _inventoryService.GetProductStockAsync(1);

            // Assert
            Assert.Equal(100m, result);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()), Times.Once);
        }

        [Fact]
        public async Task GetStockByMetadataAsync_ShouldCallSqlExecutor()
        {
            // Arrange
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()))
                .ReturnsAsync(50m);

            // Act
            var result = await _inventoryService.GetStockByMetadataAsync("Color", "Red");

            // Assert
            Assert.Equal(50m, result);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()), Times.Once);
        }

        [Fact]
        public async Task UndoInventoryTransactionAsync_ShouldCallSqlExecutor()
        {
            // Arrange
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()))
                .Returns(Task.CompletedTask);

            // Act
            await _inventoryService.UndoInventoryTransactionAsync(123);

            // Assert
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()), Times.Once);
        }
    }
}
