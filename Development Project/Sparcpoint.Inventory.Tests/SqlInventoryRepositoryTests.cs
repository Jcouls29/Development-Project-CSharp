using Moq;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.SqlServer;
using Sparcpoint.SqlServer.Abstractions;
using System;
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
    }
}
