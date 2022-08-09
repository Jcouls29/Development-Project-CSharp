using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Sparcpoint.BusinessLayer.Product;
using Sparcpoint.DataLayer.Repository;
using Sparcpoint.Mappers.DomainToEntity;
using Sparcpoint.Models;
using Sparcpoint.Models.DomainDto.Product;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Tests
{
    public class ProductManagerTest
    {
        
        [Trait("TestCategory", "Unit")]
        [Test]
        public async Task AddProduct()
        {
            IProductRepository productRepository = Substitute.For<IProductRepository>();
            IDataSerializer dataSerializer = Substitute.For<IDataSerializer>();
            ILogger<ProductLayer> logger = Substitute.For<ILogger<ProductLayer>>();
            ProductDto productDto = Substitute.For<ProductDto>();
            ProductLayer productManager = new ProductLayer(productRepository,dataSerializer, logger);
            productRepository.AddProduct(Arg.Any<Products>()).Returns(new Products() {Description = "Test Product",Name = "Test" });
            var result  = await productManager.AddProduct(ProductEntityMapper.MapDTOtoDomain(productDto));
            Assert.IsNotNull(result);
        }
    }
}