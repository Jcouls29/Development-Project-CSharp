// EVAL: InventoryController handles all inventory transaction operations.
// Separated from ProductController following single responsibility --
// products deal with catalog data, inventory deals with stock levels.

using Interview.Web.Models.Requests;
using Interview.Web.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// API endpoints for managing inventory transactions.
    /// </summary>
    [Route("api/v1/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;

        public InventoryController(IInventoryRepository inventoryRepository, IProductRepository productRepository)
        {
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        /// <summary>
        /// Adds inventory for one or more products.
        /// </summary>
        /// <param name="request">List of products and quantities to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Created transaction records with IDs for undo capability.</returns>
        /// <response code="200">Inventory added successfully.</response>
        /// <response code="400">Validation failed.</response>
        [HttpPost]
        [ProducesResponseType(typeof(InventoryTransactionResponse), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public async Task<IActionResult> AddInventory(
            [FromBody] AddInventoryRequest request,
            CancellationToken cancellationToken)
        {
            var items = request.Items.Select(i => new Sparcpoint.Inventory.Abstract.InventoryTransactionItem
            {
                ProductInstanceId = i.ProductInstanceId,
                Quantity = i.Quantity,
                TypeCategory = i.TypeCategory
            }).ToList();

            var missing = await GetMissingProductIdsAsync(items, cancellationToken);
            if (missing.Count > 0)
                return BadRequest(new ErrorResponse
                {
                    Code = "PRODUCT_NOT_FOUND",
                    Message = $"Product ID(s) not found: {string.Join(", ", missing)}"
                });

            var transactions = await _inventoryRepository.AddInventoryAsync(items, cancellationToken);
            return Ok(MapToResponse(transactions));
        }

        /// <summary>
        /// Removes inventory for one or more products.
        /// </summary>
        /// <param name="request">List of products and quantities to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Created transaction records with IDs for undo capability.</returns>
        /// <response code="200">Inventory removed successfully.</response>
        /// <response code="400">Validation failed.</response>
        // EVAL: Using POST for removal (not DELETE) because we're creating new transaction
        // records, not deleting a resource. The HTTP verb matches the server-side action.
        // EV: Consider whether to validate that removal won't cause negative inventory.
        // Current design allows negative counts (backorder scenario), which is common
        // in real inventory systems. Add a configuration flag if strict mode is needed.
        [HttpPost("remove")]
        [ProducesResponseType(typeof(InventoryTransactionResponse), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public async Task<IActionResult> RemoveInventory(
            [FromBody] RemoveInventoryRequest request,
            CancellationToken cancellationToken)
        {
            var items = request.Items.Select(i => new Sparcpoint.Inventory.Abstract.InventoryTransactionItem
            {
                ProductInstanceId = i.ProductInstanceId,
                Quantity = i.Quantity,
                TypeCategory = i.TypeCategory
            }).ToList();

            var missing = await GetMissingProductIdsAsync(items, cancellationToken);
            if (missing.Count > 0)
                return BadRequest(new ErrorResponse
                {
                    Code = "PRODUCT_NOT_FOUND",
                    Message = $"Product ID(s) not found: {string.Join(", ", missing)}"
                });

            var transactions = await _inventoryRepository.RemoveInventoryAsync(items, cancellationToken);
            return Ok(MapToResponse(transactions));
        }

        /// <summary>
        /// Retrieves inventory count by product ID or metadata attribute.
        /// </summary>
        /// <param name="productId">Filter by specific product ID.</param>
        /// <param name="metadataKey">Filter by metadata key (requires metadataValue).</param>
        /// <param name="metadataValue">Filter by metadata value (requires metadataKey).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inventory counts for matching products.</returns>
        /// <response code="200">Count retrieved successfully.</response>
        /// <response code="400">Invalid query parameters.</response>
        /// <response code="404">Product not found (when filtering by productId).</response>
        // EVAL: Single endpoint with flexible query params supports both use cases
        // from Requirement 5: count by product ID or by metadata.
        [HttpGet("count")]
        [ProducesResponseType(typeof(InventoryCountResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetInventoryCount(
            [FromQuery] int? productId,
            [FromQuery] string metadataKey,
            [FromQuery] string metadataValue,
            CancellationToken cancellationToken)
        {
            // RV: Must provide either productId or both metadataKey+metadataValue
            if (productId == null && (string.IsNullOrWhiteSpace(metadataKey) || string.IsNullOrWhiteSpace(metadataValue)))
            {
                return BadRequest(new ErrorResponse
                {
                    Code = "INVALID_QUERY",
                    Message = "Provide either 'productId' or both 'metadataKey' and 'metadataValue'."
                });
            }

            var response = new InventoryCountResponse();

            if (productId.HasValue)
            {
                // Count for a single product
                var count = await _inventoryRepository.GetCountByProductIdAsync(productId.Value, cancellationToken);

                if (count == null)
                    return NotFound(new ErrorResponse
                    {
                        Code = "PRODUCT_NOT_FOUND",
                        Message = $"Product with ID {productId.Value} was not found."
                    });

                var product = await _productRepository.GetByIdAsync(productId.Value, cancellationToken);

                response.Items.Add(new ProductInventoryCount
                {
                    ProductInstanceId = productId.Value,
                    ProductName = product?.Name,
                    Count = count.Value
                });
                response.TotalCount = count.Value;
            }
            else
            {
                // Count by metadata
                var counts = await _inventoryRepository.GetCountByMetadataAsync(
                    metadataKey, metadataValue, cancellationToken);

                IDictionary<int, Sparcpoint.Inventory.Models.Product> products =
                    counts.Count == 0
                        ? new Dictionary<int, Sparcpoint.Inventory.Models.Product>()
                        : await _productRepository.GetByIdsAsync(counts.Keys, cancellationToken);

                foreach (var kvp in counts)
                {
                    products.TryGetValue(kvp.Key, out var product);

                    response.Items.Add(new ProductInventoryCount
                    {
                        ProductInstanceId = kvp.Key,
                        ProductName = product?.Name,
                        Count = kvp.Value
                    });
                }
                response.TotalCount = counts.Values.Sum();
            }

            return Ok(response);
        }

        /// <summary>
        /// Retrieves inventory transactions, optionally filtered by product and active state.
        /// Useful for finding a transactionId to pass to DELETE /inventory/transactions/{id}.
        /// </summary>
        /// <param name="productId">Optional product ID filter.</param>
        /// <param name="activeOnly">If true (default), excludes already-undone transactions.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Matching transactions, newest first.</returns>
        /// <response code="200">Transactions retrieved successfully.</response>
        [HttpGet("transactions")]
        [ProducesResponseType(typeof(InventoryTransactionResponse), 200)]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int? productId,
            [FromQuery] bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var transactions = await _inventoryRepository.GetTransactionsAsync(
                productId, activeOnly, cancellationToken);

            return Ok(MapToResponse(transactions));
        }

        /// <summary>
        /// Undoes a specific inventory transaction by marking it as completed.
        /// The transaction remains in the log for audit but is excluded from inventory counts.
        /// </summary>
        /// <param name="transactionId">The transaction ID to undo.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated transaction record.</returns>
        /// <response code="200">Transaction undone successfully.</response>
        /// <response code="404">Transaction not found.</response>
        /// <response code="409">Transaction was already undone.</response>
        // EVAL: DELETE verb is appropriate here because we're logically removing
        // the transaction's effect from inventory counts (even though the row persists).
        [HttpDelete("transactions/{transactionId:int}")]
        [ProducesResponseType(typeof(TransactionDetail), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> UndoTransaction(
            int transactionId,
            CancellationToken cancellationToken)
        {
            var transaction = await _inventoryRepository.UndoTransactionAsync(transactionId, cancellationToken);

            if (transaction == null)
                return NotFound(new ErrorResponse
                {
                    Code = "TRANSACTION_NOT_FOUND",
                    Message = $"Transaction with ID {transactionId} was not found."
                });

            // RV: If the transaction was already undone before our call, return 409 Conflict.
            // The repository returns the existing record; we check IsActive here.
            // Note: If CompletedTimestamp was already set before we called Undo,
            // the repository skips the UPDATE and returns the existing record.
            if (!transaction.IsActive && transaction.CompletedTimestamp.HasValue)
            {
                // EV: Returning 409 vs 200 for already-undone is a design choice.
                // 409 signals to the client that no change occurred, which is more precise
                // than silently returning 200.
                return Conflict(new ErrorResponse
                {
                    Code = "TRANSACTION_ALREADY_UNDONE",
                    Message = $"Transaction {transactionId} was already undone at {transaction.CompletedTimestamp:O}."
                });
            }

            return Ok(new TransactionDetail
            {
                TransactionId = transaction.TransactionId,
                ProductInstanceId = transaction.ProductInstanceId,
                Quantity = transaction.Quantity,
                TypeCategory = transaction.TypeCategory,
                StartedTimestamp = transaction.StartedTimestamp,
                IsActive = transaction.IsActive
            });
        }

        #region Private Helpers

        private InventoryTransactionResponse MapToResponse(IEnumerable<Sparcpoint.Inventory.Models.InventoryTransaction> transactions)
        {
            return new InventoryTransactionResponse
            {
                Transactions = transactions.Select(t => new TransactionDetail
                {
                    TransactionId = t.TransactionId,
                    ProductInstanceId = t.ProductInstanceId,
                    Quantity = t.Quantity,
                    TypeCategory = t.TypeCategory,
                    StartedTimestamp = t.StartedTimestamp,
                    IsActive = t.IsActive
                }).ToList()
            };
        }

        private async Task<List<int>> GetMissingProductIdsAsync(
            IEnumerable<Sparcpoint.Inventory.Abstract.InventoryTransactionItem> items,
            CancellationToken cancellationToken)
        {
            var requestedIds = items.Select(i => i.ProductInstanceId).Distinct().ToList();
            if (requestedIds.Count == 0)
                return new List<int>();

            var existing = await _productRepository.GetByIdsAsync(requestedIds, cancellationToken);
            return requestedIds.Where(id => !existing.ContainsKey(id)).ToList();
        }

        #endregion Private Helpers
    }
}