using Moq;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.SqlServer;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class SqlInventoryRepositoryTests
    {
        [Fact]
        public void Constructor_ThrowsOnNullExecutor()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlInventoryRepository(null));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task AddAsync_ThrowsOnNonPositiveQuantity(decimal qty)
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlInventoryRepository(executor.Object);
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repo.AddAsync(1, qty));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task RemoveAsync_ThrowsOnNonPositiveQuantity(decimal qty)
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlInventoryRepository(executor.Object);
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repo.RemoveAsync(1, qty));
        }

        [Fact]
        public async Task AddBatchAsync_ThrowsOnNullItems()
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlInventoryRepository(executor.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddBatchAsync(null));
        }

        [Fact]
        public async Task RemoveBatchAsync_ThrowsOnNullItems()
        {
            var executor = new Mock<ISqlExecutor>();
            var repo = new SqlInventoryRepository(executor.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.RemoveBatchAsync(null));
        }

        [Fact]
        public async Task GetCountAsync_DelegatesToExecutorAndReturnsResult()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()))
                .ReturnsAsync(75.5m);

            var repo = new SqlInventoryRepository(mockExecutor.Object);
            var result = await repo.GetCountAsync(productInstanceId: 1);

            Assert.Equal(75.5m, result);
            mockExecutor.Verify(
                e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCountAsync_ReturnsZeroWhenExecutorReturnsZero()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()))
                .ReturnsAsync(0m);

            var repo = new SqlInventoryRepository(mockExecutor.Object);
            var result = await repo.GetCountAsync(productInstanceId: 99);

            Assert.Equal(0m, result);
        }

        [Fact]
        public async Task AddBatchAsync_WithEmptyList_DoesNotThrow()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()))
                .Returns(Task.CompletedTask);

            var repo = new SqlInventoryRepository(mockExecutor.Object);

            // EVAL: empty batch is valid - just a no-op, nothing gets inserted
            var exception = await Record.ExceptionAsync(() =>
                repo.AddBatchAsync(new List<InventoryBatchItem>()));

            Assert.Null(exception);
        }

        [Fact]
        public async Task RemoveBatchAsync_WithEmptyList_DoesNotThrow()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()))
                .Returns(Task.CompletedTask);

            var repo = new SqlInventoryRepository(mockExecutor.Object);

            var exception = await Record.ExceptionAsync(() =>
                repo.RemoveBatchAsync(new List<InventoryBatchItem>()));

            Assert.Null(exception);
        }

        [Fact]
        public async Task AddAsync_DelegatesToExecutorAndReturnsTransactionId()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(42);

            var repo = new SqlInventoryRepository(mockExecutor.Object);
            var transactionId = await repo.AddAsync(productInstanceId: 1, quantity: 10m);

            Assert.Equal(42, transactionId);
        }

        [Fact]
        public async Task RemoveAsync_DelegatesToExecutorAndReturnsTransactionId()
        {
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(99);

            var repo = new SqlInventoryRepository(mockExecutor.Object);
            var transactionId = await repo.RemoveAsync(productInstanceId: 2, quantity: 5m);

            Assert.Equal(99, transactionId);
        }
    }
}
