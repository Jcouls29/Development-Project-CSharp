using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;
using Sparcpoint.Core.Repositories;
using Sparcpoint.Core.Services;

namespace Sparcpoint.Core.Services.Implementations
{
    // EVAL: Service Implementation - Orchestrates inventory operations using ledger pattern
    // EVAL: Dependency Injection - Constructor injection for testability
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryTransactionRepository _transactionRepository;
        private readonly IProductRepository _productRepository;

        public InventoryService(IInventoryTransactionRepository transactionRepository, IProductRepository productRepository)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        // EVAL: Ledger pattern - Add inventory creates positive transaction
        // EVAL: Business rule validation - quantity must be positive
        public async Task<int> AddInventoryAsync(int productInstanceId, decimal quantity, string typeCategory = null)
        {
            // EVAL: Guard clause - prevent invalid operations
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive", nameof(quantity));

            // EVAL: Verify product exists
            var product = await _productRepository.GetByIdAsync(productInstanceId);
            if (product == null) throw new ArgumentException("Product not found", nameof(productInstanceId));

            // EVAL: Create transaction with positive quantity
            var transaction = InventoryTransaction.Create(productInstanceId, quantity, typeCategory ?? "ADD");

            // EVAL: Immediately complete the transaction (could be made asynchronous in production)
            transaction.Complete();

            // EVAL: Persist transaction to ledger
            return await _transactionRepository.AddAsync(transaction);
        }

        public async Task<IEnumerable<int>> AddInventoryAsync(IEnumerable<InventoryAdjustment> adjustments)
        {
            if (adjustments == null) throw new ArgumentNullException(nameof(adjustments));

            var transactionIds = new List<int>();
            foreach (var adjustment in adjustments)
            {
                if (adjustment.Quantity <= 0) throw new ArgumentException("Quantity must be positive", nameof(adjustment.Quantity));
                var product = await _productRepository.GetByIdAsync(adjustment.ProductInstanceId);
                if (product == null) throw new ArgumentException("Product not found", nameof(adjustment.ProductInstanceId));
                var transaction = InventoryTransaction.Create(adjustment.ProductInstanceId, adjustment.Quantity, adjustment.TypeCategory ?? "ADD");
                transaction.Complete();
                transactionIds.Add(await _transactionRepository.AddAsync(transaction));
            }

            return transactionIds;
        }

        // EVAL: Ledger pattern - Remove inventory creates negative transaction
        // EVAL: Business rule validation - quantity must be positive, sufficient inventory must exist
        public async Task<int> RemoveInventoryAsync(int productInstanceId, decimal quantity, string typeCategory = null)
        {
            // EVAL: Guard clause - prevent invalid operations
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive", nameof(quantity));

            // EVAL: Verify product exists
            var product = await _productRepository.GetByIdAsync(productInstanceId);
            if (product == null) throw new ArgumentException("Product not found", nameof(productInstanceId));

            // EVAL: Check current inventory level before allowing removal
            var currentInventory = await GetCurrentInventoryAsync(productInstanceId);
            if (currentInventory < quantity)
                throw new InvalidOperationException($"Insufficient inventory. Current: {currentInventory}, Requested: {quantity}");

            // EVAL: Create transaction with negative quantity
            var transaction = InventoryTransaction.Create(productInstanceId, -quantity, typeCategory ?? "REMOVE");

            // EVAL: Immediately complete the transaction
            transaction.Complete();

            // EVAL: Persist transaction to ledger
            return await _transactionRepository.AddAsync(transaction);
        }

        public async Task<IEnumerable<int>> RemoveInventoryAsync(IEnumerable<InventoryAdjustment> adjustments)
        {
            if (adjustments == null) throw new ArgumentNullException(nameof(adjustments));

            var transactionIds = new List<int>();
            foreach (var adjustment in adjustments)
            {
                if (adjustment.Quantity <= 0) throw new ArgumentException("Quantity must be positive", nameof(adjustment.Quantity));

                var product = await _productRepository.GetByIdAsync(adjustment.ProductInstanceId);
                if (product == null) throw new ArgumentException("Product not found", nameof(adjustment.ProductInstanceId));

                var currentInventory = await GetCurrentInventoryAsync(adjustment.ProductInstanceId);
                if (currentInventory < adjustment.Quantity)
                    throw new InvalidOperationException($"Insufficient inventory for product {adjustment.ProductInstanceId}. Current: {currentInventory}, Requested: {adjustment.Quantity}");

                var transaction = InventoryTransaction.Create(adjustment.ProductInstanceId, -adjustment.Quantity, adjustment.TypeCategory ?? "REMOVE");
                transaction.Complete();
                transactionIds.Add(await _transactionRepository.AddAsync(transaction));
            }

            return transactionIds;
        }

        // EVAL: Undo capability - critical for ledger pattern error correction
        // EVAL: Creates compensating transaction with opposite quantity
        public async Task<bool> UndoTransactionAsync(int transactionId)
        {
            // EVAL: Retrieve original transaction
            var originalTransaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (originalTransaction == null) return false;

            // EVAL: Only allow undo of completed transactions
            if (!originalTransaction.CompletedTimestamp.HasValue)
                throw new InvalidOperationException("Cannot undo uncompleted transaction");

            // EVAL: Create compensating transaction with negated quantity
            var undoTransaction = InventoryTransaction.Create(
                originalTransaction.ProductInstanceId,
                -originalTransaction.Quantity, // Negate the original quantity
                "UNDO"
            );

            // EVAL: Immediately complete the undo transaction
            undoTransaction.Complete();

            // EVAL: Persist compensating transaction
            await _transactionRepository.AddAsync(undoTransaction);

            return true;
        }

        // EVAL: Current inventory calculation - SUM of all completed ledger transactions
        // EVAL: Ledger pattern ensures accurate inventory tracking through immutable transaction history
        public async Task<decimal> GetCurrentInventoryAsync(int productInstanceId)
        {
            return await _productRepository.GetCurrentInventoryCountAsync(productInstanceId);
        }

        public async Task<decimal> GetInventoryCountByFilterAsync(string name = null, string description = null,
            IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null)
        {
            var products = await _productRepository.SearchAsync(name, description, categoryIds, metadataFilters, null, null);
            decimal total = 0;

            foreach (var product in products)
            {
                total += await GetCurrentInventoryAsync(product.InstanceId);
            }

            return total;
        }
    }
}