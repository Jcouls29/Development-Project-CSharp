using Interview.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoriesService)
        {
            _categoryService = categoriesService;
        }

        [HttpGet]
        public Task<IActionResult> GetAllCategories()
        {
            var categories = _categoryService.GetAllCategories();
            return Task.FromResult((IActionResult)Ok(categories));
        }
    }
}
