using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Models.DTOs.Requests;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Models.Search;
using Sparcpoint.Inventory.Services.Interfaces;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// API Controller for inventory transaction management.
    /// EVAL: Supports bulk operations and transaction undo as per requirements.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Adds inventory for one or more products.
        /// EVAL: Supports requirement for bulk inventory operations.
        /// </summary>
        /// <param name="request">Inventory addition details</param>
        /// <returns>Created transactions</returns>
        [HttpPost("add")]
        [ProducesResponseType(typeof(List<InventoryTransactionResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<InventoryTransactionResponse>>> AddInventory([FromBody] AddInventoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transactions = await _inventoryService.AddInventoryAsync(request);
                return CreatedAtAction(nameof(GetInventoryCount), null, transactions);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Add inventory failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding inventory");
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        /// <summary>
        /// Removes inventory for one or more products.
        /// </summary>
        /// <param name="request">Inventory removal details</param>
        /// <returns>Created transactions</returns>
        [HttpPost("remove")]
        [ProducesResponseType(typeof(List<InventoryTransactionResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<InventoryTransactionResponse>>> RemoveInventory([FromBody] RemoveInventoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transactions = await _inventoryService.RemoveInventoryAsync(request);
                return CreatedAtAction(nameof(GetInventoryCount), null, transactions);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Remove inventory failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error removing inventory");
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        /// <summary>
        /// Removes a specific transaction (undo operation).
        /// EVAL: Supports requirement for ability to undo transactions.
        /// </summary>
        /// <param name="id">Transaction ID to undo</param>
        /// <returns>Success status</returns>
        [HttpDelete("transactions/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UndoTransaction(int id)
        {
            var result = await _inventoryService.UndoTransactionAsync(id);
            
            if (!result)
                return NotFound(new { error = $"Transaction {id} not found" });

            return NoContent();
        }

        /// <summary>
        /// Gets inventory counts based on criteria.
        /// EVAL: Supports requirement for retrieving counts by product or metadata.
        /// </summary>
        /// <param name="productId">Specific product ID</param>
        /// <param name="categoryIds">Category IDs (comma-separated)</param>
        /// <returns>Inventory count results</returns>
        [HttpGet("count")]
        [ProducesResponseType(typeof(List<InventoryCountResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<InventoryCountResponse>>> GetInventoryCount(
            [FromQuery] int? productId = null,
            [FromQuery] string? categoryIds = null)
        {
            var criteria = new InventoryCountCriteria
            {
                ProductInstanceId = productId,
                OnlyCompleted = true
            };

            if (!string.IsNullOrWhiteSpace(categoryIds))
            {
                criteria.CategoryIds = new List<int>();
                foreach (var id in categoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(id, out var categoryId))
                        criteria.CategoryIds.Add(categoryId);
                }
            }

            var results = await _inventoryService.GetInventoryCountAsync(criteria);
            return Ok(results);
        }

        /// <summary>
        /// Gets a specific transaction by ID.
        /// </summary>
        [HttpGet("transactions/{id}")]
        [ProducesResponseType(typeof(InventoryTransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryTransactionResponse>> GetTransaction(int id)
        {
            var transaction = await _inventoryService.GetTransactionByIdAsync(id);
            
            if (transaction == null)
                return NotFound(new { error = $"Transaction {id} not found" });

            return Ok(transaction);
        }
    }
}