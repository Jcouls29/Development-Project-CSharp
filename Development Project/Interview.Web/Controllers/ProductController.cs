using AutoMapper;
using Interview.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public ProductController(IUnitOfWork uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await uow.ProductsRepository.GetProductsAsync();

            var productDtos = mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(productDtos);
        }
    }
}
