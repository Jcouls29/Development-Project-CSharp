using System;
using Interview.Web.Domain.Dto;
using MediatR;

namespace Interview.Web.Queries
{
    public record GetInventoryForProductIdQuery(Guid ProductId) : IRequest<InventoryData>;
}