using System;
using System.Collections.Generic;
using Interview.Web.Domain.Dto;
using MediatR;

namespace Interview.Web.Queries
{
    public record SearchProductsQuery(Search.SearchProduct searchProduct) : IRequest<IEnumerable<ProductData>>;
}