using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Interview.DataEntities.Models;
using Interview.Services;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _CategoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _CategoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
        {
            try
            {
                var categoryId = await _CategoryService.CreateCategoryAsync(request);
                return CreatedAtAction(nameof(CreateCategory), new { id = categoryId }, new { id = categoryId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{categoryId}/products/{productId}")]
        public async Task<IActionResult> AddProductToCategory(int categoryId, int productId)
        {
            try
            {
                await _CategoryService.AddProductToCategoryAsync(productId, categoryId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _CategoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{categoryId}/attributes")]
        public async Task<IActionResult> GetCategoryAttributes(int categoryId)
        {
            var attributes = await _CategoryService.GetCategoryAttributesAsync(categoryId);
            return Ok(attributes);
        }
    }
}
