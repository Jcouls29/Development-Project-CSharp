using Domain.Dtos;
using Domain.Entity;
using MediatR;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ProdFeatures.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        public string StockUnitCode { get; set; }

        public int ProductId { get; set; }
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

                var category = _categoryRepository.GetCategoryId(request.ProductId);
                var product = new Product(request.ProductName, request.ProductDescription, request.StockUnitCode, category);
                _productRepository.InsertProduct(product);
                _productRepository.SaveChangesAsync();
                var productDto = new ProductDto
                {
                    ProductName product.ProductName,
                    ProductDescription = product.ProductDescription,
                    StockUnitCode = product.StockUnitCode,
                    ProductDescription = category.ProductDescription
                };
                return await Task.FromResult(productDto);

            }
        }
    }
}