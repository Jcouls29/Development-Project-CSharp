using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.Persistence.Entity.Sparcpoint.Entities;
using Sparcpoint.Infrastructure.Services.Interfaces;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/category")]
    [ApiController]
    [Consumes("application/json")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Category id is required.");
            }

            var category = await _categoryService.GetCategoryByIdAsync(id);

            return Ok(category);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            // EVAL: Check if model state is valid before proceeding.
            // Can use model validation attributes on the model to handle this
            // or use FluentValidation to handle this.

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool createdCategory = await _categoryService.CreateCategoryAsync(category);

            return createdCategory ? StatusCode(201) : BadRequest();
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            // EVAL: Check if model state is valid before proceeding.
            // Can use model validation attributes on the model to handle this
            // or use FluentValidation to handle this.

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedCategory = await _categoryService.UpdateCategoryAsync(category);

            return updatedCategory != null ? StatusCode(200) : BadRequest();
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Category id is required.");
            }

            bool deletedCategory = await _categoryService.DeleteCategoryAsync(id);

            return deletedCategory ? StatusCode(200) : BadRequest();
        }
    }
}
