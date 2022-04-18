using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Commands;
using Interview.Web.Domain.Dto;
using Interview.Web.Queries;
using Interview.Web.Search;
using MediatR;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IList<ProductData>>> GetProducts()
        {
            var products = await _mediator.Send(new GetProductsQuery());

            return Ok(products);
        }
        
        [HttpPost]
        public async Task<ActionResult<ProductData>> CreateProduct([FromBody] ProductData productData)
        {
            var newProduct = await _mediator.Send(new AddProductCommand(productData));

            return Ok(newProduct);
        }
        
        [HttpPost("search")]
        public async Task<ActionResult<ProductData>> SearchProducts([FromBody] SearchProduct searchProduct)
        {
            var products = await _mediator.Send(new SearchProductsQuery(searchProduct));

            return Ok(products);
        }
    }
}

