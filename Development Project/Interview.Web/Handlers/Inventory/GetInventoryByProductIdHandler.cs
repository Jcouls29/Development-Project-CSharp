using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Interview.Web.Domain.Dto;
using Interview.Web.Infrastructure;
using Interview.Web.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Handlers.Inventory;

public class GetInventoryByProductIdHandler : IRequestHandler<GetInventoryForProductIdQuery, InventoryData>
{
    private readonly IProductInvetoryDb _Db;
    private readonly IMapper _Mapper;

    public GetInventoryByProductIdHandler(IProductInvetoryDb db, IMapper mapper)
    {
        _Db = db;
        _Mapper = mapper;
    }

    public async Task<InventoryData> Handle(GetInventoryForProductIdQuery request,
        CancellationToken cancellationToken)
    {
        var inventory = await _Db.Inventories.SingleAsync(i => i.Product.Id == request.ProductId);
        return _Mapper.Map<InventoryData>(inventory);
    }
}