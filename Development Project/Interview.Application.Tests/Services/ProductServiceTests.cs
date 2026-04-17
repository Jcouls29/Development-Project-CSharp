using Interview.Application.Abstractions;
using Interview.Application.Services;
using Interview.Application.UseCases.Command;
using Interview.Application.UseCases.Query;
using Interview.Application.UseCases.Result;
using Moq;
using Xunit;

namespace Interview.Application.Tests.Services;

public sealed class ProductServiceTests
{
    private static CreateProductCommand BaseCommand() => new()
    {
        Name = "Valid",
        Description = "Description",
        ProductImageUris = new List<string>(),
        ValidSkus = new List<string>(),
        Attributes = new List<CreateProductAttributeItem>(),
        CategoryIds = new List<int>(),
    };

    [Fact]
    public async Task CreateAsync_WhenNameIsEmptyOrWhitespaceAfterTrim_ReturnsInvalidName()
    {
        var repository = new Mock<IProductRepository>(MockBehavior.Strict);
        var sut = new ProductService(repository.Object);

        var commandSpaces = BaseCommand() with { Name = "   " };
        var resultSpaces = await sut.CreateAsync(commandSpaces);
        Assert.IsType<CreateProductResult.InvalidName>(resultSpaces);

        var commandEmpty = BaseCommand() with { Name = string.Empty };
        var resultEmpty = await sut.CreateAsync(commandEmpty);
        Assert.IsType<CreateProductResult.InvalidName>(resultEmpty);

        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_WhenDuplicateAttributeKeys_ReturnsDuplicateAttributeKeysWithKeysListed()
    {
        var repository = new Mock<IProductRepository>(MockBehavior.Strict);
        var sut = new ProductService(repository.Object);

        var command = BaseCommand() with
        {
            Name = "Product",
            Attributes = new List<CreateProductAttributeItem>
            {
                new() { Key = "Color", Value = "Red" },
                new() { Key = "Color", Value = "Blue" },
            },
        };

        var result = await sut.CreateAsync(command);

        var duplicate = Assert.IsType<CreateProductResult.DuplicateAttributeKeys>(result);
        Assert.Contains("Color", duplicate.Keys);

        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_WhenProductNameExists_ReturnsDuplicateName()
    {
        var repository = new Mock<IProductRepository>();
        repository
            .Setup(r => r.ProductNameExistsAsync("Existing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new ProductService(repository.Object);
        var command = BaseCommand() with { Name = "Existing" };

        var result = await sut.CreateAsync(command);

        Assert.IsType<CreateProductResult.DuplicateName>(result);
        repository.Verify(r => r.InsertProductAsync(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenInsertSucceeds_ReturnsSucceededWithProductId()
    {
        const int expectedId = 42;
        var repository = new Mock<IProductRepository>();
        repository
            .Setup(r => r.ProductNameExistsAsync("NewProduct", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository
            .Setup(r => r.InsertProductAsync(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var sut = new ProductService(repository.Object);
        var command = BaseCommand() with { Name = "NewProduct" };

        var result = await sut.CreateAsync(command);

        var success = Assert.IsType<CreateProductResult.Succeeded>(result);
        Assert.Equal(expectedId, success.ProductInstanceId);
    }

    [Fact]
    public async Task SearchProductsAsync_WhenPageIsLessThanOne_ReturnsInvalidPagination()
    {
        var repository = new Mock<IProductRepository>(MockBehavior.Strict);
        var sut = new ProductService(repository.Object);

        var query = new SearchProductsQuery { Page = 0, PageSize = 10 };

        var outcome = await sut.SearchProductsAsync(query);

        Assert.IsType<SearchProductsOutcomeResult.InvalidPagination>(outcome);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SearchProductsAsync_WhenPageSizeOutOfRange_ReturnsInvalidPagination()
    {
        var repository = new Mock<IProductRepository>(MockBehavior.Strict);
        var sut = new ProductService(repository.Object);

        var query = new SearchProductsQuery { Page = 1, PageSize = 101 };

        var outcome = await sut.SearchProductsAsync(query);

        Assert.IsType<SearchProductsOutcomeResult.InvalidPagination>(outcome);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SearchProductsAsync_WhenValid_DelegatesToRepository()
    {
        var expected = new SearchProductsResult(
            new List<ProductSummary> { new(7, "A", "B") },
            TotalCount: 1,
            Page: 2,
            PageSize: 10);

        var repository = new Mock<IProductRepository>();
        repository
            .Setup(r => r.SearchProductsAsync(It.IsAny<SearchProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var sut = new ProductService(repository.Object);
        var query = new SearchProductsQuery { Page = 2, PageSize = 10 };

        var outcome = await sut.SearchProductsAsync(query);

        var success = Assert.IsType<SearchProductsOutcomeResult.Succeeded>(outcome);
        Assert.Same(expected, success.Result);
        repository.Verify(
            r => r.SearchProductsAsync(
                It.Is<SearchProductsQuery>(q => q.Page == 2 && q.PageSize == 10),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
