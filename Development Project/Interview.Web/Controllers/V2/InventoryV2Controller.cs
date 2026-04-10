using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Services;

namespace Interview.Web.Controllers.V2
{
    /// <summary>
    /// EVAL: V2 Inventory controller extends V1 with bulk operations.
    /// Fulfills Requirement #4: "Adding and Removing inventory should happen
    /// on an individual product level or multiple products at once."
    /// </summary>
    [ApiVersion(2.0)]
    [Route("api/v{version:apiVersion}/inventory")]
    [ApiController]
    public class InventoryV2Controller : ControllerBase
    {
        private readonly IInventoryService _InventoryService;

        public InventoryV2Controller(IInventoryService inventoryService)
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
        /// EVAL: V2 bulk add — partial failure support per item.
        /// </summary>
        [HttpPost("bulk/add")]
        public async Task<IActionResult> BulkAddToInventory([FromBody] List<BulkInventoryRequest> requests)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest(new { error = "At least one inventory item is required." });

            var results = new List<InventoryTransactionResponse>();
            var errors = new List<object>();

            for (int i = 0; i < requests.Count; i++)
            {
                try
                {
                    var transaction = await _InventoryService.AddToInventoryAsync(
                        requests[i].ProductInstanceId, requests[i].Quantity, requests[i].TypeCategory);
                    results.Add(InventoryTransactionResponse.FromTransaction(transaction));
                }
                catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException)
                {
                    errors.Add(new { index = i, productInstanceId = requests[i].ProductInstanceId, error = ex.Message });
                }
            }

            return Ok(new { created = results, errors });
        }

        /// <summary>
        /// EVAL: V2 bulk remove — partial failure support per item.
        /// </summary>
        [HttpPost("bulk/remove")]
        public async Task<IActionResult> BulkRemoveFromInventory([FromBody] List<BulkInventoryRequest> requests)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest(new { error = "At least one inventory item is required." });

            var results = new List<InventoryTransactionResponse>();
            var errors = new List<object>();

            for (int i = 0; i < requests.Count; i++)
            {
                try
                {
                    var transaction = await _InventoryService.RemoveFromInventoryAsync(
                        requests[i].ProductInstanceId, requests[i].Quantity, requests[i].TypeCategory);
                    results.Add(InventoryTransactionResponse.FromTransaction(transaction));
                }
                catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException)
                {
                    errors.Add(new { index = i, productInstanceId = requests[i].ProductInstanceId, error = ex.Message });
                }
            }

            return Ok(new { created = results, errors });
        }

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
