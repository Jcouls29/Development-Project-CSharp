using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models.Requests;
using System.Collections.Generic;
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
            _Inventory = inventory;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddInventory([FromBody] InventoryAdjustmentRequest request)
        {
            var transactionId = await _Inventory.AddAsync(request);
            return Ok(new { TransactionId = transactionId });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveInventory([FromBody] InventoryAdjustmentRequest request)
        {
            var transactionId = await _Inventory.RemoveAsync(request);
            return Ok(new { TransactionId = transactionId });
        }

        [HttpPost("add/batch")]
        public async Task<IActionResult> AddInventoryBatch([FromBody] IEnumerable<InventoryAdjustmentRequest> requests)
        {
            var transactionIds = await _Inventory.AddBatchAsync(requests);
            return Ok(new { TransactionIds = transactionIds });
        }

        [HttpPost("remove/batch")]
        public async Task<IActionResult> RemoveInventoryBatch([FromBody] IEnumerable<InventoryAdjustmentRequest> requests)
        {
            var transactionIds = await _Inventory.RemoveBatchAsync(requests);
            return Ok(new { TransactionIds = transactionIds });
        }

        [HttpGet("count/{productInstanceId:int}")]
        public async Task<IActionResult> GetCountByProduct(int productInstanceId)
        {
            var count = await _Inventory.GetCountAsync(productInstanceId);
            return Ok(new { ProductInstanceId = productInstanceId, Count = count });
        }

        // EVAL: POST body allows multiple attribute pairs — GET query strings can't cleanly express
        // a dictionary of arbitrary key/value pairs
        [HttpPost("count/metadata")]
        public async Task<IActionResult> GetCountByMetadata([FromBody] InventoryCountByMetadataRequest request)
        {
            var count = await _Inventory.GetCountByMetadataAsync(request);
            return Ok(new { Attributes = request.Attributes, Count = count });
        }

        [HttpDelete("transactions/{transactionId:int}")]
        public async Task<IActionResult> RemoveTransaction(int transactionId)
        {
            await _Inventory.RemoveTransactionAsync(transactionId);
            return NoContent();
        }
    }
}
