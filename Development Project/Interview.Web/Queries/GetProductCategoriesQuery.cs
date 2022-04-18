using System.Collections.Generic;
using Interview.Web.Domain.Dto;
using MediatR;

namespace Interview.Web.Queries
{
    public record GetProductCategoriesQuery : IRequest<IEnumerable<CategoryData>>;
}