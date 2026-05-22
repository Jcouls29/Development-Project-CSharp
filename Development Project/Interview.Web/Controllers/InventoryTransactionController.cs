using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Services;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory-transactions")]
    public class InventoryTransactionController : Controller
    {
        private readonly IInventoryTransactionService _inventoryTransactionService;

        public InventoryTransactionController(IInventoryTransactionService inventoryTransactionService)
        {
            _inventoryTransactionService = inventoryTransactionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventoryTransaction([FromBody] InventoryTransactionCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            if (dto.Quantity == 0)
                return BadRequest("Quantity must not be zero.");

            var transaction = await _inventoryTransactionService.CreateInventoryTransaction(dto);
            if (transaction == null)
                return NotFound($"Product '{dto.ProductId}' was not found.");

            return Created(
                $"/api/v1/inventory-transactions/{transaction.TransactionId}",
                transaction);
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetInventoryTransactionsByProductId(Guid productId)
        {
            var transactions = await _inventoryTransactionService.GetInventoryTransactionsByProductId(productId);
            return Ok(transactions);
        }
    }
}
