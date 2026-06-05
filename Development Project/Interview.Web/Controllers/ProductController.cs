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

    /// <summary>Add a new product.</summary>
    /// <remarks>
    /// Products are permanent — they cannot be deleted once created.
    /// Pass arbitrary metadata in Attributes and assign CategoryIds to categorize the product.
    /// </remarks>
    /// <param name="request">Name, description, SKUs, image URIs, attributes, and category IDs.</param>
    /// <returns>The InstanceId of the new product.</returns>
    /// <response code="201">Product created.</response>
    /// <response code="400">Name is required and cannot exceed 256 characters.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var id = await _productsService.CreateAsync(request);
        // EVAL: CreatedAtAction returns 201 and sets the Location header to the GET endpoint.
        return CreatedAtAction(nameof(GetProducts), null, new { InstanceId = id });
    }

    /// <summary>Search for products.</summary>
    /// <remarks>
    /// All filters are optional — omitting them returns everything.
    /// Multiple filters are combined with AND.
    /// </remarks>
    /// <param name="request">Name, Description, AttributeKey/AttributeValue, CategoryIds.</param>
    /// <returns>Products matching the filters.</returns>
    /// <response code="200">Returns matching products.</response>
    [HttpGet]
    [ProducesResponseType(typeof(System.Collections.Generic.IEnumerable<ProductResponse>), 200)]
    public async Task<IActionResult> GetProducts([FromQuery] ProductSearchRequest request)
    {
        var products = await _productsService.SearchAsync(request);
        return Ok(products);
    }
}
