using Interview.Web.Contexts;
using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Contexts;
using Microsoft.Extensions.Logging;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private ILogger<CategoryController> _logger;

        public CategoryController(ILogger<CategoryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<CategoryModel>> GetAllCategories()
        {
            try
            {
                var categories = CategoryContext.GetCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem();
            }
        }

        [HttpPost]
        public ActionResult<CategoryModel> CreateCategory([FromBody] CreateCategoryDto category)
        {
            try
            {
                var newCat = CategoryContext.CreateCategory(category);
                return Ok(newCat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem();
            }
        }
    }
}