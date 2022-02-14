using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Application.Abstracts;
using Sparcpoint.Domain.Instance.Entities;
using Sparcpoint.Domain.Requestes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICategoryValidationService _categoryValidationService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, ICategoryValidationService categoryValidationService, IMapper mapper)
        {
            _categoryService = categoryService;
            _categoryValidationService = categoryValidationService;
            _mapper = mapper;
        }

        // <summary>
        /// Creates new category
        /// </summary>
        [Route("create_category")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var catValidation = _categoryValidationService.CategoryIsValid(request);

            if (!catValidation.IsValid)
            {
                return BadRequest($"{catValidation.InvalidMessage}");
            }
            Category category = _mapper.Map<Category>(request);
            await _categoryService.CreateCategoryAsync(category);
            return Ok();
        }
        /// <summary>
        /// Adds attribute to a category
        /// </summary>
        [HttpPost("{categoryId}/attributes")]
        public async Task<IActionResult> AddAttributesToCategory(int categoryId, [FromBody] List<CategoryAttributeRequest> attributesRequest)
        {
            if (attributesRequest==null || attributesRequest.Count==0)
            {
                return BadRequest("You must include at least one attribute");
            }
            var attributes = _mapper.Map<List<CategoryAttributeRequest>, List<CategoryAttribute>>(attributesRequest);
            await _categoryService.AddAttributeToCategoryAsync(categoryId, attributes);
            return Ok();
        }
        /// <summary>
        /// Adds category to a category
        /// </summary>
        [HttpPost("{categoryId}/categories")]
        public async Task<IActionResult> AddCategoryToCategory(int categoryId, List<CategoryOfCategoryRequest> categoriesRequest)
        {
            if (!categoriesRequest.Any())
            {
                return BadRequest("You must include at least one category");
            }
            var categories = _mapper.Map<List<CategoryOfCategoryRequest>, List<CategoryOfCategory>>(categoriesRequest);
            await _categoryService.AddCategoryToCategoriesAsync(categoryId, categories);
            return Ok();
        }
        /// <summary>
        /// Gets all categories
        /// </summary>
        [HttpGet]
        public async Task<List<Category>> GetAllCategories()
        {
            return await _categoryService.GetCategoriesAsync();
        }




    }
}
