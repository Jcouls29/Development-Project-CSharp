using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Commands;
using Interview.Web.Domain.Dto;
using Interview.Web.Queries;
using MediatR;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    public class CategoriesController : Controller
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IList<CategoryData>>> GetCategoris()
        {
            var categories = await _mediator.Send(new GetProductCategoriesQuery());

            return Ok(categories);
        }
    }
}

