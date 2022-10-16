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
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _ProductRepository;
        public ProductController(IProductRepository productRepository)
        {
            PreConditions.ParameterNotNull(productRepository, nameof(productRepository));

            _ProductRepository = productRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            return Ok(await _ProductRepository.AddProductAsync(product));
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
