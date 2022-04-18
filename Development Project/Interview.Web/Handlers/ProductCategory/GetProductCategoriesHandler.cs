using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Interview.Web.Domain.Dto;
using Interview.Web.Infrastructure;
using Interview.Web.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Handlers.ProductCategory;

public class GetProductCategoriesHandler :  IRequestHandler<GetProductCategoriesQuery, IEnumerable<CategoryData>>
{
    private readonly IProductInvetoryDb _Db;
    private readonly IMapper _Mapper;
    public GetProductCategoriesHandler(IProductInvetoryDb db, IMapper mapper)
    {
        _Db = db;
        _Mapper = mapper;
    }

    public async Task<IEnumerable<CategoryData>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _Db.Categories.ToListAsync(cancellationToken: cancellationToken);
        return _Mapper.Map<List<CategoryData>>(categories);
    }
}