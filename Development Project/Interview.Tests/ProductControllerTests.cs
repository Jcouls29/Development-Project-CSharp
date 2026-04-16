using Interview.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Interview.Tests
{
    public class ProductControllerTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly TestWebApplicationFactory<Startup> _factory;

        public ProductControllerTests(TestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateProduct_ValidData_Returns201()
        {
            var request = new CreateProductRequest
            {
                Name = "Widget",
                Description = "Warehouse widget",
                ProductImageUris = new List<string> { "https://example.com/widget.png" },
                ValidSkus = new List<string> { "WIDGET-001" },
                Attributes = new Dictionary<string, string> { ["color"] = "red" },
                CategoryIds = new List<int> { 5 },
            };

            var createdProduct = new ProductDetailModel
            {
                InstanceId = 123,
                Name = request.Name,
                Description = request.Description,
                ProductImageUris = request.ProductImageUris,
                ValidSkus = request.ValidSkus,
                Attributes = request.Attributes,
                Categories = new List<CategoryModel>
                {
                    new CategoryModel
                    {
                        InstanceId = 5,
                        Name = "Hardware",
                        Description = "Hardware items",
                        CreatedTimestamp = DateTime.UtcNow,
                    },
                },
                CreatedTimestamp = DateTime.UtcNow,
            };

            var productService = new Mock<IProductService>();
            productService
                .Setup(service => service.CreateProductAsync(It.Is<CreateProductRequest>(model => model.Name == request.Name)))
                .ReturnsAsync(createdProduct);

            using var client = CreateClient(productService);

            var response = await client.PostAsJsonAsync("/api/v1/products", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            Assert.Contains("/api/v1/products/123", response.Headers.Location!.ToString());

            var body = await response.Content.ReadFromJsonAsync<ProductDetailModel>();
            Assert.NotNull(body);
            Assert.Equal(123, body!.InstanceId);
            Assert.Equal("Widget", body.Name);
            productService.Verify(service => service.CreateProductAsync(It.IsAny<CreateProductRequest>()), Times.Once);
        }

        [Fact]
        public async Task CreateProduct_MissingName_Returns400()
        {
            var request = new CreateProductRequest
            {
                Description = "Warehouse widget",
                ProductImageUris = new List<string>(),
                ValidSkus = new List<string>(),
                Attributes = new Dictionary<string, string>(),
                CategoryIds = new List<int>(),
            };

            var productService = new Mock<IProductService>(MockBehavior.Strict);
            using var client = CreateClient(productService);

            var response = await client.PostAsJsonAsync("/api/v1/products", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(body);
            Assert.Contains("Name", body!.Errors.Keys);
            productService.Verify(service => service.CreateProductAsync(It.IsAny<CreateProductRequest>()), Times.Never);
        }

        [Fact]
        public async Task GetProduct_ById_Returns200()
        {
            var productService = new Mock<IProductService>();
            productService
                .Setup(service => service.GetProductAsync(42))
                .ReturnsAsync(new ProductDetailModel
                {
                    InstanceId = 42,
                    Name = "Widget",
                    Description = "Warehouse widget",
                    Attributes = new Dictionary<string, string> { ["color"] = "blue" },
                    Categories = new List<CategoryModel>(),
                    ProductImageUris = new List<string>(),
                    ValidSkus = new List<string>(),
                    CreatedTimestamp = DateTime.UtcNow,
                });

            using var client = CreateClient(productService);

            var response = await client.GetAsync("/api/v1/products/42");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<ProductDetailModel>();
            Assert.NotNull(body);
            Assert.Equal(42, body!.InstanceId);
            Assert.Equal("Widget", body.Name);
        }

        [Fact]
        public async Task GetProduct_NotFound_Returns404()
        {
            var productService = new Mock<IProductService>();
            productService.Setup(service => service.GetProductAsync(404)).ReturnsAsync((ProductDetailModel)null!);

            using var client = CreateClient(productService);

            var response = await client.GetAsync("/api/v1/products/404");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(body);
            Assert.Equal("Not Found", body!.Title);
            Assert.Contains("404", body.Detail);
        }

        [Fact]
        public async Task SearchProducts_Returns200()
        {
            var request = new ProductSearchRequest
            {
                Name = "Widget",
                Page = 1,
                PageSize = 25,
            };

            var searchResult = new PaginatedResult<ProductModel>
            {
                Items = new List<ProductModel>
                {
                    new ProductModel
                    {
                        InstanceId = 12,
                        Name = "Widget",
                        Description = "Warehouse widget",
                        ProductImageUris = new List<string>(),
                        ValidSkus = new List<string> { "WIDGET-001" },
                        CreatedTimestamp = DateTime.UtcNow,
                    },
                },
                TotalCount = 1,
                Page = 1,
                PageSize = 25,
            };

            var productService = new Mock<IProductService>();
            productService.Setup(service => service.SearchProductsAsync(It.IsAny<ProductSearchRequest>())).ReturnsAsync(searchResult);

            using var client = CreateClient(productService);

            var response = await client.PostAsJsonAsync("/api/v1/products/search", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<PaginatedResult<ProductModel>>();
            Assert.NotNull(body);
            Assert.Single(body!.Items);
            Assert.Equal(1, body.TotalCount);
            Assert.Equal("Widget", body.Items[0].Name);
        }

        private HttpClient CreateClient(Mock<IProductService> productService)
        {
            return _factory.CreateHttpsClient(services =>
            {
                services.RemoveAll<IProductService>();
                services.AddSingleton(productService.Object);
            });
        }
    }
}
