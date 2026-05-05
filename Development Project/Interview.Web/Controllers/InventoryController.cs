using Interview.Web.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _Inventory;

        public InventoryController(IInventoryRepository inventory)
        {
            _Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        }

        /// <summary>POST /api/v1/inventory/add — add a quantity of a single product to inventory</summary>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ModifyInventoryRequest request)
        {
            if (request == null || request.Quantity <= 0)
                return BadRequest("Quantity must be positive.");

            int transactionId = await _Inventory.AddAsync(
                request.ProductInstanceId, request.Quantity, request.TypeCategory);

            return Ok(new { TransactionId = transactionId });
        }

        /// <summary>POST /api/v1/inventory/add/bulk — add inventory for multiple products atomically</summary>
        [HttpPost("add/bulk")]
        public async Task<IActionResult> AddBulk([FromBody] BulkModifyInventoryRequest request)
        {
            if (request?.Items == null || !request.Items.Any())
                return BadRequest("At least one item is required.");

            if (request.Items.Any(i => i.Quantity <= 0))
                return BadRequest("All quantities must be positive.");

            await _Inventory.AddBulkAsync(
                request.Items.Select(i => (i.ProductInstanceId, i.Quantity, i.TypeCategory)));

            return Ok();
        }

        /// <summary>POST /api/v1/inventory/remove — remove a quantity from a single product</summary>
        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] ModifyInventoryRequest request)
        {
            if (request == null || request.Quantity <= 0)
                return BadRequest("Quantity must be positive.");

            await _Inventory.RemoveAsync(
                request.ProductInstanceId, request.Quantity, request.TypeCategory);

            return Ok();
        }

        /// <summary>POST /api/v1/inventory/remove/bulk — remove inventory for multiple products atomically</summary>
        [HttpPost("remove/bulk")]
        public async Task<IActionResult> RemoveBulk([FromBody] BulkModifyInventoryRequest request)
        {
            if (request?.Items == null || !request.Items.Any())
                return BadRequest("At least one item is required.");

            if (request.Items.Any(i => i.Quantity <= 0))
                return BadRequest("All quantities must be positive.");

            await _Inventory.RemoveBulkAsync(
                request.Items.Select(i => (i.ProductInstanceId, i.Quantity, i.TypeCategory)));

            return Ok();
        }

        /// <summary>DELETE /api/v1/inventory/transactions/{id} — remove an individual transaction ("undo")</summary>
        [HttpDelete("transactions/{transactionId:int}")]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            await _Inventory.DeleteTransactionAsync(transactionId);
            return NoContent();
        }

        /// <summary>GET /api/v1/inventory/count/{productId} — current inventory count for a product</summary>
        [HttpGet("count/{productId:int}")]
        public async Task<IActionResult> GetCount(int productId)
        {
            decimal count = await _Inventory.GetCountAsync(productId);
            return Ok(new { ProductInstanceId = productId, Count = count });
        }

        /// <summary>POST /api/v1/inventory/count/by-attributes — count across all products matching the given metadata</summary>
        [HttpPost("count/by-attributes")]
        public async Task<IActionResult> GetCountByAttributes([FromBody] GetCountByAttributesRequest request)
        {
            if (request?.Attributes == null || !request.Attributes.Any())
                return BadRequest("At least one attribute filter is required.");

            decimal count = await _Inventory.GetCountByAttributesAsync(request.Attributes);
            return Ok(new { Count = count });
        }
    }
}
