using Moq;
using Sparcpoint.Inventory.Implementations;
using Sparcpoint.Inventory.Models.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class InventoryRepositoryTests
    {
        private readonly Mock<ISqlExecutor> _MockExecutor;
        private readonly SqlInventoryRepository _Repository;

        public InventoryRepositoryTests()
        {
            _MockExecutor = new Mock<ISqlExecutor>();
            _Repository = new SqlInventoryRepository(_MockExecutor.Object);
        }

        [Fact]
        public void Constructor_NullExecutor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlInventoryRepository(null));
        }

        [Fact]
        public async Task AddAsync_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Repository.AddAsync(null));
        }

        [Fact]
        public async Task AddAsync_ZeroQuantity_ThrowsArgumentException()
        {
            var request = new InventoryAdjustmentRequest { ProductInstanceId = 1, Quantity = 0 };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.AddAsync(request));
        }

        [Fact]
        public async Task AddAsync_NegativeQuantity_ThrowsArgumentException()
        {
            var request = new InventoryAdjustmentRequest { ProductInstanceId = 1, Quantity = -5 };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.AddAsync(request));
        }

        [Fact]
        public async Task AddAsync_ValidRequest_DelegatesToExecutor()
        {
            _MockExecutor
                .Setup(e => e.ExecuteAsync<int>(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(42);

            var request = new InventoryAdjustmentRequest { ProductInstanceId = 1, Quantity = 10 };
            var result = await _Repository.AddAsync(request);

            Assert.Equal(42, result);
        }

        [Fact]
        public async Task RemoveAsync_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Repository.RemoveAsync(null));
        }

        [Fact]
        public async Task RemoveAsync_ZeroQuantity_ThrowsArgumentException()
        {
            var request = new InventoryAdjustmentRequest { ProductInstanceId = 1, Quantity = 0 };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.RemoveAsync(request));
        }

        [Fact]
        public async Task RemoveAsync_ValidRequest_DelegatesToExecutor()
        {
            _MockExecutor
                .Setup(e => e.ExecuteAsync<int>(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(99);

            var request = new InventoryAdjustmentRequest { ProductInstanceId = 1, Quantity = 5 };
            var result = await _Repository.RemoveAsync(request);

            Assert.Equal(99, result);
        }

        [Fact]
        public async Task GetCountByMetadataAsync_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Repository.GetCountByMetadataAsync(null));
        }

        [Fact]
        public async Task GetCountByMetadataAsync_EmptyAttributes_ThrowsArgumentException()
        {
            var request = new InventoryCountByMetadataRequest();
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.GetCountByMetadataAsync(request));
        }

        [Fact]
        public async Task GetCountByMetadataAsync_ValidRequest_DelegatesToExecutor()
        {
            _MockExecutor
                .Setup(e => e.ExecuteAsync<decimal>(It.IsAny<Func<IDbConnection, IDbTransaction, Task<decimal>>>()))
                .ReturnsAsync(15m);

            var request = new InventoryCountByMetadataRequest
            {
                Attributes = new System.Collections.Generic.Dictionary<string, string> { { "Color", "Red" }, { "Brand", "Acme" } }
            };
            var result = await _Repository.GetCountByMetadataAsync(request);

            Assert.Equal(15m, result);
        }

        [Fact]
        public async Task AddBatchAsync_NullRequests_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Repository.AddBatchAsync(null));
        }

        [Fact]
        public async Task AddBatchAsync_EmptyList_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.AddBatchAsync(new List<InventoryAdjustmentRequest>()));
        }

        [Fact]
        public async Task AddBatchAsync_AnyItemWithZeroQuantity_ThrowsArgumentException()
        {
            var requests = new List<InventoryAdjustmentRequest>
            {
                new InventoryAdjustmentRequest { ProductInstanceId = 1, Quantity = 5 },
                new InventoryAdjustmentRequest { ProductInstanceId = 2, Quantity = 0 }
            };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.AddBatchAsync(requests));
        }

        [Fact]
        public async Task AddBatchAsync_ValidRequests_DelegatesToExecutor()
        {
            _MockExecutor
                .Setup(e => e.ExecuteAsync<IEnumerable<int>>(It.IsAny<Func<IDbConnection, IDbTransaction, Task<IEnumerable<int>>>>()))
                .ReturnsAsync(new[] { 1, 2 });

            var requests = new List<InventoryAdjustmentRequest>
            {
                new InventoryAdjustmentRequest { ProductInstanceId = 1, Quantity = 5 },
                new InventoryAdjustmentRequest { ProductInstanceId = 2, Quantity = 10 }
            };
            var result = await _Repository.AddBatchAsync(requests);

            Assert.Equal(new[] { 1, 2 }, result);
        }

        [Fact]
        public async Task RemoveBatchAsync_NullRequests_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Repository.RemoveBatchAsync(null));
        }

        [Fact]
        public async Task RemoveBatchAsync_EmptyList_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.RemoveBatchAsync(new List<InventoryAdjustmentRequest>()));
        }

        [Fact]
        public async Task RemoveBatchAsync_AnyItemWithZeroQuantity_ThrowsArgumentException()
        {
            var requests = new List<InventoryAdjustmentRequest>
            {
                new InventoryAdjustmentRequest { ProductInstanceId = 1, Quantity = 0 }
            };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.RemoveBatchAsync(requests));
        }

        // This test is expected to fail: whitespace keys pass the initial .Any() check but are
        // silently filtered out inside the implementation, leaving no JOINs and querying all
        // inventory instead of throwing. The fix is to validate after filtering, not before.
        [Fact]
        public async Task GetCountByMetadataAsync_AllWhitespaceKeys_ShouldThrowArgumentException()
        {
            var request = new InventoryCountByMetadataRequest
            {
                Attributes = new System.Collections.Generic.Dictionary<string, string> { { "   ", "Red" } }
            };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.GetCountByMetadataAsync(request));
        }

        [Fact]
        public async Task RemoveTransactionAsync_DelegatesToExecutor()
        {
            _MockExecutor
                .Setup(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()))
                .Returns(Task.CompletedTask);

            await _Repository.RemoveTransactionAsync(1);

            _MockExecutor.Verify(e => e.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>()), Times.Once);
        }
    }
}
