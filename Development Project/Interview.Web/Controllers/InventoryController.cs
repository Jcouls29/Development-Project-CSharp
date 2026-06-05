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

    /// <summary>Add an inventory transaction.</summary>
    /// <remarks>
    /// Positive Quantity adds stock, negative removes it.
    /// Each call creates a transaction row that can be undone individually via DELETE.
    /// TypeCategory is optional — use it to label the transaction e.g. "RECEIVE", "SALE".
    /// </remarks>
    /// <param name="request">ProductInstanceId, Quantity, and optional TypeCategory.</param>
    /// <returns>The TransactionId of the new transaction.</returns>
    /// <response code="200">Transaction created.</response>
    /// <response code="400">Quantity cannot be zero. ProductInstanceId must be positive.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddInventory([FromBody] AddInventoryRequest request)
    {
        var transactionId = await _inventoryService.AddAsync(request);
        // EVAL: Returning the TransactionId lets the caller reference this transaction for DELETE.
        return Ok(new { TransactionId = transactionId });
    }

    /// <summary>Remove (undo) a transaction.</summary>
    /// <remarks>
    /// Permanently deletes the transaction row. The inventory count updates immediately.
    /// </remarks>
    /// <param name="transactionId">The transaction to remove.</param>
    /// <response code="204">Transaction removed.</response>
    /// <response code="400">TransactionId must be a positive integer.</response>
    [HttpDelete("{transactionId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> RemoveTransaction(int transactionId)
    {
        await _inventoryService.RemoveTransactionAsync(transactionId);
        // EVAL: 204 No Content — correct response for a DELETE with no body.
        return NoContent();
    }

    /// <summary>Get total inventory count.</summary>
    /// <remarks>
    /// Filter by ProductInstanceId for a single product, or by AttributeKey/AttributeValue
    /// to aggregate across all products with that metadata (e.g. all red products).
    /// No filters returns the total across everything.
    /// </remarks>
    /// <param name="request">ProductInstanceId, AttributeKey, AttributeValue — all optional.</param>
    /// <returns>Total inventory count.</returns>
    /// <response code="200">Returns the count.</response>
    [HttpGet("count")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetCount([FromQuery] InventoryCountRequest request)
    {
        var count = await _inventoryService.GetCountAsync(request);
        return Ok(new { Count = count });
    }
}
