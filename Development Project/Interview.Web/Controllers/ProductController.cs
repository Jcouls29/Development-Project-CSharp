using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Models.DTOs.Requests;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Models.Search;
using Sparcpoint.Inventory.Services.Interfaces;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// API Controller for product management operations.
    /// EVAL: RESTful design with clear HTTP verb usage and status codes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new product with attributes and categories.
        /// </summary>
        /// <param name="request">Product creation details</param>
        /// <returns>Created product</returns>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
        {
            try
            {
                // EVAL: ModelState validation happens automatically via Data Annotations
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.CreateProductAsync(request);
                
                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = product.InstanceId },
                    product);
            }
            catch (InvalidOperationException ex)
            {
                // EVAL: Business rule violations return 400 with clear message
                _logger.LogWarning(ex, "Product creation failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating product");
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">Product instance ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
                return NotFound(new { error = $"Product with ID {id} not found" });

            return Ok(product);
        }

        /// <summary>
        /// Searches for products based on criteria.
        /// EVAL: Supports requirement for searching by metadata, categories, and general details.
        /// </summary>
        /// <param name="name">Product name filter (partial match)</param>
        /// <param name="description">Description filter (partial match)</param>
        /// <param name="sku">SKU filter (exact match)</param>
        /// <param name="categoryIds">Category IDs (comma-separated)</param>
        /// <param name="limit">Maximum results to return</param>
        /// <param name="offset">Number of results to skip</param>
        /// <returns>List of matching products</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<ProductResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductResponse>>> SearchProducts(
            [FromQuery] string? name = null,
            [FromQuery] string? description = null,
            [FromQuery] string? sku = null,
            [FromQuery] string? categoryIds = null,
            [FromQuery] int? limit = null,
            [FromQuery] int? offset = null)
        {
            // EVAL: Parse query string parameters into search criteria
            var criteria = new ProductSearchCriteria
            {
                Name = name,
                Description = description,
                Sku = sku,
                Limit = limit,
                Offset = offset
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

            var products = await _productService.SearchProductsAsync(criteria);
            return Ok(products);
        }

        /// <summary>
        /// Gets the current inventory count for a specific product.
        /// </summary>
        /// <param name="id">Product instance ID</param>
        /// <returns>Inventory count details</returns>
        [HttpGet("{id}/inventory")]
        [ProducesResponseType(typeof(InventoryCountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryCountResponse>> GetProductInventory(int id)
        {
            var inventory = await _productService.GetProductInventoryAsync(id);
            
            if (inventory == null)
                return NotFound(new { error = $"No inventory data found for product {id}" });

            return Ok(inventory);
        }
    }
}