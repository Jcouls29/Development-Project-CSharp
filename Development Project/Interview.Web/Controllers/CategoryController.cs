using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models.Requests;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _Categories;

        public CategoryController(ICategoryRepository categories)
        {
            _Categories = categories;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequest request)
        {
            var instanceId = await _Categories.AddAsync(request);
            return CreatedAtAction(nameof(GetAllCategories), new { }, new { InstanceId = instanceId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _Categories.GetAllAsync();
            return Ok(categories);
        }
    }
}
