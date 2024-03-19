using Interview.Web.DTO;
using Interview.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAll()
        {
            var products = _productService.GetAllProducts();
            return Task.FromResult((IActionResult)Ok(products));
        }
        [HttpGet("search")]
        public IActionResult Search(SearchCriteriaDto criteria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var products = _productService.SearchProducts(criteria);
            return Ok(products);
        }

        [HttpGet("inventory/count")]
        public IActionResult GetInventoryCountByMetadata(SearchCriteriaDto criteria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var count = _productService.GetInventoryCountByMetadata(criteria);
            return Ok(count);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProductCreationRequestDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = _productService.CreateProductAsync(productDto);
            return Ok(response);
        }
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var isDeleted = await _productService.SoftDeleteProduct(id);
            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
