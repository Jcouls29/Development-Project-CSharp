using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Models.DTOs;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;
        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        //EVAL: - 1. API to add products to the inventory
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] AddToInventoryRequestDto request)
        {
            try
            {
                var inventoryID = await _inventoryService.AddProductToInventoryAsync(request);
                return Ok(new { inventoryID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to inventory");
                return StatusCode(500, "An error occurred while adding to inventory.");
            }

        }

        //EVAL: - 2. API to remove products from inventory
        [HttpDelete("product/{productId}")]
        public async Task<IActionResult> RemoveProduct([FromRoute] int productId)
        {
            try
            {
                var rowsDeleted = await _inventoryService.RemoveProductFromInventoryAsync(productId);
                if(rowsDeleted == 0)
                {
                    return NotFound(new { Message = $"No inventory record found for Product Id {productId}." });
                }
                return Ok(new { Message = "Product deleted successfully from inventory" });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product from inventory for product Id : {ProductId}", productId);
                return StatusCode(500, "An error occurred while removing product from inventory.");
            }

        }
        //EVAL: 3- API to remove products from inventory
        [HttpDelete("transaction/{transactionId}")]
        public async Task<IActionResult> RemoveTransaction([FromRoute] int transactionId)
        {
            try
            {
                int rowsDeleted = await _inventoryService.RemoveInventoryTransactionAsync(transactionId);
                if (rowsDeleted == 0)
                {
                    return NotFound(new { Message = $"No inventory transaction found with ID {transactionId}." });
                }
                return Ok(new { Message = "Transaction deleted successfully from inventory" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing inventory transaction with ID: {TransactionID}", transactionId);
                return StatusCode(500, "An error occurred while removing inventory transaction.");
            }

        }

        //EVAL: 4- API to get inventory count for a product
        [HttpGet("product/{productId}/count")]
        public async Task<IActionResult> GetInventoryCount([FromRoute] int productId)
        {
            try
            {
                decimal inventoryCount = await _inventoryService.GetProuctInventoryCountAsync(productId);
                return Ok(new { inventoryCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory transaction count for Product ID: {ProductId}", productId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting inventory transaction count");
            }

        }
    }
}
