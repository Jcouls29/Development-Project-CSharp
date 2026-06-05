using System;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using Interview.Web.Services.Implementations;
using Moq;
using Xunit;

namespace Interview.Web.Tests.Services;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _repositoryMock;
    private readonly InventoryService _sut;

    public InventoryServiceTests()
    {
        _repositoryMock = new Mock<IInventoryRepository>();
        _sut = new InventoryService(_repositoryMock.Object);
    }

    #region AddAsync

    [Fact]
    public async Task AddAsync_ValidRequest_DelegatesToRepository()
    {
        var request = new AddInventoryRequest { ProductInstanceId = 1, Quantity = 10 };
        _repositoryMock.Setup(r => r.AddAsync(request)).ReturnsAsync(1);

        var id = await _sut.AddAsync(request);

        Assert.Equal(1, id);
        _repositoryMock.Verify(r => r.AddAsync(request), Times.Once);
    }

    [Fact]
    public async Task AddAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddAsync(null));
    }

    [Fact]
    public async Task AddAsync_ZeroQuantity_ThrowsArgumentException()
    {
        var request = new AddInventoryRequest { ProductInstanceId = 1, Quantity = 0 };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddAsync(request));
    }

    [Fact]
    public async Task AddAsync_InvalidProductInstanceId_ThrowsArgumentException()
    {
        var request = new AddInventoryRequest { ProductInstanceId = 0, Quantity = 10 };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddAsync(request));
    }

    #endregion

    #region RemoveTransactionAsync

    [Fact]
    public async Task RemoveTransactionAsync_ValidId_DelegatesToRepository()
    {
        await _sut.RemoveTransactionAsync(1);

        _repositoryMock.Verify(r => r.RemoveTransactionAsync(1), Times.Once);
    }

    [Fact]
    public async Task RemoveTransactionAsync_InvalidId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.RemoveTransactionAsync(0));
    }

    #endregion

    #region GetCountAsync

    [Fact]
    public async Task GetCountAsync_ValidRequest_DelegatesToRepository()
    {
        var request = new InventoryCountRequest { ProductInstanceId = 1 };
        _repositoryMock.Setup(r => r.GetCountAsync(request)).ReturnsAsync(42m);

        var count = await _sut.GetCountAsync(request);

        Assert.Equal(42m, count);
        _repositoryMock.Verify(r => r.GetCountAsync(request), Times.Once);
    }

    [Fact]
    public async Task GetCountAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetCountAsync(null));
    }

    #endregion
}
