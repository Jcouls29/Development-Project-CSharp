using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Services;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// EVAL: Inventory API controller for managing stock levels.
    /// Inventory is tracked through transactions — each add/remove creates a transaction record.
    /// The "undo" capability soft-deletes a transaction, reversing its effect.
    /// Note: This controller does NOT delete products — only transactions can be undone.
    /// </summary>
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _InventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _InventoryService = inventoryService;
        }

        [HttpPost("{productId:int}/add")]
        public async Task<IActionResult> AddToInventory(int productId, [FromBody] InventoryTransactionRequest request)
        {
            var transaction = await _InventoryService.AddToInventoryAsync(productId, request.Quantity, request.TypeCategory);
            return Ok(InventoryTransactionResponse.FromTransaction(transaction));
        }

        [HttpPost("{productId:int}/remove")]
        public async Task<IActionResult> RemoveFromInventory(int productId, [FromBody] InventoryTransactionRequest request)
        {
            var transaction = await _InventoryService.RemoveFromInventoryAsync(productId, request.Quantity, request.TypeCategory);
            return Ok(InventoryTransactionResponse.FromTransaction(transaction));
        }

        /// <summary>
        /// EVAL: Undoes a specific inventory transaction via soft delete.
        /// </summary>
        [HttpDelete("transactions/{transactionId:int}")]
        public async Task<IActionResult> UndoTransaction(int transactionId)
        {
            var removed = await _InventoryService.UndoTransactionAsync(transactionId);
            if (!removed)
                return NotFound(new { error = $"Transaction with ID {transactionId} not found." });

            return NoContent();
        }

        [HttpGet("{productId:int}/count")]
        public async Task<IActionResult> GetInventoryCount(int productId)
        {
            var count = await _InventoryService.GetInventoryCountAsync(productId);
            return Ok(new InventoryCountResponse { ProductInstanceId = productId, Count = count });
        }

        /// <summary>
        /// EVAL: Retrieves inventory counts for all products matching a given attribute.
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> GetInventoryCountByAttribute([FromQuery] string key, [FromQuery] string value)
        {
            var results = await _InventoryService.GetInventoryCountsByAttributeAsync(key, value);
            return Ok(results.Select(r => new InventoryCountResponse
            {
                ProductInstanceId = r.ProductInstanceId,
                Count = r.Count
            }));
        }
    }
}
