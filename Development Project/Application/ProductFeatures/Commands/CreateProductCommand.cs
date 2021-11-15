using Domain.Dtos;
using Domain.Entity;
using MediatR;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ProductFeatures.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string StockKeepUnitCode { get; set; }

        public int CategoryId { get; set; }
        public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
        {
            private readonly IProductRepository _productRepository;
            private readonly ICategoryRepository _categoryRepository;

            public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
            {
                this._categoryRepository = categoryRepository;
                this._productRepository = productRepository;
            }

            public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
            {

                var category = _categoryRepository.GetCategoryId(request.CategoryId);
                var product = new Product(request.Name,request.Description,request.StockKeepUnitCode,category);
                _productRepository.InsertProduct(product);
                _productRepository.SaveChangesAsync();
                var productDto = new ProductDto
                {
                    Name = product.Name,
                    Description = product.Description,
                    StockKeepUnitCode = product.StockKeepUnitCode,
                    CategoryDescription = category.Description
                };
                return await Task.FromResult(productDto);

            }
        }
    }
}
