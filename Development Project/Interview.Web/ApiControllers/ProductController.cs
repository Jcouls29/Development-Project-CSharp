using Microsoft.AspNetCore.Mvc;
using Dal.Interfaces;
using Dal.Models;
using System.Collections.Generic;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interview.Web.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public IEnumerable<Products> Get()
        {
            return _productRepository.GetProducts();
        }

        // GET api/<ProductController>/5
        [HttpGet("searchByCategory")]
        public List<Products> Get(string searchCriteria)
        {
            return _productRepository.SearchProductsByCategory(searchCriteria);
        }

        [HttpPost]
        public int Post([FromBody] Products product)
        {
            //Will only add category if category with the same name does not exist already
            var categoriesId = _productRepository.AddCategories(product);
            //Will only add product if product with same name does not exist already
            int productId = _productRepository.AddProduct(product);

            //Update Product Categories and CategoryCategories
            _productRepository.UpdateProductCategories(categoriesId, productId);
            return productId;
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
    
}
