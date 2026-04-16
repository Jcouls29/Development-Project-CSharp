using Microsoft.AspNetCore.Mvc;
using Sparcpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("{productInstanceId}/stock")]
        public async Task<IActionResult> GetStockCount(int productInstanceId)
        {
            var count = await _inventoryService.GetStockCountAsync(productInstanceId);
            return Ok(new { ProductInstanceId = productInstanceId, StockCount = count });
        }

        [HttpPost("bulk-update")]
        public async Task<IActionResult> SaveTransaction([FromBody] List<InventoryUpdate> request)
        {
            if (request == null || !request.Any()) return BadRequest("The list cannot be empty.");

            await _inventoryService.SaveTransactionAsync(request);
            return Ok("Transaction saved successfully.");
        }

        [HttpPost("undo/{transactionId}")]
        public async Task<IActionResult> UndoTransaction(Guid transactionId)
        {
            await _inventoryService.UndoTransactionAsync(transactionId);
            return Ok("Transaction undone successfully.");
        }
    }
}
