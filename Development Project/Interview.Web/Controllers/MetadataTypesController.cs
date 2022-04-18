using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Commands;
using Interview.Web.Domain.Dto;
using Interview.Web.Queries;
using MediatR;

namespace Interview.Web.Controllers
{
    [Route("api/v1/metadataTypes")]
    public class MetadataTypesController : Controller
    {
        private readonly IMediator _mediator;

        public MetadataTypesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IList<MetadataTypeData>>> GetMetadataTypes()
        {
            var metadataTypes = await _mediator.Send(new GetMetadataTypesQuery());

            return Ok(metadataTypes);
        }
    }
}

