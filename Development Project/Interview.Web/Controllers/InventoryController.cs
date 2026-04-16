using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products/{id}/inventory")]
    public class InventoryController : Controller
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStock(int id)
        {
            var stock = await _inventoryRepository.GetStockAsync(id);
            return Ok(new InventoryResponse { ProductInstanceId = id, CurrentStock = stock });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] InventoryTransactionRequest request)
        {
            if (request == null)
                return BadRequest();

            request.ProductInstanceId = id;
            var transactionId = await _inventoryRepository.AddTransactionAsync(request);
            return Ok(new { TransactionId = transactionId });
        }

        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> RemoveTransaction(int transactionId)
        {
            var result = await _inventoryRepository.RemoveTransactionAsync(transactionId);
            if (result)
                return NoContent();

            return NotFound();
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetStockByMetadata([FromQuery] string key, [FromQuery] string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return BadRequest("Metadata key and value are required.");

            var stock = await _inventoryRepository.GetStockByMetadataAsync(key, value);
            return Ok(new { Key = key, Value = value, CurrentStock = stock });
        }
    }
}
