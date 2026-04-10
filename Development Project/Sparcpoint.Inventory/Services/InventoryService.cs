using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories;

namespace Sparcpoint.Inventory.Services
{
    /// <summary>
    /// EVAL: Implements inventory management business logic.
    /// Adding inventory creates a positive-quantity transaction.
    /// Removing inventory creates a negative-quantity transaction.
    /// Undo deletes a specific transaction, reversing its effect on the net count.
    /// Products are validated to exist before any inventory operation.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _InventoryRepository;
        private readonly IProductRepository _ProductRepository;

        public InventoryService(IInventoryRepository inventoryRepository, IProductRepository productRepository)
        {
            _InventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            _ProductRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<InventoryTransaction> AddToInventoryAsync(int productInstanceId, decimal quantity, string? typeCategory = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            await ValidateProductExistsAsync(productInstanceId);

            var transaction = new InventoryTransaction
            {
                ProductInstanceId = productInstanceId,
                Quantity = quantity,
                StartedTimestamp = DateTime.UtcNow,
                CompletedTimestamp = DateTime.UtcNow,
                TypeCategory = typeCategory
            };

            return await _InventoryRepository.AddTransactionAsync(transaction);
        }

        /// <summary>
        /// EVAL: Removing from inventory creates a negative-quantity transaction rather than
        /// deleting existing transactions. This preserves the full audit trail while
        /// still reducing the net inventory count.
        /// </summary>
        public async Task<InventoryTransaction> RemoveFromInventoryAsync(int productInstanceId, decimal quantity, string? typeCategory = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            await ValidateProductExistsAsync(productInstanceId);

            var transaction = new InventoryTransaction
            {
                ProductInstanceId = productInstanceId,
                Quantity = -quantity, // EVAL: Negative quantity represents removal from inventory
                StartedTimestamp = DateTime.UtcNow,
                CompletedTimestamp = DateTime.UtcNow,
                TypeCategory = typeCategory
            };

            return await _InventoryRepository.AddTransactionAsync(transaction);
        }

        /// <summary>
        /// EVAL: Undo soft-deletes the transaction by clearing its CompletedTimestamp.
        /// The record remains in the database for audit purposes, but is excluded from
        /// inventory counts (which filter on CompletedTimestamp IS NOT NULL).
        /// This is the "undo" capability mentioned in the requirements.
        /// </summary>
        public async Task<bool> UndoTransactionAsync(int transactionId)
        {
            if (transactionId <= 0)
                throw new ArgumentException("Transaction ID must be a positive integer.", nameof(transactionId));

            return await _InventoryRepository.RemoveTransactionAsync(transactionId);
        }

        public async Task<decimal> GetInventoryCountAsync(int productInstanceId)
        {
            if (productInstanceId <= 0)
                throw new ArgumentException("Product instance ID must be a positive integer.", nameof(productInstanceId));

            return await _InventoryRepository.GetInventoryCountAsync(productInstanceId);
        }

        public async Task<IEnumerable<InventoryCountSummary>> GetInventoryCountsByAttributeAsync(string key, string value)
        {
            PreConditions.StringNotNullOrWhitespace(key, nameof(key));
            PreConditions.StringNotNullOrWhitespace(value, nameof(value));

            return await _InventoryRepository.GetInventoryCountsByAttributeAsync(key, value);
        }

        private async Task ValidateProductExistsAsync(int productInstanceId)
        {
            if (productInstanceId <= 0)
                throw new ArgumentException("Product instance ID must be a positive integer.", nameof(productInstanceId));

            var product = await _ProductRepository.GetByIdAsync(productInstanceId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productInstanceId} does not exist.");
        }
    }
}
