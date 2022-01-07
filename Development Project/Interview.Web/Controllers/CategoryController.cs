using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Models;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var results = await _categoryService.GetCategories();
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCategory(CreateCategoryRequest request)
        {
            await _categoryService.CreateCategoryAsync(request);
            return Ok();
        }

        [HttpPost("/{categoryId}/attributes")]
        public async Task<IActionResult> AddAttributesToCategory(int categoryId, List<KeyValuePair<string, string>> attributes)
        {
            await _categoryService.AddAttributesToCategory(categoryId, attributes);
            return Ok();
        }

        [HttpPost("/{categoryId}/categories")]
        public async Task<IActionResult> AddProductToCategory(int categoryId, List<int> categories)
        {
            await _categoryService.AddCategoryToCategories(categoryId, categories);
            return Ok();
        }
    }

}
