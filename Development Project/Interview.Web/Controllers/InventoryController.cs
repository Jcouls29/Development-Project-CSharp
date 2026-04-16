using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Application.DTOs;
using Sparcpoint.Application.Interfaces;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] InventoryRequest request)
        {
            await _inventoryService.AddStock(request.ProductId, request.Quantity);
            return Ok();
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] InventoryRequest request)
        {
            await _inventoryService.RemoveStock(request.ProductId, request.Quantity);
            return Ok();
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetStock(int productId)
        {
            var stock = await _inventoryService.GetStock(productId);
            return Ok(new { stock });
        }

        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> Undo(int transactionId)
        {
            await _inventoryService.UndoTransaction(transactionId);
            return Ok();
        }
    }
}
