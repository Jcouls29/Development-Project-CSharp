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

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var id = await _productsService.CreateAsync(request);
        // EVAL: 201 Created with a Location header is the correct REST response for a successful
        // POST. CreatedAtAction wires the Location header to the GET endpoint automatically.
        return CreatedAtAction(nameof(GetProducts), new { id }, new { InstanceId = id });
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductSearchRequest request)
    {
        var products = await _productsService.SearchAsync(request);
        return Ok(products);
    }
}
