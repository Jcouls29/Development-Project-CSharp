using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Repositories;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// EVAL: Inventory endpoints. Covers requirements 3–6 of the statement:
    ///  POST /adjust           — adds or removes stock (individual or bulk)
    ///  POST /counts           — gets counts by product/metadata/category
    ///  DELETE /transactions/{id} — reverts a transaction (undo)
    ///  GET /products/{id}/transactions — lists transactions of a product
    /// </summary>
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventory;

        public InventoryController(IInventoryRepository inventory)
        {
            _inventory = inventory;
        }

        /// <summary>Adds or removes stock for one or more products in the same transaction.</summary>
        [HttpPost("adjust")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Adjust([FromBody] AdjustInventoryRequest request, CancellationToken ct)
        {
            if (request?.Adjustments == null || request.Adjustments.Count == 0)
                return BadRequest(new { error = "At least one adjustment is required." });

            var domain = request.Adjustments.Select(i => i.ToDomain()).ToList();
            var ids = await _inventory.RecordAsync(domain, ct);
            return Ok(new { transactionIds = ids });
        }

        /// <summary>Gets counts. Allows filtering by InstanceId, metadata or category.</summary>
        [HttpPost("counts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCounts([FromBody] GetInventoryCountsRequest request, CancellationToken ct)
        {
            var results = await _inventory.GetCountsAsync(request.ToDomain(), ct);
            return Ok(results);
        }

        /// <summary>Reverts (soft-delete) a transaction. If it was already reverted, returns 404.</summary>
        [HttpDelete("transactions/{transactionId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevertTransaction(int transactionId, CancellationToken ct)
        {
            var ok = await _inventory.RevertAsync(transactionId, ct);
            return ok ? NoContent() : (IActionResult)NotFound();
        }

        [HttpGet("products/{productInstanceId:int}/transactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ListProductTransactions(int productInstanceId, CancellationToken ct)
        {
            return Ok(await _inventory.ListTransactionsAsync(productInstanceId, ct));
        }
    }
}
