using System;
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

    #region SearchAsync

    [Fact]
    public async Task SearchAsync_WithNoFilters_ReturnsAllProducts()
    {
        var expected = new List<ProductResponse> { new ProductResponse { InstanceId = 1, Name = "Widget" } };
        _repositoryMock.Setup(r => r.SearchAsync(It.IsAny<ProductSearchRequest>()))
            .ReturnsAsync(expected);

        var result = await _sut.SearchAsync(new ProductSearchRequest());

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task SearchAsync_DelegatesToRepository()
    {
        var request = new ProductSearchRequest { Name = "Widget" };
        _repositoryMock.Setup(r => r.SearchAsync(request)).ReturnsAsync(new List<ProductResponse>());

        await _sut.SearchAsync(request);

        _repositoryMock.Verify(r => r.SearchAsync(request), Times.Once);
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ValidRequest_DelegatesToRepository()
    {
        var request = new CreateProductRequest { Name = "Widget", Description = "A widget" };
        _repositoryMock.Setup(r => r.CreateAsync(request)).ReturnsAsync(1);

        var id = await _sut.CreateAsync(request);

        Assert.Equal(1, id);
        _repositoryMock.Verify(r => r.CreateAsync(request), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateAsync(null));
    }

    [Fact]
    public async Task CreateAsync_NullName_ThrowsArgumentException()
    {
        var request = new CreateProductRequest { Name = null, Description = "A widget" };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WhitespaceName_ThrowsArgumentException()
    {
        var request = new CreateProductRequest { Name = "   ", Description = "A widget" };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_NameExceeds256Chars_ThrowsArgumentException()
    {
        var request = new CreateProductRequest
        {
            Name = new string('a', 257),
            Description = "A widget"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_DescriptionExceeds256Chars_ThrowsArgumentException()
    {
        var request = new CreateProductRequest
        {
            Name = "Widget",
            Description = new string('a', 257)
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(request));
    }

    #endregion
}
