using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using Business.Interfaces;
using DataAccess.Models;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        private IProductService _productService;
        
        // NOTE: Sample Action
        [HttpGet]
        public async Task<List<Product>> GetAllProducts()
        {
            return await _productService.GetProducts();
        }

        [HttpGet("metadata")]
        public async Task<List<Product>> GetProductsWithMetadata(List<Metadata> metadatas)
        {
            return await _productService.GetProductsWithMetadata(metadatas);
        }

        [HttpPost("single")]
        public async Task<Product> AddProduct(Product product)
        {
            return await _productService.AddProduct(product);
        }

        [HttpPost("multiple")]
        public async Task<List<Product>> AddProduct(List<Product> products)
        {
            return await _productService.AddProducts(products);
        }
    }
}
