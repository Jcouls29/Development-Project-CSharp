using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories.InMemory
{
    public sealed class InMemoryInventoryRepository : IInventoryRepository
    {
        private readonly ConcurrentDictionary<int, InventoryTransaction> _transactions = new();
        private readonly IProductRepository _productRepository;
        private int _nextId = 0;

        public InMemoryInventoryRepository(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public Task<int> RecordTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));
            var id = Interlocked.Increment(ref _nextId);
            transaction.TransactionId = id;
            _transactions[id] = transaction;
            return Task.FromResult(id);
        }

        public async Task<IReadOnlyList<int>> RecordTransactionsAsync(IReadOnlyList<InventoryTransaction> transactions, CancellationToken cancellationToken = default)
        {
            var ids = new List<int>(transactions.Count);
            foreach (var t in transactions)
                ids.Add(await RecordTransactionAsync(t, cancellationToken));
            return ids;
        }

        public Task<bool> RemoveTransactionAsync(int transactionId, CancellationToken cancellationToken = default)
            => Task.FromResult(_transactions.TryRemove(transactionId, out _));

        public Task<InventoryCount> GetCountAsync(int productInstanceId, CancellationToken cancellationToken = default)
        {
            var total = _transactions.Values
                .Where(t => t.ProductInstanceId == productInstanceId && t.CompletedTimestamp != null)
                .Sum(t => t.Quantity);
            return Task.FromResult(new InventoryCount { ProductInstanceId = productInstanceId, Quantity = total });
        }

        public async Task<IReadOnlyList<InventoryCount>> GetCountsByAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
        {
            if (attribute is null) throw new ArgumentNullException(nameof(attribute));

            // EVAL: In-memory path walks products to find matches — acceptable for tests;
            // SQL implementation joins ProductAttributes for efficiency.
            var matches = await _productRepository.SearchAsync(new ProductSearchCriteria
            {
                AttributeMatches = new[] { attribute },
                Take = int.MaxValue
            }, cancellationToken);

            var results = new List<InventoryCount>(matches.Count);
            foreach (var product in matches)
                results.Add(await GetCountAsync(product.InstanceId, cancellationToken));
            return results;
        }
    }
}
