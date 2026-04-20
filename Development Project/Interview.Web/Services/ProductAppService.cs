using Interview.Web.Contracts;
using Interview.Web.DTOs;
using Sparcpoint.Contracts;
using Sparcpoint.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    /** ProductAppService is a class that implements the IProductAppService interface.
     */
    public class ProductAppService : IProductAppService
    {
        private readonly IProductRepository productRepository;

        public ProductAppService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<int> AddProduct(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                ProductImageUris = createProductDto.ImageUris,
                ValidSkus = createProductDto.ValidSkus,
            };

            foreach (var attribute in createProductDto.Attributes)
            {
                product.SetAttribute(attribute.Key, attribute.Value);
            }

            foreach (var category in createProductDto.Categories)
            {
                product.SetCategory(category);
            }

            int id = await productRepository.Add(product);

            return id;
        }

        //filter=Name:Test 1,Color:Azul,Largo:10CM
        public async Task<IEnumerable<GetProductDto>> GetProduct(string filter)
        {
            var result = await productRepository.Get(filter);

            return result.Select(p => new GetProductDto
            {
                Name = p.Name,
                Description = p.Description,
                ImageUris = p.ProductImageUris,
                ValidSkus = p.ValidSkus,
                CreatedTimestamp = p.CreatedTimestamp,
                Attributes = p.Attributes.Select(a => new ProductAttributeDto { Key = a.Key, Value = a.Value }).ToList(),
                Categories = p.Categories.Select(c => new CategoryDto { Name = c.Name }).ToList()
            });
        }
    }
}
