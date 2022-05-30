using Interview.Web.Helpers;
using Interview.Web.Interfaces;
using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepo = categoryRepository;
        }

        /// <summary>
        /// EVAL: Checks for availablity:  This is a ping
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("IsAvailable")]
        public IActionResult IsAvailable()
        {
            return Ok(Constants.MessageIsAvailable);
        }

        [HttpGet]
        [Route("GetCategories")]
        public Task<IActionResult> GetCategories()
        {
            return Task.FromResult((IActionResult)Ok(_categoryRepo.GetAll()));
        }

        [HttpPost]
        [Route("AddCategory")]
        public Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return Task.FromResult((IActionResult)BadRequest("Invalid"));
            }
            else
            {
                return Task.FromResult((IActionResult)Ok(_categoryRepo.Add(category)));
            }
        }
    }
}
