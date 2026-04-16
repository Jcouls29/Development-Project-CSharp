using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Models;
using Sparcpoint.Models.DTOs;
using Sparcpoint.Repositories;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        private readonly IProductRepository _productRepository;


        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDto newProduct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categories = new List<Category>();
            foreach (var category in newProduct.Categories)
            {
                var validCategory = await _categoryRepository.FindByIdAsync(category.InstanceId);
                if (category != null) categories.Add(validCategory);
            }

            var product = Product.Create(newProduct);

            await _productRepository.AddAsync(product);

            return NoContent();
        }
    }
}
