using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// Manages inventory transactions: adding stock, removing stock,
    /// undoing individual transactions, and querying counts.
    /// </summary>
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _Inventory;

        public InventoryController(IInventoryRepository inventory)
        {
            _Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        }

        /// <summary>
        /// Adds stock for a single product. Returns the new TransactionId.
        /// </summary>
        [HttpPost("{productId:int}/add")]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddInventory(
            [FromRoute] int productId,
            [FromBody] InventoryAdjustRequest request)
        {
            var transactionId = await _Inventory.AddAsync(productId, request.Quantity, request.TypeCategory);
            return StatusCode(201, transactionId);
        }

        /// <summary>
        /// Adds stock for multiple products in a single atomic operation.
        /// All items succeed or all roll back.
        /// </summary>
        [HttpPost("batch/add")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddInventoryBatch([FromBody] IEnumerable<InventoryBatchItem> items)
        {
            await _Inventory.AddBatchAsync(items);
            return NoContent();
        }

        /// <summary>
        /// Removes stock for a single product. Returns the new TransactionId.
        /// </summary>
        [HttpPost("{productId:int}/remove")]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RemoveInventory(
            [FromRoute] int productId,
            [FromBody] InventoryAdjustRequest request)
        {
            var transactionId = await _Inventory.RemoveAsync(productId, request.Quantity, request.TypeCategory);
            return StatusCode(201, transactionId);
        }

        /// <summary>
        /// Removes stock for multiple products in a single atomic operation.
        /// </summary>
        [HttpPost("batch/remove")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RemoveInventoryBatch([FromBody] IEnumerable<InventoryBatchItem> items)
        {
            await _Inventory.RemoveBatchAsync(items);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific transaction, effectively undoing its inventory effect.
        /// EVAL: deleting the row is the undo mechanism - cleaner than a compensating transaction
        /// since it leaves no audit noise for a user correction. Returns 404 if the transaction
        /// doesn't exist, caught by ApiExceptionFilter.
        /// </summary>
        [HttpDelete("transactions/{transactionId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTransaction([FromRoute] int transactionId)
        {
            await _Inventory.DeleteTransactionAsync(transactionId);
            return NoContent();
        }

        /// <summary>
        /// Returns the current net inventory count (sum of all completed transactions) for a product.
        /// </summary>
        [HttpGet("{productId:int}/count")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> GetCount([FromRoute] int productId)
        {
            var count = await _Inventory.GetCountAsync(productId);
            return Ok(count);
        }

        /// <summary>
        /// Returns inventory counts for all products matching the given filter.
        /// Useful for querying stock levels across a category or metadata subset.
        /// </summary>
        [HttpGet("count")]
        [ProducesResponseType(typeof(IEnumerable<ProductInventoryCount>), 200)]
        public async Task<IActionResult> GetCountByFilter(
            [FromQuery] string name = null,
            [FromQuery] int[] categoryIds = null,
            [FromQuery] string attributeKey = null,
            [FromQuery] string attributeValue = null)
        {
            var filter = new ProductSearchFilter
            {
                Name = name,
                CategoryIds = categoryIds?.Length > 0 ? categoryIds : null
            };

            if (!string.IsNullOrWhiteSpace(attributeKey) && !string.IsNullOrWhiteSpace(attributeValue))
            {
                filter.Attributes = new Dictionary<string, string>
                {
                    [attributeKey] = attributeValue
                };
            }

            var counts = await _Inventory.GetCountByFilterAsync(filter);
            return Ok(counts);
        }
    }

}
