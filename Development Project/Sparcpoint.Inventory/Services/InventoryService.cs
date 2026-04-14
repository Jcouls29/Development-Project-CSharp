using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Services
{
    public sealed class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<int> AddInventoryAsync(int productInstanceId, decimal quantity, CancellationToken cancellationToken = default)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive for an add.", nameof(quantity));
            return _repository.RecordTransactionAsync(new InventoryTransaction
            {
                ProductInstanceId = productInstanceId,
                Quantity = quantity,
                TypeCategory = InventoryTransactionTypes.Add,
                StartedTimestamp = DateTime.UtcNow,
                CompletedTimestamp = DateTime.UtcNow,
            }, cancellationToken);
        }

        public Task<int> RemoveInventoryAsync(int productInstanceId, decimal quantity, CancellationToken cancellationToken = default)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive; the transaction is recorded as a negative.", nameof(quantity));
            return _repository.RecordTransactionAsync(new InventoryTransaction
            {
                ProductInstanceId = productInstanceId,
                // EVAL: Remove is stored as a negative quantity so counts collapse to a simple SUM.
                Quantity = -quantity,
                TypeCategory = InventoryTransactionTypes.Remove,
                StartedTimestamp = DateTime.UtcNow,
                CompletedTimestamp = DateTime.UtcNow,
            }, cancellationToken);
        }

        public Task<IReadOnlyList<int>> AdjustInventoryBulkAsync(IReadOnlyList<(int ProductInstanceId, decimal Quantity)> adjustments, CancellationToken cancellationToken = default)
        {
            if (adjustments is null || adjustments.Count == 0) throw new ArgumentException("At least one adjustment is required.", nameof(adjustments));

            var transactions = new List<InventoryTransaction>(adjustments.Count);
            var now = DateTime.UtcNow;
            foreach (var (productId, qty) in adjustments)
            {
                if (qty == 0) throw new ArgumentException("Adjustment quantity cannot be zero.", nameof(adjustments));
                transactions.Add(new InventoryTransaction
                {
                    ProductInstanceId = productId,
                    Quantity = qty,
                    TypeCategory = qty > 0 ? InventoryTransactionTypes.Add : InventoryTransactionTypes.Remove,
                    StartedTimestamp = now,
                    CompletedTimestamp = now,
                });
            }

            return _repository.RecordTransactionsAsync(transactions, cancellationToken);
        }

        public Task<bool> UndoTransactionAsync(int transactionId, CancellationToken cancellationToken = default)
            => _repository.RemoveTransactionAsync(transactionId, cancellationToken);

        public Task<InventoryCount> GetCountAsync(int productInstanceId, CancellationToken cancellationToken = default)
            => _repository.GetCountAsync(productInstanceId, cancellationToken);

        public Task<IReadOnlyList<InventoryCount>> GetCountsByAttributeAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Attribute key is required.", nameof(key));
            if (value is null) throw new ArgumentNullException(nameof(value));
            return _repository.GetCountsByAttributeAsync(new ProductAttribute(key, value), cancellationToken);
        }
    }
}
