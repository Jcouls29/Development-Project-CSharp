using Microsoft.AspNetCore.Mvc;
using Sparcpoint;
using Sparcpoint.Abstract;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _CategoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            PreConditions.ParameterNotNull(categoryRepository, nameof(categoryRepository));

            _CategoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            return Ok(await _CategoryRepository.AddCategoryAsync(category));
        }
    }
}
