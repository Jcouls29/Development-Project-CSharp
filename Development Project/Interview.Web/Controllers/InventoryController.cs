using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _InventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _InventoryService = inventoryService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddInventory([FromBody] InventoryTransactionRequest request)
        {
            var result = await _InventoryService.AddInventoryAsync(request);
            return Ok(result);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveInventory([FromBody] InventoryTransactionRequest request)
        {
            var result = await _InventoryService.RemoveInventoryAsync(request);
            return Ok(result);
        }

        [HttpPost("add/bulk")]
        public async Task<IActionResult> AddBulkInventory([FromBody] BulkInventoryRequest request)
        {
            var result = await _InventoryService.AddInventoryBulkAsync(request);
            return Ok(result);
        }

        [HttpPost("remove/bulk")]
        public async Task<IActionResult> RemoveBulkInventory([FromBody] BulkInventoryRequest request)
        {
            var result = await _InventoryService.RemoveInventoryBulkAsync(request);
            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetInventoryCount([FromQuery] int productInstanceId)
        {
            var result = await _InventoryService.GetInventoryCountAsync(productInstanceId);
            return Ok(result);
        }

        [HttpDelete("transactions/{id}")]
        public async Task<IActionResult> UndoTransaction(int id)
        {
            var result = await _InventoryService.UndoTransactionAsync(id);
            return Ok(result);
        }

        [HttpGet("count/by-metadata")]
        public async Task<ActionResult<InventoryCountByMetadataModel>> GetCountByMetadata([FromQuery] Dictionary<string, string> attributes)
        {
            var result = await _InventoryService.GetInventoryCountByMetadataAsync(attributes);
            return Ok(result);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactionHistory([FromQuery] int productInstanceId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            var result = await _InventoryService.GetTransactionHistoryAsync(productInstanceId, page, pageSize);
            return Ok(result);
        }
    }
}
