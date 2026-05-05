// EVAL: Integration-style tests using InMemoryInventoryRepository.
// Tests the full inventory lifecycle: add, remove, count, undo,
// and metadata-based queries with seeded product data.

using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Tests.Fakes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Tests
{
    public class InventoryRepositoryTests
    {
        private readonly InMemoryInventoryRepository _repo;

        public InventoryRepositoryTests()
        {
            _repo = new InMemoryInventoryRepository();

            // Register products with attributes for metadata queries
            _repo.RegisterProduct(1, new Dictionary<string, string>
            {
                { "Brand", "Samsung" }, { "Color", "Black" }
            });
            _repo.RegisterProduct(2, new Dictionary<string, string>
            {
                { "Brand", "Apple" }, { "Color", "Space Black" }
            });
            _repo.RegisterProduct(3, new Dictionary<string, string>
            {
                { "Brand", "Nike" }, { "Color", "White" }
            });
        }

        #region AddInventoryAsync Tests

        [Fact]
        public async Task AddInventoryAsync_SingleItem_CreatesPositiveTransaction()
        {
            var items = new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 50, TypeCategory = "Purchase" }
            };

            var results = await _repo.AddInventoryAsync(items);

            Assert.Single(results);
            var tx = results.First();
            Assert.Equal(50, tx.Quantity);
            Assert.Equal("Purchase", tx.TypeCategory);
            Assert.True(tx.IsActive);
            Assert.True(tx.TransactionId > 0);
        }

        [Fact]
        public async Task AddInventoryAsync_BulkItems_CreatesMultipleTransactions()
        {
            var items = new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 10 },
                new() { ProductInstanceId = 2, Quantity = 20 },
                new() { ProductInstanceId = 3, Quantity = 30 }
            };

            var results = await _repo.AddInventoryAsync(items);

            Assert.Equal(3, results.Count());
            // Each gets a unique transaction ID
            Assert.Equal(3, results.Select(r => r.TransactionId).Distinct().Count());
        }

        #endregion

        #region RemoveInventoryAsync Tests

        [Fact]
        public async Task RemoveInventoryAsync_NegatesQuantity()
        {
            var items = new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 5, TypeCategory = "Sale" }
            };

            var results = await _repo.RemoveInventoryAsync(items);

            Assert.Single(results);
            Assert.Equal(-5, results.First().Quantity);
        }

        #endregion

        #region GetCountByProductIdAsync Tests

        [Fact]
        public async Task GetCountByProductIdAsync_NoTransactions_ReturnsZero()
        {
            var count = await _repo.GetCountByProductIdAsync(1);

            Assert.NotNull(count);
            Assert.Equal(0m, count.Value);
        }

        [Fact]
        public async Task GetCountByProductIdAsync_AfterAddAndRemove_ReturnsNetCount()
        {
            // Add 50, then remove 12
            await _repo.AddInventoryAsync(new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 50 }
            });
            await _repo.RemoveInventoryAsync(new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 12 }
            });

            var count = await _repo.GetCountByProductIdAsync(1);

            Assert.Equal(38m, count.Value);
        }

        [Fact]
        public async Task GetCountByProductIdAsync_UnknownProduct_ReturnsNull()
        {
            var count = await _repo.GetCountByProductIdAsync(99999);

            Assert.Null(count);
        }

        [Fact]
        public async Task GetCountByProductIdAsync_UndoneTransactions_ExcludedFromCount()
        {
            // Add 50, then add 25 (will be undone)
            await _repo.AddInventoryAsync(new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 50 }
            });
            var toUndo = await _repo.AddInventoryAsync(new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 25 }
            });

            // Undo the second transaction
            await _repo.UndoTransactionAsync(toUndo.First().TransactionId);

            var count = await _repo.GetCountByProductIdAsync(1);

            // Only the 50 counts, the undone 25 is excluded
            Assert.Equal(50m, count.Value);
        }

        #endregion

        #region GetCountByMetadataAsync Tests

        [Fact]
        public async Task GetCountByMetadataAsync_MatchingProducts_ReturnsCounts()
        {
            // Add inventory to Samsung (product 1) and Apple (product 2)
            await _repo.AddInventoryAsync(new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 40 },
                new() { ProductInstanceId = 2, Quantity = 20 }
            });

            // Query by Brand=Samsung -- should only return product 1
            var counts = await _repo.GetCountByMetadataAsync("Brand", "Samsung");

            Assert.Single(counts);
            Assert.Equal(40m, counts[1]);
        }

        [Fact]
        public async Task GetCountByMetadataAsync_NoMatch_ReturnsEmptyDictionary()
        {
            var counts = await _repo.GetCountByMetadataAsync("Brand", "NonExistent");

            Assert.NotNull(counts);
            Assert.Empty(counts);
        }

        #endregion

        #region UndoTransactionAsync Tests

        [Fact]
        public async Task UndoTransactionAsync_ActiveTransaction_SetsCompletedTimestamp()
        {
            var added = await _repo.AddInventoryAsync(new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 10 }
            });
            var txId = added.First().TransactionId;

            var undone = await _repo.UndoTransactionAsync(txId);

            Assert.NotNull(undone);
            Assert.False(undone.IsActive);
            Assert.NotNull(undone.CompletedTimestamp);
        }

        [Fact]
        public async Task UndoTransactionAsync_AlreadyUndone_ReturnsSameRecord()
        {
            var added = await _repo.AddInventoryAsync(new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 10 }
            });
            var txId = added.First().TransactionId;

            await _repo.UndoTransactionAsync(txId);
            var secondUndo = await _repo.UndoTransactionAsync(txId);

            // Returns the already-undone transaction
            Assert.NotNull(secondUndo);
            Assert.False(secondUndo.IsActive);
        }

        [Fact]
        public async Task UndoTransactionAsync_NonExistentId_ReturnsNull()
        {
            var result = await _repo.UndoTransactionAsync(99999);

            Assert.Null(result);
        }

        #endregion

        #region Full Lifecycle Test

        [Fact]
        public async Task FullLifecycle_AddRemoveCountUndo()
        {
            // 1. Add 100 units
            var purchase = await _repo.AddInventoryAsync(new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 100, TypeCategory = "Purchase" }
            });

            // 2. Sell 30
            await _repo.RemoveInventoryAsync(new List<InventoryTransactionItem>
            {
                new() { ProductInstanceId = 1, Quantity = 30, TypeCategory = "Sale" }
            });

            // 3. Count should be 70
            var count1 = await _repo.GetCountByProductIdAsync(1);
            Assert.Equal(70m, count1.Value);

            // 4. Undo the purchase (was a mistake)
            await _repo.UndoTransactionAsync(purchase.First().TransactionId);

            // 5. Count should be -30 (only the sale remains)
            var count2 = await _repo.GetCountByProductIdAsync(1);
            Assert.Equal(-30m, count2.Value);
        }

        #endregion
    }
}
