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
        public async Task Record_ShouldInvokeExecutor_OnSingleTransaction()
        {
            var req = new InventoryRequest { ProductInstanceId = 1, Quantity = 10, TypeCategory = "In" };
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(1);

            var id = await _inventoryService.RecordInventoryTransactionAsync(req);

            Assert.Equal(1, id);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()), Times.Once);
        }

        [Fact]
        public async Task Record_ShouldInvokeExecutor_OnBatchRequest()
        {
            var batch = new List<InventoryRequest> { new InventoryRequest { ProductInstanceId = 1, Quantity = 10 } };
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()))
                .Returns(Task.CompletedTask);

            await _inventoryService.RecordInventoryTransactionsAsync(batch);

            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()), Times.Once);
        }

        [Fact]
        public async Task GetProductStock_ShouldReturnDataFromExecutor()
        {
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()))
                .ReturnsAsync(100m);

            var stock = await _inventoryService.GetProductStockAsync(1);

            Assert.Equal(100m, stock);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()), Times.Once);
        }

        [Fact]
        public async Task GetStockByMetadata_ShouldQueryExecutorWithParams()
        {
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()))
                .ReturnsAsync(50m);

            var stock = await _inventoryService.GetStockByMetadataAsync("Color", "Red");

            Assert.Equal(50m, stock);
            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()), Times.Once);
        }

        [Fact]
        public async Task Undo_ShouldUpdateTransactionState()
        {
            _mockSqlExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()))
                .Returns(Task.CompletedTask);

            await _inventoryService.UndoInventoryTransactionAsync(123);

            _mockSqlExecutor.Verify(x => x.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()), Times.Once);
        }
    }
}
