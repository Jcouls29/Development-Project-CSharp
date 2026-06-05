using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Controllers;

[ApiController]
[Route("api/v1/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Add an inventory transaction for a product.
    /// </summary>
    /// <remarks>
    /// Use a positive Quantity to add stock and a negative Quantity to remove stock.
    /// Each call creates a new transaction row which can be individually undone via DELETE.
    /// The optional TypeCategory field can label the transaction (e.g. "RECEIVE", "SALE", "ADJUSTMENT").
    /// </remarks>
    /// <param name="request">The product ID, quantity, and optional type category.</param>
    /// <returns>The TransactionId of the newly created transaction.</returns>
    /// <response code="200">Transaction created successfully.</response>
    /// <response code="400">Validation failed — quantity cannot be zero, product ID must be positive.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddInventory([FromBody] AddInventoryRequest request)
    {
        var transactionId = await _inventoryService.AddAsync(request);
        // EVAL: Returning the TransactionId gives the caller a handle to reference this
        // specific transaction if they need to undo it via DELETE later.
        return Ok(new { TransactionId = transactionId });
    }

    /// <summary>
    /// Remove (undo) an inventory transaction by its ID.
    /// </summary>
    /// <remarks>
    /// Deleting a transaction permanently removes it and immediately corrects the inventory count.
    /// This is the undo mechanism — the transaction row is deleted rather than flagged.
    /// </remarks>
    /// <param name="transactionId">The ID of the transaction to remove.</param>
    /// <response code="204">Transaction removed successfully.</response>
    /// <response code="400">TransactionId must be a positive integer.</response>
    [HttpDelete("{transactionId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> RemoveTransaction(int transactionId)
    {
        await _inventoryService.RemoveTransactionAsync(transactionId);
        // EVAL: 204 No Content is the correct REST response for a successful DELETE
        // that returns no body.
        return NoContent();
    }

    /// <summary>
    /// Retrieve the total inventory count for a product or set of products.
    /// </summary>
    /// <remarks>
    /// Filter by ProductInstanceId to get the count for a specific product.
    /// Filter by AttributeKey and AttributeValue to aggregate inventory across all products
    /// sharing that metadata (e.g. all products where color=red).
    /// Omitting all filters returns the total inventory count across every product.
    /// </remarks>
    /// <param name="request">Optional filters: ProductInstanceId, AttributeKey, AttributeValue.</param>
    /// <returns>The total inventory count matching the filters.</returns>
    /// <response code="200">Returns the inventory count.</response>
    [HttpGet("count")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetCount([FromQuery] InventoryCountRequest request)
    {
        var count = await _inventoryService.GetCountAsync(request);
        return Ok(new { Count = count });
    }
}
