using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
     // ProductController.cs
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
          private readonly Product_DbContext _context;
         
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        public ProductController(Product_DbContext context)
        {
            _context = context;
        }
        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {  
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
     
    

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string searchTerm, List<int> categoryIds, List<int> metadataIds)
        {
            var products = await _context.SearchProductsAsync(searchTerm, categoryIds, metadataIds);
            return Ok(products);
        }
       
        [HttpPost]
       [HttpPost]
        public IActionResult AddProduct([FromBody] ProductRequestModel model)
        {
            // Model validation and error handling
            if(model == null){
                return;
            }
            try
            {
                _productService.AddProduct(model.Name, model.Description, model.Price, model.CategoryNames, model.Metadata);
             
            }
            catch (System.Exception ex)
            {
                throw ex.Message;
            }
           
            return Ok("Product added successfully.");
        }


    }

}
