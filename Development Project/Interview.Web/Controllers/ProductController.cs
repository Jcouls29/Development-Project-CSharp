using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductController : ControllerBase
{
    private readonly IProductsService _productsService;

    public ProductController(IProductsService productsService)
    {
        _productsService = productsService;
    }

    /// <summary>
    /// Add a new product to the system.
    /// </summary>
    /// <remarks>
    /// Products can never be deleted once created. Provide arbitrary metadata as key/value
    /// pairs in Attributes and assign one or more CategoryIds to categorize the product.
    /// </remarks>
    /// <param name="request">The product details including name, description, SKUs, image URIs, attributes, and categories.</param>
    /// <returns>The InstanceId of the newly created product.</returns>
    /// <response code="201">Product created successfully.</response>
    /// <response code="400">Validation failed — name is required and cannot exceed 256 characters.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var id = await _productsService.CreateAsync(request);
        // EVAL: 201 Created with a Location header is the correct REST response for a successful
        // POST. CreatedAtAction wires the Location header to the GET endpoint automatically.
        return CreatedAtAction(nameof(GetProducts), null, new { InstanceId = id });
    }

    /// <summary>
    /// Search for products using optional filters.
    /// </summary>
    /// <remarks>
    /// All filters are optional. Omitting all filters returns every product in the system.
    /// Filters are combined with AND — each additional filter narrows the result set.
    /// </remarks>
    /// <param name="request">Optional filters: Name, Description, AttributeKey/AttributeValue, and CategoryIds.</param>
    /// <returns>A list of products matching the provided filters.</returns>
    /// <response code="200">Returns the matching products.</response>
    [HttpGet]
    [ProducesResponseType(typeof(System.Collections.Generic.IEnumerable<ProductResponse>), 200)]
    public async Task<IActionResult> GetProducts([FromQuery] ProductSearchRequest request)
    {
        var products = await _productsService.SearchAsync(request);
        return Ok(products);
    }
}
