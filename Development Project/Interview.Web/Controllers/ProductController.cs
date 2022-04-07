using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SparcpointServices.Interface;
using SparcpointServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {

        private readonly IProduct _product;
        private readonly IMapper _mapper;

        public ProductController(IProduct product, IMapper mapper)
        {
            _product = product;
            _mapper = mapper;
        }

        
        
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                return Ok(_product.GetAllProducts());
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
            // return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [Route("addproduct")]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel request)
        {
            try
            {
                Product product = _mapper.Map<Product>(request);
                _product.AddProduct(product);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            
        }

        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> SearchProduct(string keyword)
        {
            try
            {
                return Ok(_product.SearchProduct(keyword));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
