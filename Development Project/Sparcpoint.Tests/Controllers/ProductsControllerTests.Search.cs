using System.Threading.Tasks;
using Xunit;
using Interview.Web.Controllers;
using Sparcpoint.Core.Models;
using Sparcpoint.Application.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Sparcpoint.Tests.Controllers
{
    public partial class ProductsControllerTests
    {
        class FakeSearchRepo : IProductRepository
        {
            public Task<int> CreateProductAsync(ProductCreateDto dto) => Task.FromResult(0);
            public Task<IEnumerable<ProductResponseDto>> SearchAsync(string name, Dictionary<string, string> metadata = null, List<string> categories = null)
            {
                var list = new List<ProductResponseDto> { new ProductResponseDto { Id = 7, Name = "Found" } };
                return Task.FromResult((IEnumerable<ProductResponseDto>)list);
            }
        }

        [Fact]
        public async Task Search_ReturnsMatchingProducts()
        {
            var repo = new FakeSearchRepo();
            var controller = new ProductsController(null, repo);

            var dto = new ProductSearchDto { Name = "Test", Metadata = new Dictionary<string, string> { { "SKU", "123" } }, Categories = new List<string> { "Tools" } };
            var result = await controller.Search(dto) as OkObjectResult;

            Assert.NotNull(result);
            var value = result.Value as IEnumerable<ProductResponseDto>;
            Assert.NotNull(value);
        }
    }
}
