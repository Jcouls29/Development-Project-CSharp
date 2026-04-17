using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.Models;
using Sparcpoint.Application.Repositories;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryController(IInventoryRepository inventoryRepository = null)
        {
            _inventoryRepository = inventoryRepository;
        }

        [HttpPost("adjust")]
        public async Task<IActionResult> Adjust([FromBody] InventoryAdjustDto dto)
        {
            if (dto == null || dto.ProductId <= 0)
                return BadRequest();

            if (_inventoryRepository == null)
                return StatusCode(500, "Inventory repository not configured.");

            var txId = await _inventoryRepository.RecordTransactionAsync(dto.ProductId, dto.Quantity, dto.TransactionType ?? "Adjust");
            var resp = new InventoryTransactionResponseDto { TransactionId = txId, ProductInstanceId = dto.ProductId, Quantity = dto.Quantity };
            return Ok(resp);
        }

        [HttpPost("undo/{transactionId}")]
        public async Task<IActionResult> Undo(int transactionId)
        {
            if (transactionId <= 0)
                return BadRequest();

            if (_inventoryRepository == null)
                return StatusCode(500, "Inventory repository not configured.");

            try
            {
                var compensatingId = await _inventoryRepository.UndoTransactionAsync(transactionId);
                var resp = new InventoryTransactionResponseDto { TransactionId = compensatingId, ProductInstanceId = transactionId, Quantity = 0 };
                return Ok(new { Reversed = true, Compensating = resp });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
