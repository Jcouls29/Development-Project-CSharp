using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _CategoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _CategoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var category = await _CategoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategory), new { id = category.InstanceId }, category);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _CategoryService.GetCategoryAsync(id);
            if (category == null)
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Category with ID {id} not found" });
            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            var result = await _CategoryService.GetCategoriesAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPost("{id}/attributes")]
        public async Task<IActionResult> AddAttribute(int id, [FromBody] UpdateCategoryRequest request)
        {
            foreach (var attr in request.Attributes)
            {
                await _CategoryService.AddAttributeAsync(id, attr.Key, attr.Value);
            }
            var category = await _CategoryService.GetCategoryAsync(id);
            return Ok(category);
        }

        [HttpDelete("{id}/attributes/{key}")]
        public async Task<IActionResult> RemoveAttribute(int id, string key)
        {
            await _CategoryService.RemoveAttributeAsync(id, key);
            return Ok();
        }

        [HttpPost("{id}/parents/{parentCategoryId}")]
        public async Task<IActionResult> AddParentCategory(int id, int parentCategoryId)
        {
            await _CategoryService.AddParentCategoryAsync(id, parentCategoryId);
            var category = await _CategoryService.GetCategoryAsync(id);
            return Ok(category);
        }

        [HttpDelete("{id}/parents/{parentCategoryId}")]
        public async Task<IActionResult> RemoveParentCategory(int id, int parentCategoryId)
        {
            await _CategoryService.RemoveParentCategoryAsync(id, parentCategoryId);
            return Ok();
        }
    }
}
