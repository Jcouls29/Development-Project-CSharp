using Interview.Web.Models;
using Interview.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventory;

        public InventoryController(IInventoryService inventory)
        {
            _inventory = inventory;
        }

        public class InventoryRequest { public int Delta { get; set; } public string Note { get; set; } }

        // EVAL: The ability to add and remove products from inventory 
        [HttpPost("{id:guid}")]
        public async Task<IActionResult> AddInventory(Guid id, [FromBody] InventoryRequest req)
        {
            if (req == null) return BadRequest();
            try
            {
                var t = await _inventory.AddInventoryAsync(id, req.Delta, req.Note);
                return Ok(t);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // EVAL: The ability to undo the last inventory transaction for a product.
        [HttpPost("{id:guid}/undo")]
        public async Task<IActionResult> UndoInventory(Guid id)
        {
            var ok = await _inventory.UndoLastInventoryAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("{id:guid}/count")]
        public async Task<IActionResult> GetQuantity(Guid id)
        {
            var q = await _inventory.GetQuantityAsync(id);
            if (q == null) return NotFound();
            return Ok(new { productId = id, quantity = q.Value });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetQuantityByCriteria([FromBody] Dictionary<string, string> metadata)
        {
            var sum = await _inventory.GetQuantityByMetadataAsync(metadata ?? new Dictionary<string, string>());
            return Ok(new { quantity = sum });
        }

        [HttpPost("batch")]
        public async Task<IActionResult> AddInventoryBatch([FromBody] IEnumerable<InventoryTransaction> txs)
        {
            if (txs == null) return BadRequest();
            try
            {
                var added = await _inventory.AddInventoryBatchAsync(txs);
                return Ok(added);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("transaction/{id:guid}")]
        public async Task<IActionResult> RemoveTransaction(Guid id)
        {
            var ok = await _inventory.RemoveTransactionAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
