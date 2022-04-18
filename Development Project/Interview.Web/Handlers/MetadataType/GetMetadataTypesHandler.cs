using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Interview.Web.Domain.Dto;
using Interview.Web.Infrastructure;
using Interview.Web.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Handlers.MetadataType;

public class GetMetadataTypesHandler :  IRequestHandler<GetMetadataTypesQuery, IEnumerable<MetadataTypeData>>
{
    private readonly IProductInvetoryDb _Db;
    private readonly IMapper _Mapper;
    public GetMetadataTypesHandler(IProductInvetoryDb db, IMapper mapper)
    {
        _Db = db;
        _Mapper = mapper;
    }

    public async Task<IEnumerable<MetadataTypeData>> Handle(GetMetadataTypesQuery request, CancellationToken cancellationToken)
    {
        var metadataTypes =  await _Db.MetadataTypes.ToListAsync(cancellationToken: cancellationToken);
        return _Mapper.Map<List<MetadataTypeData>>(metadataTypes);
    }
}