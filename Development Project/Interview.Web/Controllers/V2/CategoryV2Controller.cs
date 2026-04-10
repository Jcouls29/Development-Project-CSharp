using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;

namespace Interview.Web.Controllers.V2
{
    /// <summary>
    /// EVAL: V2 Category controller extends V1 with bulk category creation.
    /// </summary>
    [ApiVersion(2.0)]
    [Route("api/v{version:apiVersion}/categories")]
    [ApiController]
    public class CategoryV2Controller : ControllerBase
    {
        private readonly ICategoryService _CategoryService;

        public CategoryV2Controller(ICategoryService categoryService)
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

        /// <summary>
        /// EVAL: V2 bulk create — partial failure support per item.
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateCategoriesBulk([FromBody] List<CreateCategoryRequest> requests)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest(new { error = "At least one category is required." });

            var results = new List<CategoryResponse>();
            var errors = new List<object>();

            for (int i = 0; i < requests.Count; i++)
            {
                try
                {
                    var category = new Category
                    {
                        Name = requests[i].Name,
                        Description = requests[i].Description,
                        Attributes = requests[i].Attributes ?? new Dictionary<string, string>(),
                        ParentCategoryIds = requests[i].ParentCategoryIds ?? new List<int>()
                    };

                    var created = await _CategoryService.CreateCategoryAsync(category);
                    results.Add(CategoryResponse.FromCategory(created));
                }
                catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException)
                {
                    errors.Add(new { index = i, error = ex.Message });
                }
            }

            return Ok(new { created = results, errors });
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

        [HttpGet("{id:int}/children")]
        public async Task<IActionResult> GetChildCategories(int id)
        {
            var children = await _CategoryService.GetChildCategoriesAsync(id);
            return Ok(children.Select(CategoryResponse.FromCategory));
        }
    }
}
