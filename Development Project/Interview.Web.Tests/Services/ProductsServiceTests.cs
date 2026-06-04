using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using Interview.Web.Services.Implementations;
using Moq;
using Xunit;

namespace Interview.Web.Tests.Services;

public class ProductsServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductsService _sut;

    public ProductsServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _sut = new ProductsService(_repositoryMock.Object);
    }

    // EVAL: These tests will fail until Milestone 2 implements the real logic.
    // They exist now so the interfaces are locked in and any breaking change is caught immediately.

    [Fact]
    public async Task SearchAsync_WithNoFilters_ReturnsAllProducts()
    {
        var expected = new List<Product> { new Product { InstanceId = 1, Name = "Widget" } };
        _repositoryMock.Setup(r => r.SearchAsync(It.IsAny<ProductSearchRequest>()))
            .ReturnsAsync(expected);

        var result = await _sut.SearchAsync(new ProductSearchRequest());

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task SearchAsync_DelegatesToRepository()
    {
        var request = new ProductSearchRequest { Name = "Widget" };
        _repositoryMock.Setup(r => r.SearchAsync(request)).ReturnsAsync(new List<Product>());

        await _sut.SearchAsync(request);

        _repositoryMock.Verify(r => r.SearchAsync(request), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DelegatesToRepository()
    {
        var request = new CreateProductRequest { Name = "Widget", Description = "A widget" };
        _repositoryMock.Setup(r => r.CreateAsync(request)).ReturnsAsync(1);

        var id = await _sut.CreateAsync(request);

        Assert.Equal(1, id);
        _repositoryMock.Verify(r => r.CreateAsync(request), Times.Once);
    }
}
