using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        public sealed class AdjustmentRequest
        {
            public int ProductInstanceId { get; set; }
            public decimal Quantity { get; set; }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAsync([FromBody] AdjustmentRequest request, CancellationToken cancellationToken)
        {
            var id = await _service.AddInventoryAsync(request.ProductInstanceId, request.Quantity, cancellationToken);
            return Ok(new { transactionId = id });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveAsync([FromBody] AdjustmentRequest request, CancellationToken cancellationToken)
        {
            var id = await _service.RemoveInventoryAsync(request.ProductInstanceId, request.Quantity, cancellationToken);
            return Ok(new { transactionId = id });
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkAsync([FromBody] IReadOnlyList<AdjustmentRequest> adjustments, CancellationToken cancellationToken)
        {
            var ids = await _service.AdjustInventoryBulkAsync(
                adjustments.Select(a => (a.ProductInstanceId, a.Quantity)).ToArray(),
                cancellationToken);
            return Ok(new { transactionIds = ids });
        }

        [HttpDelete("transactions/{transactionId:int}")]
        public async Task<IActionResult> UndoAsync(int transactionId, CancellationToken cancellationToken)
        {
            var removed = await _service.UndoTransactionAsync(transactionId, cancellationToken);
            return removed ? NoContent() : NotFound();
        }

        [HttpGet("count/{productInstanceId:int}")]
        public async Task<IActionResult> GetCountAsync(int productInstanceId, CancellationToken cancellationToken)
            => Ok(await _service.GetCountAsync(productInstanceId, cancellationToken));

        [HttpGet("count/by-attribute")]
        public async Task<IActionResult> GetByAttributeAsync([FromQuery] string key, [FromQuery] string value, CancellationToken cancellationToken)
            => Ok(await _service.GetCountsByAttributeAsync(key, value, cancellationToken));
    }
}
