using Interview.Web.Dtos.Requests;
using Interview.Web.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Application.Dtos.Requests;
using Sparcpoint.Inventory.Application.Interfaces;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var id = await _service.CreateAsync(request);

            return Ok(new CreateProductResponse
            {
                Id = id
            });
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(SearchProductsRequest request)
        {
            var result = await _service.SearchAsync(request);
            return Ok(result);
        }
    }
}
