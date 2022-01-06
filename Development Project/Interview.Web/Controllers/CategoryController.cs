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
    }

}
