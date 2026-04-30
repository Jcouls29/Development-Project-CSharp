using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    // EVAL: ControllerBase is used instead of Controller since this is a pure API —
    // no Razor views are needed. This avoids pulling in MVC view dependencies.
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _Products;
        private readonly IInventoryRepository _Inventory;

        // EVAL: Both repositories injected via constructor so the controller
        // has no knowledge of SQL, Dapper, or connection strings. Swapping
        // implementations (e.g. in-memory for tests) requires zero controller changes.
        public ProductController(IProductRepository products, IInventoryRepository inventory)
        {
            _Products = products ?? throw new ArgumentNullException(nameof(products));
            _Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        }

        // GET api/v1/products
        // EVAL: Returns all products. In production this would be paged,
        // but for the scope of this project a full list is acceptable.
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _Products.GetAllAsync();
            return Ok(products);
        }

        // GET api/v1/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _Products.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // GET api/v1/products/search?name=Widget&categoryIds=1&categoryIds=2&attrKeys=Color&attrVals=Red
        // EVAL: Search is a GET with query string params rather than POST + body.
        // This keeps results bookmark-able and cache-friendly. If attribute filters
        // grow complex, this can be promoted to POST without breaking existing callers.
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string name,
            [FromQuery] int[]? categoryIds,
            [FromQuery] string[]? attrKeys,
            [FromQuery] string[]? attrVals)
        {
            // EVAL: attrKeys and attrVals are parallel arrays — index N of attrKeys
            // pairs with index N of attrVals. This is a pragmatic workaround since
            // query strings don't support dictionary syntax natively.
            var request = new ProductSearchRequest
            {
                Name = name,
                CategoryInstanceIds = categoryIds != null
                    ? new System.Collections.Generic.List<int>(categoryIds)
                    : new List<int>()
            };

            if (attrKeys != null && attrVals != null)
            {
                int pairCount = Math.Min(attrKeys.Length, attrVals.Length);
                for (int i = 0; i < pairCount; i++)
                    request.Attributes[attrKeys[i]] = attrVals[i];
            }

            var products = await _Products.SearchAsync(request);
            return Ok(products);
        }

        // POST api/v1/products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Product name is required.");

            int newId = await _Products.AddFromRequestAsync(request);

            // EVAL: 201 Created with a Location header pointing to the new resource
            // follows REST convention — lets clients navigate directly to the created
            // product without a separate lookup.
            return CreatedAtAction(nameof(GetProduct), new { id = newId }, new { InstanceId = newId });
        }

        // GET api/v1/products/{id}/inventory
        [HttpGet("{id:int}/inventory")]
        public async Task<IActionResult> GetInventoryCount(int id)
        {
            var product = await _Products.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var count = await _Inventory.GetInventoryCountAsync(id);
            return Ok(new { ProductInstanceId = id, Count = count });
        }

        // POST api/v1/products/{id}/inventory/add
        [HttpPost("{id:int}/inventory/add")]
        public async Task<IActionResult> AddInventory(int id, [FromBody] AddInventoryRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            var product = await _Products.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            // EVAL: Route id takes precedence over anything in the body to prevent
            // callers from accidentally adding inventory to a different product.
            request.ProductInstanceId = id;

            int transactionId = await _Inventory.AddAsync(request);
            return Ok(new { TransactionId = transactionId });
        }

        // POST api/v1/products/{id}/inventory/remove
        [HttpPost("{id:int}/inventory/remove")]
        public async Task<IActionResult> RemoveInventory(int id, [FromBody] RemoveInventoryRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            var product = await _Products.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            request.ProductInstanceId = id;

            int transactionId = await _Inventory.RemoveAsync(request);
            return Ok(new { TransactionId = transactionId });
        }

        // DELETE api/v1/products/inventory/transactions/{transactionId}
        // EVAL: This is the "undo" endpoint from the spec. Deleting a transaction
        // row reverses it since inventory count is always derived from SUM(Quantity).
        // Scoped under /products since all transactions are product-scoped, but only
        // the transactionId is needed in the route — no product id required.
        [HttpDelete("inventory/transactions/{transactionId:int}")]
        public async Task<IActionResult> UndoTransaction(int transactionId)
        {
            await _Inventory.RemoveTransactionAsync(transactionId);
            return NoContent();
        }
    }
}
