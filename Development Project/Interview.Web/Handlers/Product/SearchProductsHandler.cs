using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Interview.Web.Domain.Dto;
using Interview.Web.Infrastructure;
using Interview.Web.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Handlers.Product;

public class SearchProductsHandler :  IRequestHandler<SearchProductsQuery, IEnumerable<ProductData>>
{
    private readonly IProductInvetoryDb _Db;
    private readonly IMapper _Mapper;
    
    public SearchProductsHandler(IProductInvetoryDb db, IMapper mapper)
    {
        _Db = db;
        _Mapper = mapper;
    }

    public async Task<IEnumerable<ProductData>> Handle(SearchProductsQuery searchQuery, CancellationToken cancellationToken)
    {

        var filter = _Db.Products
            .Include(p => p.Categories)
            .Include(p => p.Metadatas)
            .ThenInclude(m => m.MetadataType)
            .AsNoTracking();
            
        filter = QueryBasicProperties(searchQuery.searchProduct, filter);
        filter = QueryCategories(searchQuery.searchProduct, filter);
        filter = QueryMetadatas(searchQuery.searchProduct, filter);

        var result = await filter.ToListAsync();

        return _Mapper.Map<IEnumerable<ProductData>>(result);
    }

    private IQueryable<Domain.Entities.Product> QueryBasicProperties(Search.SearchProduct searchSearchProduct, IQueryable<Domain.Entities.Product> filter)
    {
        if (!string.IsNullOrEmpty(searchSearchProduct.Name))
        {
            filter = filter.Where(p => p.Name.Contains(searchSearchProduct.Name));
        }
        if (!string.IsNullOrEmpty(searchSearchProduct.Description))
        {
            filter = filter.Where(p => p.Description.Contains(searchSearchProduct.Description));
        }
        
        return filter;
    }
    
    private IQueryable<Domain.Entities.Product> QueryCategories(Search.SearchProduct searchSearchProduct, IQueryable<Domain.Entities.Product> filter)
    {
        if (searchSearchProduct.SearchCategories?.Count > 0)
        {
            foreach (var category in searchSearchProduct.SearchCategories)
            {
                if (!string.IsNullOrEmpty(category.Name))
                {
                    filter = filter.Where(p => p.Categories.Any(c => c.Name.Contains(category.Name)));
                }
                if (!string.IsNullOrEmpty(category.Description))
                {
                    filter = filter.Where(p => p.Categories.Any(c => c.Description.Contains(category.Description)));
                }
            }
        }
        return filter;
    }
    
    private IQueryable<Domain.Entities.Product> QueryMetadatas(Search.SearchProduct searchSearchProduct, IQueryable<Domain.Entities.Product> filter)
    {
        if (searchSearchProduct.SearchMetadatas?.Count > 0)
        {
            foreach (var metadata in searchSearchProduct.SearchMetadatas)
            {
                if (!string.IsNullOrEmpty(metadata.Name))
                {
                    filter = filter.Where(p => p.Metadatas.Any(c => c.Name.Contains(metadata.Name)));
                }
                if (!string.IsNullOrEmpty(metadata.Description))
                {
                    filter = filter.Where(p => p.Metadatas.Any(c => c.Description.Contains(metadata.Description)));
                }
                if (!string.IsNullOrEmpty(metadata.Value))
                {
                    filter = filter.Where(p => p.Metadatas.Any(c => c.Value.Contains(metadata.Value)));
                }
                if (!string.IsNullOrEmpty(metadata.MetadataType?.Name))
                {
                    filter = filter.Where(p => p.Metadatas.Any(c => c.MetadataType.Name.Contains(metadata.MetadataType.Name)));
                }
            }
        }
        return filter;
    }
}