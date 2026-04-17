using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Interfaces;
using Sparcpoint.Models.Requests;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// Controller responsible for managing inventory operations.
    /// Provides functionality to retrieve inventory counts, transactions,
    /// and perform inventory updates such as add, remove, and undo operations.
    /// </summary>
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryController"/> class.
        /// </summary>
        /// <param name="inventoryRepository">Inventory repository dependency.</param>
        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        /// <summary>
        /// Retrieves the inventory count for a specific product.
        /// </summary>
        /// <param name="productId">Unique identifier of the product.</param>
        /// <returns>The total inventory count for the specified product.</returns>
        /// <response code="200">Returns the inventory count</response>
        [HttpGet("{productId}/count")]
        public async Task<IActionResult> GetCount(int productId)
        {
            var count = await _inventoryRepository.GetCountAsync(productId);
            return Ok(count);
        }

        /// <summary>
        /// Retrieves the inventory count filtered by a specific attribute.
        /// </summary>
        /// <param name="key">Attribute key (e.g., Color, Size).</param>
        /// <param name="value">Attribute value (e.g., Red, Large).</param>
        /// <returns>The inventory count matching the specified attribute.</returns>
        /// <remarks>
        /// Both key and value parameters are required.
        /// </remarks>
        /// <response code="200">Returns the inventory count</response>
        /// <response code="400">Invalid input parameters</response>
        [HttpGet("count")]
        public async Task<IActionResult> GetCountByAttribute(
            [FromQuery] string key,
            [FromQuery] string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return BadRequest("Key and Value are required");

            var count = await _inventoryRepository.GetCountByAttributeAsync(key, value);
            return Ok(count);
        }

        /// <summary>
        /// Retrieves all inventory transactions for a specific product.
        /// </summary>
        /// <param name="productId">Unique identifier of the product.</param>
        /// <returns>A list of inventory transactions.</returns>
        /// <response code="200">Returns the list of transactions</response>
        [HttpGet("{productId}/transactions")]
        public async Task<IActionResult> GetTransactions(int productId)
        {
            var transactions = await _inventoryRepository.GetTransactionsAsync(productId);
            return Ok(transactions);
        }

        /// <summary>
        /// Adds inventory for one or more product instances.
        /// </summary>
        /// <param name="request">Inventory update request containing product instances and quantity.</param>
        /// <returns>A success message if the inventory is updated correctly.</returns>
        /// <remarks>
        /// Quantity must be greater than zero and at least one product instance must be specified.
        /// </remarks>
        /// <response code="200">Inventory successfully added</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] InventoryUpdateRequest request)
        {
            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            if (request.ProductInstanceIds == null || request.ProductInstanceIds.Count == 0)
                return BadRequest("At least one product instance must be specified");

            await _inventoryRepository.AddAsync(request);
            return Ok("Inventory successfully added");
        }

        /// <summary>
        /// Removes inventory for one or more product instances.
        /// </summary>
        /// <param name="request">Inventory update request containing product instances and quantity.</param>
        /// <returns>A success message if the inventory is updated correctly.</returns>
        /// <remarks>
        /// Quantity must be greater than zero and at least one product instance must be specified.
        /// </remarks>
        /// <response code="200">Inventory successfully removed</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] InventoryUpdateRequest request)
        {
            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            if (request.ProductInstanceIds == null || request.ProductInstanceIds.Count == 0)
                return BadRequest("At least one product instance must be specified");

            await _inventoryRepository.RemoveAsync(request);
            return Ok("Inventory successfully removed");
        }

        /// <summary>
        /// Reverts a previously executed inventory transaction.
        /// </summary>
        /// <param name="transactionId">Unique identifier of the transaction to be undone.</param>
        /// <returns>A success message if the transaction is successfully reverted.</returns>
        /// <response code="200">Transaction successfully reverted</response>
        [HttpDelete("transactions/{transactionId}")]
        public async Task<IActionResult> UndoTransaction(int transactionId)
        {
            await _inventoryRepository.UndoTransactionAsync(transactionId);
            return Ok("Transaction successfully reverted");
        }
    }
}