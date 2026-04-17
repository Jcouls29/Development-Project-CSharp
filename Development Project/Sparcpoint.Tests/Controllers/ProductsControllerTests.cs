using System.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Interview.Web.Controllers;
using Sparcpoint.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Application.Repositories;
using System.Collections.Generic;

namespace Sparcpoint.Tests.Controllers
{
    public partial class ProductsControllerTests
    {
        class FakeProductRepo : IProductRepository
        {
            private readonly int _id;
            public FakeProductRepo(int id) { _id = id; }
            public Task<int> CreateProductAsync(ProductCreateDto dto) => Task.FromResult(_id);
            public Task<ProductResponseDto> GetByIdAsync(int id) => Task.FromResult((ProductResponseDto)null);
            public Task<IEnumerable<ProductResponseDto>> SearchAsync(string name, System.Collections.Generic.Dictionary<string, string> metadata = null, System.Collections.Generic.List<string> categories = null)
            {
                return Task.FromResult((IEnumerable<ProductResponseDto>)new System.Collections.Generic.List<ProductResponseDto>());
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedWithId()
        {
            var repo = new FakeProductRepo(42);
            var controller = new ProductsController(repo);

            var dto = new ProductCreateDto { Name = "Test", Metadata = null, Categories = null };
            var result = await controller.Create(dto) as CreatedAtActionResult;

            Assert.NotNull(result);
            Assert.IsType<ProductResponseDto>(result.Value);
            var response = (ProductResponseDto)result.Value;
            Assert.Equal(42, response.Id);
        }
    }
}
