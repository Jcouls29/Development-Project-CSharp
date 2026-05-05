// EVAL: In-memory fake implementation of IInventoryRepository.
// Mirrors the append-only transaction log pattern from the SQL implementation:
// positive quantity = add, negative = remove, CompletedTimestamp = undone.

using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Tests.Fakes
{
    /// <summary>
    /// In-memory implementation of IInventoryRepository for testing.
    /// </summary>
    public class InMemoryInventoryRepository : IInventoryRepository
    {
        private readonly ConcurrentDictionary<int, InventoryTransaction> _transactions = new();
        private readonly HashSet<int> _knownProductIds = new();
        private int _nextTransactionId = 1;

        // Product attributes for metadata-based count queries
        private readonly Dictionary<int, Dictionary<string, string>> _productAttributes = new();

        /// <summary>
        /// Registers a product ID so the repository knows it exists.
        /// </summary>
        public void RegisterProduct(int productId, Dictionary<string, string> attributes = null)
        {
            _knownProductIds.Add(productId);
            if (attributes != null)
                _productAttributes[productId] = attributes;
        }

        public Task<IEnumerable<InventoryTransaction>> AddInventoryAsync(
            IEnumerable<InventoryTransactionItem> items, CancellationToken cancellationToken = default)
        {
            return InsertTransactions(items, negate: false);
        }

        public Task<IEnumerable<InventoryTransaction>> RemoveInventoryAsync(
            IEnumerable<InventoryTransactionItem> items, CancellationToken cancellationToken = default)
        {
            return InsertTransactions(items, negate: true);
        }

        public Task<decimal?> GetCountByProductIdAsync(int productInstanceId, CancellationToken cancellationToken = default)
        {
            if (!_knownProductIds.Contains(productInstanceId))
                return Task.FromResult<decimal?>(null);

            var count = _transactions.Values
                .Where(t => t.ProductInstanceId == productInstanceId && t.IsActive)
                .Sum(t => t.Quantity);

            return Task.FromResult<decimal?>(count);
        }

        public Task<Dictionary<int, decimal>> GetCountByMetadataAsync(
            string attributeKey, string attributeValue, CancellationToken cancellationToken = default)
        {
            // Find products matching the attribute
            var matchingProductIds = _productAttributes
                .Where(kvp => kvp.Value.TryGetValue(attributeKey, out var val) && val == attributeValue)
                .Select(kvp => kvp.Key)
                .ToHashSet();

            var counts = _transactions.Values
                .Where(t => matchingProductIds.Contains(t.ProductInstanceId) && t.IsActive)
                .GroupBy(t => t.ProductInstanceId)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Quantity));

            return Task.FromResult(counts);
        }

        public Task<InventoryTransaction> UndoTransactionAsync(int transactionId, CancellationToken cancellationToken = default)
        {
            if (!_transactions.TryGetValue(transactionId, out var transaction))
                return Task.FromResult<InventoryTransaction>(null);

            if (!transaction.IsActive)
                return Task.FromResult(transaction);

            transaction.CompletedTimestamp = DateTime.UtcNow;
            return Task.FromResult(transaction);
        }

        private Task<IEnumerable<InventoryTransaction>> InsertTransactions(
            IEnumerable<InventoryTransactionItem> items, bool negate)
        {
            var results = new List<InventoryTransaction>();

            foreach (var item in items)
            {
                var transaction = new InventoryTransaction
                {
                    TransactionId = Interlocked.Increment(ref _nextTransactionId),
                    ProductInstanceId = item.ProductInstanceId,
                    Quantity = negate ? -item.Quantity : item.Quantity,
                    TypeCategory = item.TypeCategory,
                    StartedTimestamp = DateTime.UtcNow,
                    CompletedTimestamp = null
                };

                _transactions[transaction.TransactionId] = transaction;
                results.Add(transaction);
            }

            return Task.FromResult<IEnumerable<InventoryTransaction>>(results);
        }
    }
}
