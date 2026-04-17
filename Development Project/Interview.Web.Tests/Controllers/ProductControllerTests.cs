using Interview.Application.Abstractions;
using Interview.Application.UseCases.Command;
using Interview.Application.UseCases.Query;
using Interview.Application.UseCases.Result;
using Interview.Web.Controllers;
using Interview.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Interview.Web.Tests.Controllers;

public sealed class ProductControllerTests
{
    private static CreateProductRequest MinimalRequest() => new()
    {
        Name = "N",
        Description = "D",
        ProductImageUris = new List<string>(),
        ValidSkus = new List<string>(),
        Attributes = new List<CreateProductAttributeRequest>(),
        CategoryIds = new List<int>(),
    };

    [Fact]
    public async Task CreateProduct_WhenSucceeded_Returns201WithLocationAndInstanceId()
    {
        const int instanceId = 42;
        var productService = new Mock<IProductService>();
        productService
            .Setup(s => s.CreateAsync(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateProductResult.Succeeded(instanceId));

        var sut = new ProductController(productService.Object);
        var result = await sut.CreateProduct(MinimalRequest(), CancellationToken.None);

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal($"/api/v1/products/{instanceId}", created.Location);

        var body = Assert.IsType<CreateProductResponse>(created.Value);
        Assert.Equal(instanceId, body.InstanceId);
    }

    [Fact]
    public async Task CreateProduct_WhenInvalidName_Returns400()
    {
        var productService = new Mock<IProductService>();
        productService
            .Setup(s => s.CreateAsync(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateProductResult.InvalidName());

        var sut = new ProductController(productService.Object);
        var result = await sut.CreateProduct(MinimalRequest(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public async Task SearchProducts_WhenSucceeded_Returns200WithPayload()
    {
        var searchResult = new SearchProductsResult(
            new List<ProductSummary> { new(3, "Name", "Desc") },
            TotalCount: 9,
            Page: 2,
            PageSize: 5);

        var productService = new Mock<IProductService>();
        productService
            .Setup(s => s.SearchProductsAsync(It.IsAny<SearchProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchProductsOutcomeResult.Succeeded(searchResult));

        var sut = new ProductController(productService.Object);
        var request = new SearchProductsRequest { Page = 2, PageSize = 5 };

        var result = await sut.SearchProducts(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<SearchProductsResponse>(ok.Value);
        Assert.Equal(9, body.TotalCount);
        Assert.Equal(2, body.Page);
        Assert.Equal(5, body.PageSize);
        Assert.Single(body.Items);
        Assert.Equal(3, body.Items[0].InstanceId);
    }

}
