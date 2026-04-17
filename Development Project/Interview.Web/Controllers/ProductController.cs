using AutoMapper;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract.Services;
using Sparcpoint.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _mapper = mapper;
            _productService = productService;
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            if (productDto == null) return BadRequest("Invalid product data.");

            try
            {
                var request = _mapper.Map<CreateProductRequestDto>(productDto);
                int productId = await _productService.AddProductAsync(request);
                return Ok(new { ProductId = productId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchDto searchDto)
        {
            try
            {
                var request = _mapper.Map<ProductSearchRequestDto>(searchDto);
                var results = await _productService.SearchProductsAsync(request);

                if (results == null || !results.Any())
                    return NotFound("No products found.");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
