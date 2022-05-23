using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Models.Request.Category;
using Sparcpoint.Models.Request.Product;
using Sparcpoint.Models.Response.Category;
using Sparcpoint.Models.Response.Product;
using Sparcpoint.Services.Interfaces;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts()
        {
            var categories = await _categoryService.GetAllCategories(new GetCategoryRequest());
            return Ok(categories);
        }

        [HttpGet("GetProductCategories")]
        [ProducesResponseType(typeof(GetProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductById([FromQuery] int productId)
        {
            var request = new GetProductRequestById(productId);
            var products = await this._categoryService.GetProductCategories(request);
            return Ok(products);
        }
    }
}