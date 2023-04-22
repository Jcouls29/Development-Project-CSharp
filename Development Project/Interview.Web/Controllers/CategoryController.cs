using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sparcpoint.Abstract;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ICategoryOperator _categoryOperator;

        public CategoryController(ICategoryOperator categoryOperator)
        {
            _categoryOperator = categoryOperator;
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllCategories()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
