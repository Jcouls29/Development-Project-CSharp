using Interview.Web.Contracts;
using Interview.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryManagementService _inventoryManagementService;

        public InventoryController(IInventoryManagementService inventoryManagementService)
        {
            _inventoryManagementService = inventoryManagementService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<IReadOnlyList<InventoryTransactionResponse>>> AddInventory([FromBody] InventoryAdjustmentRequest request)
        {
            var transactions = await _inventoryManagementService.AddInventoryAsync(request);
            return Ok(transactions);
        }

        [HttpPost("remove")]
        public async Task<ActionResult<IReadOnlyList<InventoryTransactionResponse>>> RemoveInventory([FromBody] InventoryAdjustmentRequest request)
        {
            var transactions = await _inventoryManagementService.RemoveInventoryAsync(request);
            return Ok(transactions);
        }

        [HttpDelete("transactions/{transactionId:int}")]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            var deleted = await _inventoryManagementService.DeleteInventoryTransactionAsync(transactionId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPost("counts/query")]
        public async Task<ActionResult<InventoryCountResponse>> QueryInventoryCounts([FromBody] InventoryCountRequest request)
        {
            var counts = await _inventoryManagementService.GetInventoryCountsAsync(request);
            return Ok(counts);
        }
    }
}
