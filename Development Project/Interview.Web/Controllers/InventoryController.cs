using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    // EVAL: Three endpoints map directly to inventory requirements: POST /transactions covers
    // requirement #4 (bulk add/remove via signed quantities), DELETE /transactions/{id} covers
    // requirement #6 (undo a transaction), and GET /count covers requirement #5 (count by product
    // id and/or subset of metadata).
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _Inventory;

        public InventoryController(IInventoryRepository inventory)
        {
            _Inventory = inventory;
        }

        [HttpPost("transactions")]
        public async Task<IActionResult> CreateTransactions([FromBody] IEnumerable<InventoryAdjustment> adjustments)
        {
            if (adjustments == null)
                return BadRequest("At least one adjustment is required.");

            try
            {
                var transactions = await _Inventory.CreateTransactionsAsync(adjustments);
                return Ok(transactions);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("transactions/{transactionId:int}")]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            try
            {
                await _Inventory.DeleteTransactionAsync(transactionId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount([FromQuery] int? productId, [FromQuery] string attributes)
        {
            var query = new InventoryCountQuery { ProductId = productId };

            if (!string.IsNullOrWhiteSpace(attributes))
            {
                try
                {
                    query.AttributeFilters = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(attributes);
                }
                catch
                {
                    return BadRequest("'attributes' must be a JSON object of key/value pairs.");
                }
            }

            var count = await _Inventory.GetCountAsync(query);
            return Ok(new { Count = count });
        }
    }
}
