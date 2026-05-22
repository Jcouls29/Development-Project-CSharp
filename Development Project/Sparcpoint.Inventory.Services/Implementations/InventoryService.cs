using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sparcpoint.Inventory.Models.Domain;
using Sparcpoint.Inventory.Models.DTOs.Requests;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Models.Search;
using Sparcpoint.Inventory.Repositories.Interfaces;
using Sparcpoint.Inventory.Services.Interfaces;

namespace Sparcpoint.Inventory.Services.Implementations
{
    /// <summary>
    /// Inventory service implementation.
    /// EVAL: Handles inventory add/remove operations with validation and business rules.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository,
            ILogger<InventoryService> logger)
        {
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<InventoryTransactionResponse>> AddInventoryAsync(AddInventoryRequest request)
        {
            // EVAL: Validate all products exist before creating transactions
            var productIds = request.Items.Select(i => i.ProductInstanceId).Distinct().ToList();
            await ValidateProductsExist(productIds);

            var transactions = request.Items.Select(item => new InventoryTransaction
            {
                ProductInstanceId = item.ProductInstanceId,
                Quantity = item.Quantity, // Positive for additions
                TypeCategory = item.TypeCategory ?? "ADD",
                StartedTimestamp = DateTime.UtcNow,
                CompletedTimestamp = DateTime.UtcNow // Auto-complete
            }).ToList();

            var created = await _inventoryRepository.AddInventoryAsync(transactions);
            
            _logger.LogInformation("Added inventory: {Count} transactions created", created.Count);
            
            return created.Select(MapToTransactionResponse).ToList();
        }

        public async Task<List<InventoryTransactionResponse>> RemoveInventoryAsync(RemoveInventoryRequest request)
        {
            // EVAL: Validate all products exist
            var productIds = request.Items.Select(i => i.ProductInstanceId).Distinct().ToList();
            await ValidateProductsExist(productIds);

            // EVAL: Convert to negative quantities for removal
            var transactions = request.Items.Select(item => new InventoryTransaction
            {
                ProductInstanceId = item.ProductInstanceId,
                Quantity = -Math.Abs(item.Quantity), // Ensure negative for removals
                TypeCategory = item.TypeCategory ?? "REMOVE",
                StartedTimestamp = DateTime.UtcNow,
                CompletedTimestamp = DateTime.UtcNow
            }).ToList();

            var created = await _inventoryRepository.AddInventoryAsync(transactions);
            
            _logger.LogInformation("Removed inventory: {Count} transactions created", created.Count);
            
            return created.Select(MapToTransactionResponse).ToList();
        }

        public async Task<bool> UndoTransactionAsync(int transactionId)
        {
            // EVAL: Supports requirement for ability to undo transactions
            var transaction = await _inventoryRepository.GetTransactionByIdAsync(transactionId);
            
            if (transaction == null)
            {
                _logger.LogWarning("Transaction {TransactionId} not found for undo", transactionId);
                return false;
            }

            var result = await _inventoryRepository.RemoveTransactionAsync(transactionId);
            
            if (result)
            {
                _logger.LogInformation("Transaction {TransactionId} undone successfully", transactionId);
            }
            
            return result;
        }

        public async Task<List<InventoryCountResponse>> GetInventoryCountAsync(InventoryCountCriteria criteria)
        {
            return await _inventoryRepository.GetInventoryCountAsync(criteria);
        }

        public async Task<InventoryTransactionResponse?> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _inventoryRepository.GetTransactionByIdAsync(transactionId);
            return transaction != null ? MapToTransactionResponse(transaction) : null;
        }

        #region Validation

        private async Task ValidateProductsExist(List<int> productIds)
        {
            foreach (var productId in productIds)
            {
                var product = await _productRepository.GetProductByIdAsync(productId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {productId} does not exist");
                }
            }
        }

        #endregion

        #region Mapping

        private InventoryTransactionResponse MapToTransactionResponse(InventoryTransaction transaction)
        {
            return new InventoryTransactionResponse
            {
                TransactionId = transaction.TransactionId,
                ProductInstanceId = transaction.ProductInstanceId,
                ProductName = transaction.ProductName ?? string.Empty,
                Quantity = transaction.Quantity,
                StartedTimestamp = transaction.StartedTimestamp,
                CompletedTimestamp = transaction.CompletedTimestamp,
                TypeCategory = transaction.TypeCategory
            };
        }

        #endregion
    }
}