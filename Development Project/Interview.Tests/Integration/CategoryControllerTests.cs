using Interview.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Interview.Tests
{
    public class CategoryControllerTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly TestWebApplicationFactory<Startup> _factory;

        public CategoryControllerTests(TestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateCategory_ValidData_Returns201()
        {
            var request = new CreateCategoryRequest
            {
                Name = "Hardware",
                Description = "Warehouse hardware",
                Attributes = new Dictionary<string, string> { ["type"] = "stock" },
                ParentCategoryIds = new List<int> { 1 },
            };

            var categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(service => service.CreateCategoryAsync(It.Is<CreateCategoryRequest>(model => model.Name == request.Name)))
                .ReturnsAsync(new CategoryDetailModel
                {
                    InstanceId = 10,
                    Name = request.Name,
                    Description = request.Description,
                    Attributes = request.Attributes,
                    ParentCategoryIds = request.ParentCategoryIds,
                    CreatedTimestamp = DateTime.UtcNow,
                });

            using var client = CreateClient(categoryService);

            var response = await client.PostAsJsonAsync("/api/v1/categories", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            Assert.Contains("/api/v1/categories/10", response.Headers.Location!.ToString());

            var body = await response.Content.ReadFromJsonAsync<CategoryDetailModel>();
            Assert.NotNull(body);
            Assert.Equal(10, body!.InstanceId);
            Assert.Equal("Hardware", body.Name);
        }

        [Fact]
        public async Task GetCategory_ById_Returns200()
        {
            var categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(service => service.GetCategoryAsync(11))
                .ReturnsAsync(new CategoryDetailModel
                {
                    InstanceId = 11,
                    Name = "Electronics",
                    Description = "Warehouse electronics",
                    Attributes = new Dictionary<string, string> { ["type"] = "fragile" },
                    ParentCategoryIds = new List<int> { 2 },
                    CreatedTimestamp = DateTime.UtcNow,
                });

            using var client = CreateClient(categoryService);

            var response = await client.GetAsync("/api/v1/categories/11");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<CategoryDetailModel>();
            Assert.NotNull(body);
            Assert.Equal(11, body!.InstanceId);
            Assert.Equal("Electronics", body.Name);
        }

        [Fact]
        public async Task AddParentCategory_Returns200()
        {
            var categoryService = new Mock<ICategoryService>();
            categoryService
                .Setup(service => service.AddParentCategoryAsync(15, 3))
                .Returns(Task.CompletedTask);
            categoryService
                .Setup(service => service.GetCategoryAsync(15))
                .ReturnsAsync(new CategoryDetailModel
                {
                    InstanceId = 15,
                    Name = "Child",
                    Description = "Child category",
                    Attributes = new Dictionary<string, string>(),
                    ParentCategoryIds = new List<int> { 3 },
                    CreatedTimestamp = DateTime.UtcNow,
                });

            using var client = CreateClient(categoryService);

            var response = await client.PostAsync("/api/v1/categories/15/parents/3", new StringContent(string.Empty));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<CategoryDetailModel>();
            Assert.NotNull(body);
            Assert.Contains(3, body!.ParentCategoryIds);
            categoryService.Verify(service => service.AddParentCategoryAsync(15, 3), Times.Once);
            categoryService.Verify(service => service.GetCategoryAsync(15), Times.Once);
        }

        private HttpClient CreateClient(Mock<ICategoryService> categoryService)
        {
            return _factory.CreateHttpsClient(services =>
            {
                services.RemoveAll<ICategoryService>();
                services.AddSingleton(categoryService.Object);
            });
        }
    }
}
