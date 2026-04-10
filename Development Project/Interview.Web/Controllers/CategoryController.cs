using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// EVAL: Category API controller for managing product categories and hierarchies.
    /// This fulfills Goal #3: "The ability to categorize and create hierarchies of products."
    /// </summary>
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/categories")]
    [ApiController]
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
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Attributes = request.Attributes ?? new Dictionary<string, string>(),
                ParentCategoryIds = request.ParentCategoryIds ?? new List<int>()
            };

            var created = await _CategoryService.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategory), new { id = created.InstanceId }, CategoryResponse.FromCategory(created));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _CategoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new { error = $"Category with ID {id} not found." });

            return Ok(CategoryResponse.FromCategory(category));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _CategoryService.GetAllCategoriesAsync();
            return Ok(categories.Select(CategoryResponse.FromCategory));
        }

        /// <summary>
        /// EVAL: Returns child categories for hierarchy navigation.
        /// </summary>
        [HttpGet("{id:int}/children")]
        public async Task<IActionResult> GetChildCategories(int id)
        {
            var children = await _CategoryService.GetChildCategoriesAsync(id);
            return Ok(children.Select(CategoryResponse.FromCategory));
        }
    }
}
