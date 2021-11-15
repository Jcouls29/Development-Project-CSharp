using Domain.Dtos;
using MediatR;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ProductFeatures.Queries
{
    public class GetProductsQuery : IRequest<IReadOnlyCollection<ProductDto>>
    {

        public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyCollection<ProductDto>>
        {
            private readonly IProductRepository _productRepository;
            private readonly ICategoryRepository _categoryRepository;
            public GetProductsQueryHandler(IProductRepository productRepository,ICategoryRepository categoryRepository)
            {
                this._productRepository = productRepository;
                this._categoryRepository = categoryRepository;
            }
            public async Task<IReadOnlyCollection<ProductDto>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
            {
               var listOfPrdoucts = _productRepository.GetProudcts();
                var listofCategories = _categoryRepository.GetCategories();
                if (listOfPrdoucts != null && listOfPrdoucts.Any())
                {
                    var mappedProductsDto = listOfPrdoucts.Select(x => new ProductDto()
                    {
                        Description = x.Description,
                        Name = x.Name,
                        StockKeepUnitCode = x.StockKeepUnitCode,
                        CategoryDescription = listofCategories.Where(c => c.InstanceId == x.Category.InstanceId).FirstOrDefault().Description
                    }).ToList();
                    return await Task.FromResult(mappedProductsDto.AsReadOnly());
                }
                return null;
            }

        }
    }
}
