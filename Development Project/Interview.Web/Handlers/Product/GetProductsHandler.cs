using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Interview.Web.Domain.Dto;
using Interview.Web.Infrastructure;
using Interview.Web.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Handlers.Product;

public class GetProductsHandler :  IRequestHandler<GetProductsQuery, IEnumerable<ProductData>>
{
    private readonly IProductInvetoryDb _Db;
    private readonly IMapper _Mapper;
    
    public GetProductsHandler(IProductInvetoryDb db, IMapper mapper)
    {
        _Db = db;
        _Mapper = mapper;
    }

    public async Task<IEnumerable<ProductData>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products =  await _Db.Products
            .Include(p => p.Categories)
            .Include(p => p.Unit)
            .Include(p => p.Metadatas)
            .ThenInclude(m => m.MetadataType)
            .ToListAsync(cancellationToken: cancellationToken);

        return _Mapper.Map<IEnumerable<ProductData>>(products);
    }
}