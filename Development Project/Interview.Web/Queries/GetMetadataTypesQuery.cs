using System.Collections.Generic;
using Interview.Web.Domain.Dto;
using Interview.Web.Domain.Entities;
using MediatR;

namespace Interview.Web.Queries
{
    public record GetMetadataTypesQuery : IRequest<IEnumerable<MetadataTypeData>>;
}