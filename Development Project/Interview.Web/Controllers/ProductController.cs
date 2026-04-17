using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.DataEntities.Models;
using Interview.Services;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _ProductService;
        private readonly IInventoryTransactionService _InventoryService;

        public ProductController(IProductService productService, IInventoryTransactionService inventoryService)
        {
            _ProductService = productService ?? throw new ArgumentNullException(nameof(productService));
            _InventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var productId = await _ProductService.CreateProductAsync(request);
                return CreatedAtAction(nameof(CreateProduct), new { id = productId }, new { id = productId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts([FromBody] ProductSearchRequest request)
        {
            var results = await _ProductService.SearchProductsAsync(request);
            return Ok(results);
        }


        [HttpPost("inventory")]
        public async Task<IActionResult> RecordInventory([FromBody] InventoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transactionId = await _InventoryService.RecordInventoryTransactionAsync(request);
            return Ok(new { id = transactionId });
        }

        [HttpPost("inventory/bulk")]
        public async Task<IActionResult> RecordBulkInventory([FromBody] IEnumerable<InventoryRequest> requests)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _InventoryService.RecordInventoryTransactionsAsync(requests);
            return NoContent();
        }

        [HttpGet("{productId}/stock")]
        public async Task<IActionResult> GetProductStock(int productId)
        {
            var stock = await _InventoryService.GetProductStockAsync(productId);
            return Ok(new { productId, stock });
        }

        [HttpGet("stock/metadata")]
        public async Task<IActionResult> GetStockByMetadata([FromQuery] string key, [FromQuery] string value)
        {
            var stock = await _InventoryService.GetStockByMetadataAsync(key, value);
            return Ok(new { key, value, stock });
        }

        [HttpPost("inventory/undo/{transactionId}")]
        public async Task<IActionResult> UndoInventory(int transactionId)
        {
            await _InventoryService.UndoInventoryTransactionAsync(transactionId);
            return NoContent();
        }
    }
}
