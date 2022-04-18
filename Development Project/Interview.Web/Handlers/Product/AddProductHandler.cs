using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Interview.Web.Commands;
using Interview.Web.Domain.Dto;
using Interview.Web.Domain.Entities;
using Interview.Web.Infrastructure;
using MediatR;

namespace Interview.Web.Handlers.Product;

public class AddProductHandler :  IRequestHandler<AddProductCommand, ProductData>
{
    private readonly Db _Db;
    private readonly IMapper _Mapper;
    
    public AddProductHandler(Db db, IMapper mapper)
    {
        _Db = db;
        _Mapper = mapper;
    }

    public async Task<ProductData> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var productData = request.productData;
        var product = _Mapper.Map<Domain.Entities.Product>(productData);
        if (productData.Categories?.Count > 0)
        {
            product.Categories = new List<Category>();
            foreach (var categoryData in productData.Categories)
            {
                product.Categories.Add(await _Db.Categories.FindAsync(new object[] { categoryData.Id }, cancellationToken));
            }
        }
        if (productData.Metadatas?.Count > 0)
        {
            product.Metadatas = new List<Metadata>();
            foreach (var metadataData in productData.Metadatas)
            {
                product.Metadatas.Add(await _Db.Metadatas.FindAsync(new object[] { metadataData.Id }, cancellationToken));
            }
        }

        if (productData.Unit?.Id != null)
        {
            product.Unit = await _Db.Units.FindAsync(new object[] { productData.Unit.Id }, cancellationToken);
        }
        await _Db.Products.AddAsync(product, cancellationToken);
        await _Db.SaveChangesAsync(cancellationToken);
        
        return _Mapper.Map<ProductData>(product);
    }
}