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
        IValidationService _validationService;

        public CategoryController(ICategoryService categoryService, IValidationService validationService)
        {
            _categoryService = categoryService;
            _validationService = validationService;
        }


        /// <summary>
        /// Gets all categories
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var results = await _categoryService.GetCategories();
            return Ok(results);
        }

        /// <summary>
        /// Creates a category
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateNewCategory([FromBody]CreateCategoryRequest request)
        {
            var catValidation = _validationService.CategoryIsValid(request);

            if (!catValidation.IsValid)
            {
                return BadRequest($"{catValidation.InvalidMessage}");
            }

            await _categoryService.CreateCategoryAsync(request);
            return Ok();
        }

        /// <summary>
        /// Adds attribute to a category
        /// </summary>
        [HttpPost("{categoryId}/attributes")]
        public async Task<IActionResult> AddAttributesToCategory(int categoryId, [FromBody]Dictionary<string, string> attributes)
        {
            if (attributes == null)
            {
                return BadRequest("You must include at least one attribute");
            }

            await _categoryService.AddAttributesToCategory(categoryId, attributes.ToList());
            return Ok();
        }

        /// <summary>
        /// Adds category to a category
        /// </summary>
        [HttpPost("{categoryId}/categories")]
        public async Task<IActionResult> AddCategoryToCategory(int categoryId, List<int> categories)
        {
            if (!categories.Any())
            {
                return BadRequest("You must include at least one category");
            }

            await _categoryService.AddCategoryToCategories(categoryId, categories);
            return Ok();
        }
    }

}
