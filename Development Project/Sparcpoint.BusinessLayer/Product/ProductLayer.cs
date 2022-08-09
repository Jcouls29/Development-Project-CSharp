using Sparcpoint.Models;
using Sparcpoint.DataLayer.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sparcpoint.Models.DomainDto.Product;
using Sparcpoint.Mappers.DomainToEntity;
using System.Linq;
using Sparcpoint.Models.DomainModels;
using Microsoft.Extensions.Logging;

namespace Sparcpoint.BusinessLayer.Product
{
    public class ProductLayer :IProductLayer
    {
        private readonly IProductRepository _productRepository;
        private readonly IDataSerializer _serialize;
        private readonly ILogger<ProductLayer> _logger;
        public ProductLayer(IProductRepository productRepository, IDataSerializer serialize, ILogger<ProductLayer> logger)
        {
            _productRepository = productRepository;
            _serialize = serialize;
            _logger = logger; 
        }
        public async Task<Products> AddProduct(ProductDomain product) {
            var productEntity  = ProductEntityMapper.MapDomainToEntity(product);
            _logger.LogInformation("Add Product Entity Object {0}", _serialize.Serialize<Products>(productEntity));
            productEntity.Attributes.ToList().ForEach(x => x.InstanceId = productEntity.InstanceId);
            productEntity.Categories.ToList().ForEach(x => x.InstanceId = productEntity.InstanceId);
            var productAdded = await _productRepository.AddProduct(productEntity);
            return productAdded;
        }
        public async Task<List<Products>> SearchProduct(FilterParam filterModel) {
            var productFiltered = await _productRepository.SearchProduct(filterModel);
            return productFiltered;
        }
    }
}
